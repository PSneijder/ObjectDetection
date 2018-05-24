using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Device = ObjectDetection.WebApp.Models.Device;

namespace ObjectDetection.WebApp.Managers
{
    public sealed class DeviceManager
    {
        private static ServiceClient _serviceClient;
        private static RegistryManager _registryManager;

        public DeviceManager(IConfiguration config)
        {
            _serviceClient = ServiceClient.CreateFromConnectionString(config["Server:ServiceConnectionString"]);
            _registryManager = RegistryManager.CreateFromConnectionString(config["Server:RegistryConnectionString"]);
        }
        
        public async Task SendDeviceCommand(string deviceId, string methodName)
        {
            CloudToDeviceMethod method = new CloudToDeviceMethod(methodName);
            await _serviceClient.InvokeDeviceMethodAsync(deviceId, method);
        }

        public async Task SendDeviceMessage(string deviceId, string value)
        {
            Message message = new Message(Encoding.ASCII.GetBytes(value));
            await _serviceClient.SendAsync(deviceId, message);
        }

        public async Task<Device[]> ListDevices()
        {
            List<Device> devices = new List<Device>();

            IQuery query = _registryManager.CreateQuery("SELECT * FROM devices");

            while (query.HasMoreResults)
            {
                IEnumerable<Twin> page = await query.GetNextAsTwinAsync();

                foreach (Twin twin in page)
                {
                    devices.Add(new Device
                    {
                        Id = twin.DeviceId,
                        ConnectionState = twin.ConnectionState?.ToString(),
                        LastActivityTime = twin.LastActivityTime
                    });
                }
            }

            return devices.ToArray();
        }
    }
}