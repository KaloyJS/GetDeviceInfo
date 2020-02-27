using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceInfo
{
    /// <summary>
    /// Methods for Getting Device Properties for Apple Devices
    /// </summary>
    class AppleCommands
    {
        /// <summary>
        /// Gets and Calculates the connected device UDID which is needed to get the device property on apple devices
        /// </summary>
        /// <param name="NumberOfConnectedDevice">Number of connected device</param>
        /// <param name="connectedDeviceUDID">String array of Connected Device UDID recorded</param>
        /// <returns></returns>
        public static string GetUdid(int NumberOfConnectedDevice, List<string> connectedDeviceUDID) {
            string udid;
            if (NumberOfConnectedDevice <= 1)
            {
                udid = ExecuteCommandSync.AppleCommand("idevice_id -l");
            }
            else
            {
                udid = ExecuteCommandSync.AppleCommand("idevice_id -l");
                foreach (string item in connectedDeviceUDID)
                {
                    udid = udid.Replace(item, "");
                }
            } 
            return udid.Trim();
        }

        /// <summary>
        /// Gets IMEI from ideviceinfo command
        /// </summary>
        /// <param name="UDID">UDID of device</param>
        /// <returns></returns>
        public static string GetIMEI(string UDID) 
        {
            string res = ExecuteCommandSync.AppleCommand($"ideviceinfo -u {UDID} -k InternationalMobileEquipmentIdentity");
            return res.Trim();
        }

        /// <summary>
        /// Gets SerialNumber from ideviceinfo command
        /// </summary>
        /// <param name="UDID">UDID of device</param>
        /// <returns></returns>
        public static string GetSerialNumber(string UDID)
        {
            string res = ExecuteCommandSync.AppleCommand($"ideviceinfo -u {UDID} -k SerialNumber");
            return res.Trim();
        }

        /// <summary>
        /// Gets ModelNumber from ideviceinfo command
        /// </summary>
        /// <param name="UDID">UDID of device</param>
        /// <returns></returns>
        public static string GetModelNumber(string UDID)
        {
            string res = ExecuteCommandSync.AppleCommand($"ideviceinfo -u {UDID} -k ModelNumber");
            return res.Trim();
        }

        /// <summary>
        /// Gets SoftwareVersion from ideviceinfo command
        /// </summary>
        /// <param name="UDID">UDID of device</param>
        /// <returns></returns>
        public static string GetSoftwareVersion(string UDID)
        {
            string res = ExecuteCommandSync.AppleCommand($"ideviceinfo -u {UDID} -k ProductVersion");
            return res.Trim();
        }

        /// <summary>
        /// Calls to an php endpoint and it returns Codepro of product 
        /// </summary>
        /// <param name="Model">ModelNumber property of device</param>
        /// <returns></returns>
        public static string GetCodePro(string Model) { 
            string endPoint = "https://portal-ca.sbe-ltd.ca/SBE_Applications/actions.php";
            return CallToPHP.GetPost(endPoint, "get_apple-codepro", "1", "model", Model);
        }

        /// <summary>
        /// Calls to an php endpoint and it returns ModelName of product 
        /// </summary>
        /// <param name="Model">ModelNumber property of device</param>
        /// <returns></returns>
        public static string GetModelName(string Model) {
            string endPoint = "https://portal-ca.sbe-ltd.ca/SBE_Applications/actions.php";
            return CallToPHP.GetPost(endPoint, "get_apple-modelName", "1", "model", Model);
        }

        /// <summary>
        /// Calls to an php endpoint and it returns color of product 
        /// </summary>
        /// <param name="Model">ModelNumber property of device</param>
        /// <returns></returns>
        public static string GetColor(string Model)
        {
            string endPoint = "https://portal-ca.sbe-ltd.ca/SBE_Applications/actions.php";
            return CallToPHP.GetPost(endPoint, "get_apple-color", "1", "model", Model);
        }

        /// <summary>
        /// Calls to an php endpoint and it returns capacity of product 
        /// </summary>
        /// <param name="Model">ModelNumber property of device</param>
        /// <returns></returns>
        public static string GetCapacity(string Model)
        {
            string endPoint = "https://portal-ca.sbe-ltd.ca/SBE_Applications/actions.php";
            return CallToPHP.GetPost(endPoint, "get_apple-capacity", "1", "model", Model);
        }



    }
}
