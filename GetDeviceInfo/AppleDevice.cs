using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceInfo
{
    class AppleDevice
    {
        public string IMEI;
        public string Model;
        public string SoftwareVersion;
        public string OEM;
        public string SerialNumber;

        public AppleDevice() 
        {
            IMEI = ExecuteCommandSync.AppleCommand("ideviceinfo -k  InternationalMobileEquipmentIdentity");
            Model = ExecuteCommandSync.AppleCommand("ideviceinfo -k ModelNumber");
            SoftwareVersion = ExecuteCommandSync.AppleCommand("ideviceinfo -k ProductVersion");
            OEM = "Apple";
            SerialNumber = ExecuteCommandSync.AppleCommand("ideviceinfo -k SerialNumber");
        }

    }
}
