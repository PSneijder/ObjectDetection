using System.ComponentModel.DataAnnotations;

namespace ObjectDetection.WebApp.Models
{
    public enum Commands
    {
        [Display(Name = "CreateSnapshot", Description = "Creates a snapshot from device camera.")]
        CreateSnapshot = 0
    }
}