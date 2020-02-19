using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace GetDeviceInfo
{
    class CallToPHP
    {
        // Class to send data to php endpoint where it takes in url of the php endpoint ie. https://www.site.com/php/endpoint.php 
        // and then params and value separated by comma ie . 
        // example: status = CallToPHP.GetPost(postURL, "save_device_info", "1", "imei", imei, "model", model, "software_version", softwareVersion, "OEM", oem, "serial_number", serialNumber, "workStation", workStation, "capacity", capacity, "color", capacity);

        public static string GetPost(string Url, params string[] postdata) 
        {
            string result = string.Empty;
            string data = string.Empty;

            System.Text.ASCIIEncoding ascii = new ASCIIEncoding();

            if (postdata.Length % 2 != 0)
            {
                throw new Exception("Parameters must be even , \"Parameter\" , \"value\" , ... etc");
            }

            for (int i = 0; i < postdata.Length; i += 2)
            {
                data += string.Format("&{0}={1}", postdata[i], postdata[i + 1]);
            }

            data = data.Remove(0, 1);
            byte[] bytesarr = ascii.GetBytes(data);
            try
            {
                WebRequest request = WebRequest.Create(Url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytesarr.Length;

                System.IO.Stream streamwriter = request.GetRequestStream();
                streamwriter.Write(bytesarr, 0, bytesarr.Length);
                streamwriter.Close();

                WebResponse response = request.GetResponse();
                streamwriter = response.GetResponseStream();

                System.IO.StreamReader streamread = new System.IO.StreamReader(streamwriter);
                result = streamread.ReadToEnd();
                streamread.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
