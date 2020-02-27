using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetDeviceInfo;

namespace GetDeviceInfo
{

    public class DeviceChangeEventArgs : System.EventArgs
    {
        public DeviceChange DeviceChange { get; set; }

    }
}
