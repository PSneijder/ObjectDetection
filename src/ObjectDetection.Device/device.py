import requests
import random
import time
import json
import sys
import io

import picamera
import struct

import iothub_client

# pylint: disable=E0611
from iothub_client import IoTHubClient, IoTHubClientError, IoTHubTransportProvider, IoTHubClientResult
from iothub_client import IoTHubMessage, IoTHubMessageDispositionResult, IoTHubError, DeviceMethodReturnValue

with open('config.json') as configuration:    
    config = json.load(configuration)

PROTOCOL = IoTHubTransportProvider.MQTT
MESSAGE_TIMEOUT = 10000

RECEIVE_CONTEXT = 0
METHOD_CONTEXT = 0
INTERVAL = 1

def create_snapshot():
    with picamera.PiCamera() as camera:
        camera.resolution = (640, 480)
        camera.start_preview()
        time.sleep(2)
        stream = io.BytesIO()
        camera.capture(stream, format='jpeg')
        stream.seek(0)
    return stream

def send_confirmation_callback(message, result, user_context):
    print ( "IoT Hub responded to message with status: %s" % (result) )

def iothub_client_init():
    # Create an IoT Hub client
    client = IoTHubClient(config['connectionString'], PROTOCOL)
    return client

# Handle direct message calls from IoT Hub
def receive_message_callback(message, counter):
    message_buffer = message.get_bytearray()
    size = len(message_buffer)
    print ( "Received Message [%d]:" % counter )
    print ( "    Data: <<<%s>>> & Size=%d" % (message_buffer[:size].decode('utf-8'), size) )
    map_properties = message.properties()
    key_value_pair = map_properties.get_internals()
    print ( "    Properties: %s" % key_value_pair )
    counter += 1
    return IoTHubMessageDispositionResult.ACCEPTED

# Handle direct method calls from IoT Hub
def device_method_callback(method_name, payload, user_context):
    if method_name == "CreateSnapshot":
        with create_snapshot() as stream:
            request = requests.post(config['apiUrl'], data=stream)

    device_method_return_value = DeviceMethodReturnValue()
    device_method_return_value.response = "{ \"Response\": \"This is the response from the device\" }"
    device_method_return_value.status = 200
    return device_method_return_value

def iothub_client_run():

    try:
        client = iothub_client_init()
        print ( "IoT Hub RaspberyPi Device, press Ctrl-C to exit" )

        # Set up the callbacks for direct message/method calls from the hub.
        client.set_message_callback(receive_message_callback, RECEIVE_CONTEXT)
        client.set_device_method_callback(device_method_callback, METHOD_CONTEXT)

        while True:
            time.sleep(INTERVAL)

    except IoTHubError as iothub_error:
        print ( "Unexpected error %s from IoTHub" % iothub_error )
        return
    except KeyboardInterrupt:
        print ( "IoTHubClient stopped" )

if __name__ == '__main__':
    print ( "IoT Hub - RaspberryPi device" )
    print ( "Press Ctrl-C to exit" )
iothub_client_run()