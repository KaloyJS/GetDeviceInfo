using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceInfo.Data
{
    /// <summary>
    ///  Class to house Port Status properties
    /// </summary>
    class PortStatus
    {
        public string status { set; get; }
        // Constructor initializes with Empty status
        public PortStatus() {
            status = "Empty";
        }
    }
}
