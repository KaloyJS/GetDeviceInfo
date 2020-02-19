using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GetDeviceInfo
{
    class AndroidDevice
    {
        public string OEM;
        public string IMEI;
        public string Model;
        public string SoftwareVersion;
        public string CurrentVersion;
        public string SerialNumber;
        public string Capacity;
        public string Color;
        public string imei_query = "\"service call iphonesubinfo 1 | grep -o '[0-9a-f]\\{8\\} ' | tail -n+3 | while read a; do echo -n \\\\u${a:4:4}\\\\u${a:0:4}; done\"";

        public AndroidDevice() 
        {
            this.IMEI = GetIMEI();
            this.OEM = GetOEM();
            this.Model = GetModel(this.OEM);
            this.SoftwareVersion = GetSoftwareVersion(this.OEM);
            this.SerialNumber = GetSN();
            this.Capacity = GetCapacity(this.OEM);
            this.Color = GetColor(this.OEM);
            //this.CurrentVersion = GetCurrentVersion(this.Model);
        }

        public string GetIMEI() 
        {
            
            string imei_query = "\"service call iphonesubinfo 1 | grep -o '[0-9a-f]\\{8\\} ' | tail -n+3 | while read a; do echo -n \\\\u${a:4:4}\\\\u${a:0:4}; done\"";
            string imei = ExecuteCommandSync.AndroidCommand(imei_query);
            if (string.IsNullOrEmpty(imei.Trim())) 
            {
                imei_query = "service call iphonesubinfo 1 | toybox cut -d \"'\" -f2 | toybox grep -Eo '[0 - 9]' | toybox xargs | toybox sed 's /\\ //g'";
                imei = ExecuteCommandSync.AndroidCommand(imei_query);
            }
            string cleanIMEI = RemoveNonASCII(imei);
            return cleanIMEI;
        }

        public string RemoveNonASCII(string unicode) 
        {
            char[] unicodeArray = unicode.ToCharArray();
            string ascii = "";
            for (int i = 0; i < unicodeArray.Length; i++)
            {
                if (((int)unicodeArray[i]) <= 128) ascii += unicodeArray[i].ToString();
                else if (((int)unicodeArray[i]) == 8217)
                {
                    ascii += "''";
                }
                else
                {
                    ascii += " ";
                }
            }

            return ascii;
        }

        public string GetCurrentVersion(string model) 
        {
            string url = "https://portal-ca.sbe-ltd.ca/screening_new/device_info/actions.php";            
            string currentVersion = CallToPHP.GetPost(url, "get_current_version", "1", "model", model);           
            return currentVersion;
        }

        public string GetOEM() 
        {
            string oem = ExecuteCommandSync.AndroidCommand("getprop ro.product.vendor.manufacturer");
            if (string.IsNullOrEmpty(oem.Trim())) 
            {
                oem = ExecuteCommandSync.AndroidCommand("getprop ro.product.manufacturer");
            }
            return oem;
        }

        public string GetSN() {
            return ExecuteCommandSync.AndroidCommand("getprop ro.serialno");
        }

        public string GetModel(string oem)
        {
            string model;
            string manufacturer = oem.Trim();

            switch (manufacturer.ToUpper()) 
            {
                case "HUAWEI":
                    model = ExecuteCommandSync.AndroidCommand("getprop ro.product.model");
                    break;
                case "LGE":
                    model = ExecuteCommandSync.AndroidCommand("getprop ro.product.model");
                    break;
                case "MOTOROLA":
                    model = ExecuteCommandSync.AndroidCommand("getprop ro.boot.hardware.sku");
                    break;
                case "SONY":
                    model = ExecuteCommandSync.AndroidCommand("getprop ro.semc.product.model");
                    break;
                default:
                    model = ExecuteCommandSync.AndroidCommand("getprop ro.product.vendor.model");
                    break;
            }
            return model;
        }

        public string GetSoftwareVersion(string oem) 
        {
            string sv;
            string manufacturer = oem.Trim();

            switch (manufacturer.ToUpper())
            {
                case "SAMSUNG":
                    sv = ExecuteCommandSync.AndroidCommand("getprop ro.bootloader");
                    break;
                case "HUAWEI":
                    sv = ExecuteCommandSync.AndroidCommand("getprop ro.build.display.id");
                    break;
                case "LGE":
                    sv = ExecuteCommandSync.AndroidCommand("getprop ro.vendor.lge.swversion");
                    if (string.IsNullOrEmpty(sv.Trim()))
                    {
                        sv = ExecuteCommandSync.AndroidCommand("getprop ro.lge.swversion");
                    }
                    break;
                default:
                    sv = ExecuteCommandSync.AndroidCommand("getprop ro.build.id");
                    break;
            }

            return sv;
        }

        public string CalculateCapacity(string capacityLine) {
            // Class to crudely get the device capacity by querying adb shell df -h command to see drive partition and sizes
            // Turning the result into an array, then going to the last line, where it has the /data/media attributes , removing white spaces and using substring to get the 
            // first instance of G which indicates the size capacity.
            // Then determine if that is under 8GB, 16GB, 32GB, 64GB, 128GB, 256GB, 512GB
            // I know its crude but its the best way I could think of right now - CarloJS ;)


            //string[] capacity_arr = Regex.Split(getCapacity, "\n");
            //int capacity_arr_length = capacity_arr.Length - 2;
            //string capacityLine = capacity_arr[capacity_arr_length];
            //string cap = string.Join("", capacityLine.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            //cap = cap.Remove(0, 11);
            //int index = cap.IndexOf('G');
            //capacity = cap.Substring(0, index);


            string capacity = "";
            // Splits the result into array with a delimiter of \n (new line)
            string[] capacity_arr = Regex.Split(capacityLine, "\n");
            // Getting the index of the data/media line in array
            int data_media_index = capacity_arr.Length - 2;
            // assinging the /data/media line in a string
            string cap = capacity_arr[data_media_index];
            // Removing White spaces inside
            cap = string.Join("", cap.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
            // Removing the /data/media in string
            cap = cap.Remove(0, 11);
            //Getting index of first G(to indicate the GB)
            int index = cap.IndexOf('G');
            // Using substring to get capacity value
            cap = cap.Substring(0, index);
            if (Int32.Parse(cap) < 4)
            {
                capacity = "4GB";
            }
            else if (Int32.Parse(cap) < 8)
            {
                capacity = "8GB";
            }
            else if (Int32.Parse(cap) < 16)
            {
                capacity = "16GB";
            }
            else if (Int32.Parse(cap) < 32)
            {
                capacity = "32GB";
            }
            else if (Int32.Parse(cap) < 64)
            {
                capacity = "64GB";
            }
            else if (Int32.Parse(cap) < 128)
            {
                capacity = "128GB";
            }
            else if (Int32.Parse(cap) < 256)
            {
                capacity = "256GB";
            }
            else if (Int32.Parse(cap) < 512)
            {
                capacity = "512GB";
            }

            return capacity;

        }

        public string GetCapacity(string oem) {
            string capacity;
            string manufacturer = oem.Trim();

            switch (manufacturer.ToUpper())
            {
                case "GOOGLE":
                    capacity = ExecuteCommandSync.AndroidCommand("getprop ro.boot.hardware.ufs");
                    break;

                default:
                    string capacityLine = ExecuteCommandSync.AndroidCommand("df -h");
                    if (string.IsNullOrEmpty(capacityLine))
                    {
                        capacity = "";
                    }
                    else
                    {
                        capacity = CalculateCapacity(capacityLine);
                    }
                    break;
            }

            return capacity;

        }

        public string GetColor(string oem) {
            string color;
            string manufacturer = oem.Trim();
            switch (manufacturer.ToUpper())
            {
                case "GOOGLE":
                    color = ExecuteCommandSync.AndroidCommand("getprop ro.boot.hardware.color");
                    break;
                case "LGE":
                    color = ExecuteCommandSync.AndroidCommand("getprop ro.boot.product.lge.device_color");
                    break;
                case "HUAWEI":
                    color = ExecuteCommandSync.AndroidCommand("getprop ro.config.devicecolor");
                    break;
                default:
                    color = ExecuteCommandSync.AndroidCommand("getprop ro.boot.hardware.color");
                    break;
            }

            return color;
        }

    }
}
