using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GetDeviceInfo
{
    
    class Device
    {
        /// <summary>
        /// Class for device properties
        /// </summary>
        /// 

        

        public string Manufacturer { set; get;}
        // For apple devices I would need UDID
        public string UDID { set; get; }

        public string SerialNumber { set; get; }

        public string IMEI { set; get; }

        public string ModelNumber { set; get; }

        public string ModelName { set; get; }

        public string SoftwareVersion { set; get; }

        public string CodePro { set; get; }

        public string Color { set; get; }

        public string Capacity { set; get; }

       
        /// <summary>
        /// Resets the class properties
        /// </summary>
        public void Reset() {
            
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; ++i)
                properties[i].SetValue(this, null);
        }

    }
}
