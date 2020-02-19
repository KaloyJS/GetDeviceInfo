using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace GetDeviceInfo
{
    class ExecuteCommandSync
    {
        public static string CommandPrompt(object command)
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.WorkingDirectory = @"c:\";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Returns result;
            return result;
        }

        public static string GetDeviceType() 
        {
            string result;
            // Query to detect if there are any Apple or ADB enabled devices are connected
            string usb_query = CommandPrompt("wmic path Win32_PnPEntity where \" Description like  '%ADB%' OR Description like '%Apple%'\" get /value");
            if (usb_query.Length > 9) {
                // Block to determine if device is Apple or Android ADB Enabled Device
                //Apple Query
                string apple_result = CommandPrompt("wmic path Win32_PnPEntity where \" Description like '%Apple%' \" get /value ");
                if (apple_result.Length > 9)
                {
                    result = "Apple";
                }
                else
                {
                    result = "Android";
                }
            } else {
                result = "Unknown Device";            
            }

            return result;
            
        }

        public static string AppleCommand(object command)
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.WorkingDirectory = @"Files\";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            //Console.WriteLine(result);
            return result;
        }

        public static string AndroidCommand(object command)
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c adb shell " + command);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.WorkingDirectory = @"c:\";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            //Console.WriteLine(result);
            return result;
        }

/*        public static string GetIMEI(string deviceType) 
        { 
            
        }*/

    }
}
