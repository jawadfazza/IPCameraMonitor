using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraMonitor
{
    [Serializable]
    public class CameraConfig
    {
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StreamType { get; set; }
        public string Nickname { get; set; }
        public List<string> Groups { get; set; } // New Groups property
    }


    public class CameraRecord
    {
        public string IPAddress { get; set; }
        public string FilePath { get; set; }
        public DateTime RecordedAt { get; set; }
    }

}
