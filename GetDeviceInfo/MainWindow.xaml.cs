using GetDeviceInfo.Enum;
using System;
using System.Windows;
using System.Windows.Interop;

namespace GetDeviceInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Application that detects if a usb device has been connected
    /// it then determines if it is a Apple or a Android device
    /// Then depending on the type of mobile phone it then creates an Apple or a Android Object
    /// To get Its Info (ie. IMEI, Serial Number, Model, Capacity)
    /// Then saves it in a php endpoint that saves it in a database
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr windowHandle;
        public string imei;
        public string model;
        public string softwareVersion;
        public string oem;
        public string serialNumber;
        public string status;
        public string capacity;
        public string color;
        public string workStation = System.Environment.MachineName;
        private string postURL = "https://portal-ca.sbe-ltd.ca/screening_new/device_info/actions.php";
        AppleDevice appleDevice;
        AndroidDevice androidDevice;

        // clears string variables
        public void Clear() 
        {
            this.imei = string.Empty;
            this.model = string.Empty;
            this.softwareVersion = string.Empty;
            this.oem = string.Empty;
            this.serialNumber = string.Empty;
            this.status = string.Empty;
            this.capacity = string.Empty;
            this.color = string.Empty;
            //Header.Text = "Connect Device";
            //imeiTextBox.Text = imei;
            //modelTextBox.Text = model;
            //softwareVersionTextBox.Text = softwareVersion;
            //oemTextBox.Text = oem;
            //serialNumberTextBox.Text = serialNumber;
            //statusTextBox.Text = status;
            //capacityTextBox.Text = capacity;
            //colorTextBox.Text = color;
        }
       

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Adds the windows message processing hook and registers USB device add/removal notification.
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null)
            {
                windowHandle = source.Handle;
               source.AddHook(HwndHandler);
               UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }

        public event EventHandler<DeviceChange> DeviceChanged;
       
        /// <summary>
        /// Method that receives window messages.
        /// https://stackoverflow.com/questions/16245706/check-for-device-change-add-remove-events
        /// 
        /// </summary>
        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == UsbNotification.WmDevicechange)
            {

                switch ((int)wparam)
                {
                    // On USB disconnect    
                    case UsbNotification.DbtDeviceremovecomplete:
                        Clear();
                        

                        //OnThresholdReached();
                        break;
                    // On USB connect
                    case UsbNotification.DbtDevicearrival:
                        //this.DeviceChanged?.Invoke(this, DeviceChange.Arrive);
                        string deviceType = ExecuteCommandSync.GetDeviceType();
                        //MessageBox.Show(deviceType);

                        if (deviceType == "Apple")
                        {
                            AppleDevice appleDevice = new AppleDevice();
                            imei = appleDevice.IMEI;
                            model = appleDevice.Model;
                            softwareVersion = appleDevice.SoftwareVersion;
                            oem = appleDevice.OEM;
                            serialNumber = appleDevice.SerialNumber;

                            if (!string.IsNullOrEmpty(imei) && !string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(softwareVersion) && !string.IsNullOrEmpty(oem) && !string.IsNullOrEmpty(serialNumber)) 
                            {
                                //Call to a php endpoint to save in database
                                status = CallToPHP.GetPost(postURL, "save_device_info" , "1" , "imei", imei, "model", model, "software_version" , softwareVersion, "OEM", oem, "serial_number", serialNumber, "workStation", workStation );
                            }
                            
                        }
                        else if (deviceType == "Android")
                        {
                            AndroidDevice androidDevice = new AndroidDevice();
                            
                            imei = androidDevice.IMEI;
                            model = androidDevice.Model;
                            softwareVersion = androidDevice.SoftwareVersion;
                            oem = androidDevice.OEM;
                            serialNumber = androidDevice.SerialNumber;
                            capacity = androidDevice.Capacity;
                            color = androidDevice.Color;

                            if (!string.IsNullOrEmpty(imei) && !string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(softwareVersion) && !string.IsNullOrEmpty(oem) && !string.IsNullOrEmpty(serialNumber) && !string.IsNullOrEmpty(capacity)) 
                            {
                                status = CallToPHP.GetPost(postURL, "save_device_info", "1", "imei", imei, "model", model, "software_version", softwareVersion, "OEM", oem, "serial_number", serialNumber, "workStation", workStation, "capacity", capacity, "color", capacity);
                            }


                            
                        }
                        else
                        {
                            MessageBox.Show("Error Unknown Device, could not get device info");
                        }
                        // Displaying into textboxes
                        //Header.Text = "Device Connected";
                        //imeiTextBox.Text = imei;
                        //modelTextBox.Text = model;
                        //softwareVersionTextBox.Text = softwareVersion;
                        //oemTextBox.Text = oem;
                        //serialNumberTextBox.Text = serialNumber;
                        //statusTextBox.Text = status;
                        //capacityTextBox.Text = capacity;
                        //colorTextBox.Text = color;
                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

       
        



    }
}
