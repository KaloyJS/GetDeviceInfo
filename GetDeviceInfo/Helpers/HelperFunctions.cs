using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System;
using System.Windows;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.MessageBox;

namespace GetDeviceInfo
{
    class HelperFunctions
    {
        #region portstatus functions

        /// <summary>
        /// Initializes port status hash table
        /// </summary>
        public static void initializePortStatus(Hashtable hashtable)
        {
            hashtable.Add("1", "Empty");
            hashtable.Add("2", "Empty");
            hashtable.Add("3", "Empty");
        }



        /// <summary>
        /// Reset port status
        /// </summary>
        public static void resetPortStatus(Hashtable hashtable)
        {
            hashtable["1"] = "Empty";
            hashtable["2"] = "Empty";
            hashtable["3"] = "Empty";
        }

        #endregion

        public static void showConnectedDevice(Hashtable connectedDevice)
        {
            string output = "";
            foreach (DictionaryEntry de in connectedDevice)
            {
                output += "\n Port: " + de.Key.ToString() + " Status: " + de.Value.ToString();
            }

            MessageBox.Show(output);
        }
        /// <summary>
        /// Returns true if only Single UDID is recorded , false if two or more 
        /// </summary>
        /// <param name="UDID"></param>
        /// <returns></returns>
        public static Boolean checkUDID(string UDID)
        {
            // Splits UDID on New lines
            string[] lines = Regex.Split(UDID, "\r\n");

            if (lines.Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
