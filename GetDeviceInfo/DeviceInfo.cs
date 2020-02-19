using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceInfo
{
    class DeviceInfo
    {
        public string deviceType;        

        public DeviceInfo()
        {
            deviceType = ExecuteCommandSync.GetDeviceType();     


        }


    }
}
