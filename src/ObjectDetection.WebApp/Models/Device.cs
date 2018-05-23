using System;

namespace ObjectDetection.WebApp.Models
{
    public sealed class Device
    {
        public string Id { get; set; }
        public string ConnectionState { get; set; }
        public DateTime? LastActivityTime { get; set; }
    }
}