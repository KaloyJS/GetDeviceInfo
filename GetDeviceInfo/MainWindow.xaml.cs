using GetDeviceInfo;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Interop;
using LibUsbDotNet.DeviceNotify;
using USBClassLibrary;

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
        #region Declarations

        //declaring an instance of USBClass
        private USBClassLibrary.USBClass USBPort;

        //an instance List<T> of DeviceProperties if you want to read the properties of your devices
        List<USBClassLibrary.USBClass.DeviceProperties> ListOfUSBDeviceProperties;

        public static IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

        private string DeviceVID;
        private string DevicePID;        
        private Hashtable PortStatus  = new Hashtable();
        private Hashtable CurrentPortStatus = new Hashtable();

        // Instantiate 3 devices object that represents the 3 device to be processed
        private Device Device_1 = new Device();
        private Device Device_2 = new Device();
        private Device Device_3 = new Device();

        // count the number of connected device
        private int NumberOfConnectedDevice;
        private static List<string> connectedDevicesUDID = new List<string>();

        public string workStation = System.Environment.MachineName;
        private string postURL = "https://portal-ca.sbe-ltd.ca/screening_new/device_info/actions.php";

        #endregion

        #region main window

        public MainWindow()
        {          
            InitializeComponent();
            //instance of the USBClass class.
            USBPort = new USBClass();
            ListOfUSBDeviceProperties = new List<USBClassLibrary.USBClass.DeviceProperties>();
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
            // Initializes Usb Ports statuses
            
            HelperFunctions.initializePortStatus(PortStatus);
            HelperFunctions.initializePortStatus(CurrentPortStatus);

            
        }

        public event EventHandler<DeviceChange> DeviceChanged;

        #endregion



        #region Getting the usb port function

        /// <summary>
        /// Returns the Port location of the device connected
        /// </summary>
        private void GetCurrentPortStatus()
        {
            

            Nullable<UInt32> MI = null;
            bool bGetSerialPort = false;          
            

            if (USBClass.GetUSBDevice(uint.Parse(DeviceVID, System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(DevicePID, System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, bGetSerialPort, MI))
            {
                
                // Creates an array of Connected Devices 
                List<string> connectedDevices = new List<string>();
                for (int i = 0; i < ListOfUSBDeviceProperties.Count; i++)
                {
                    string Manufacturer = ListOfUSBDeviceProperties[i].DeviceManufacturer;
                    string currentPort = ListOfUSBDeviceProperties[i].DeviceLocation.Substring(ListOfUSBDeviceProperties[0].DeviceLocation.IndexOf('#') + 4, 1);
                    //MessageBox.Show(currentPort);
                    if (int.Parse(currentPort) <= 3)
                    {
                        connectedDevices.Add(currentPort.ToString());
                        if (currentPort.ToString() == "1")
                        {
                            Device_1.Manufacturer = Manufacturer;
                            
                        }
                        else if (currentPort.ToString() == "2")
                        {
                            Device_2.Manufacturer = Manufacturer;
                        }
                        else if (currentPort.ToString() == "3")
                        {
                            Device_3.Manufacturer = Manufacturer;
                        }

                    }
                    

                }
                //string toDisplay = string.Join(Environment.NewLine, connectedDevices);
                //MessageBox.Show(toDisplay);
                HelperFunctions.resetPortStatus(CurrentPortStatus);
                foreach (string device in connectedDevices)
                {
                    CurrentPortStatus[device] = "Connected";
                }


            }
        }

        /// <summary>
        /// Gets the disconnected/Connected Port of USB devices by comparing current port statuses and last port statuses
        /// </summary>
        /// <param name="CurrentPortStatus"></param>
        /// <param name="PortStatus"></param>
        /// <returns></returns>

        public static string GetPort(Hashtable CurrentPortStatus, Hashtable PortStatus)
        {
            string port = "";

            foreach (DictionaryEntry de in PortStatus)
            {
                if (PortStatus[de.Key] != CurrentPortStatus[de.Key] )
                {
                    port = de.Key.ToString();
                    break;
                }
            }

            PortStatus[port] = CurrentPortStatus[port];

            return port;
        }

        #endregion  
           


        #region On connect/disconnect event handler

        /// <summary>
        /// Event handler for any connect and disconnect of USB Devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            // On USB connect
            if (e.EventType.ToString() == "DeviceArrival")
            {

                //Error Check
                if (e.Device != null)
                {
                    // sets Vid and Pid of connected device
                    DeviceVID = e.Device.IdVendor.ToString("X4");
                    DevicePID = e.Device.IdProduct.ToString("X4");
                    GetCurrentPortStatus();                
                    string ConnectedPort = GetPort(CurrentPortStatus, PortStatus);
                    if (!string.IsNullOrEmpty(ConnectedPort))
                    {
                        NumberOfConnectedDevice++;

                        // Get Device Information and Assign to each port
                        
                        if (ConnectedPort == "1")
                        {
                            Header_1.Text = "Device Connected";                            
                            Device_1.UDID = AppleCommands.GetUdid(NumberOfConnectedDevice, connectedDevicesUDID);
                            // Check if UDID is valid  
                            if (HelperFunctions.checkUDID(Device_1.UDID))
                            {                                
                                // Adding Device UDID to array
                                connectedDevicesUDID.Add(Device_1.UDID);

                                Device_1.IMEI = AppleCommands.GetIMEI(Device_1.UDID);
                                Device_1.SerialNumber = AppleCommands.GetSerialNumber(Device_1.UDID);
                                Device_1.ModelNumber = AppleCommands.GetModelNumber(Device_1.UDID);
                                Device_1.ModelName = AppleCommands.GetModelName(Device_1.ModelNumber);
                                Device_1.SoftwareVersion = AppleCommands.GetSoftwareVersion(Device_1.UDID);
                                Device_1.CodePro = AppleCommands.GetCodePro(Device_1.ModelNumber);
                                Device_1.Color = AppleCommands.GetColor(Device_1.ModelNumber);
                                Device_1.Capacity = AppleCommands.GetCapacity(Device_1.ModelNumber);

                                OemTextBox_1.Text = Device_1.Manufacturer;
                                UdidTextBox_1.Text = Device_1.UDID;
                                ImeiTextBox_1.Text = Device_1.IMEI;
                                SerialNumberTextBox_1.Text = Device_1.SerialNumber;
                                ModelTextBox_1.Text = Device_1.ModelName;
                                SoftwareVersionTextBox_1.Text = Device_1.SoftwareVersion;
                                CodeProTextBox_1.Text = Device_1.CodePro;
                                ColorTextBox_1.Text = Device_1.Color;
                                CapacityTextBox_1.Text = Device_1.Capacity;

                            }
                            else
                            {
                                MessageBox.Show("UDID error disconnect devices and connect again");
                                ClearFields(ConnectedPort);
                            }                       
                            
                            
                        }
                        else if (ConnectedPort == "2")
                        {
                            Header_2.Text = "Device Connected";
                            Device_2.UDID = AppleCommands.GetUdid(NumberOfConnectedDevice, connectedDevicesUDID);
                            // Check if UDID is valid  
                            if (HelperFunctions.checkUDID(Device_2.UDID))
                            {
                                // Adding Device UDID to array
                                connectedDevicesUDID.Add(Device_2.UDID);

                                Device_2.IMEI = AppleCommands.GetIMEI(Device_2.UDID);
                                Device_2.SerialNumber = AppleCommands.GetSerialNumber(Device_2.UDID);
                                Device_2.ModelNumber = AppleCommands.GetModelNumber(Device_2.UDID);
                                Device_2.SoftwareVersion = AppleCommands.GetSoftwareVersion(Device_2.UDID);
                                Device_2.CodePro = AppleCommands.GetCodePro(Device_2.ModelNumber);
                                Device_2.ModelName = AppleCommands.GetModelName(Device_2.ModelNumber);
                                Device_2.Color = AppleCommands.GetColor(Device_2.ModelNumber);
                                Device_2.Capacity = AppleCommands.GetCapacity(Device_2.ModelNumber);

                                OemTextBox_2.Text = Device_2.Manufacturer;
                                UdidTextBox_2.Text = Device_2.UDID;
                                ImeiTextBox_2.Text = Device_2.IMEI;
                                SerialNumberTextBox_2.Text = Device_2.SerialNumber;
                                ModelTextBox_2.Text = Device_2.ModelName;
                                SoftwareVersionTextBox_2.Text = Device_2.SoftwareVersion;
                                CodeProTextBox_2.Text = Device_2.CodePro;
                                ColorTextBox_2.Text = Device_2.Color;
                                CapacityTextBox_2.Text = Device_2.Capacity;

                            }
                            else
                            {
                                MessageBox.Show("UDID error disconnect devices and connect again");
                                ClearFields(ConnectedPort);
                            }
                        }
                        else if (ConnectedPort == "3")
                        {
                            Header_3.Text = "Device Connected";
                            Device_3.UDID = AppleCommands.GetUdid(NumberOfConnectedDevice, connectedDevicesUDID);
                            // Check if UDID is valid  
                            if (HelperFunctions.checkUDID(Device_3.UDID))
                            {
                                // Adding Device UDID to array
                                connectedDevicesUDID.Add(Device_3.UDID);

                                Device_3.IMEI = AppleCommands.GetIMEI(Device_3.UDID);
                                Device_3.SerialNumber = AppleCommands.GetSerialNumber(Device_3.UDID);
                                Device_3.ModelNumber = AppleCommands.GetModelNumber(Device_3.UDID);
                                Device_3.SoftwareVersion = AppleCommands.GetSoftwareVersion(Device_3.UDID);
                                Device_3.CodePro = AppleCommands.GetCodePro(Device_3.ModelNumber);
                                Device_3.ModelName = AppleCommands.GetModelName(Device_3.ModelNumber);
                                Device_3.Color = AppleCommands.GetColor(Device_3.ModelNumber);
                                Device_3.Capacity = AppleCommands.GetCapacity(Device_3.ModelNumber);

                                OemTextBox_3.Text = Device_3.Manufacturer;
                                UdidTextBox_3.Text = Device_3.UDID;
                                ImeiTextBox_3.Text = Device_3.IMEI;
                                SerialNumberTextBox_3.Text = Device_3.SerialNumber;
                                ModelTextBox_3.Text = Device_3.ModelName;
                                SoftwareVersionTextBox_3.Text = Device_3.SoftwareVersion;
                                CodeProTextBox_3.Text = Device_3.CodePro;
                                ColorTextBox_3.Text = Device_3.Color;
                                CapacityTextBox_3.Text = Device_3.Capacity;
                            }
                            else
                            {
                                MessageBox.Show("UDID error disconnect devices and connect again");
                                ClearFields(ConnectedPort);
                            }
                        }
                    }


                }

            }
            // On disconnect
            else
            {
                if (!string.IsNullOrEmpty(DeviceVID) && !string.IsNullOrEmpty(DevicePID))
                {
                    HelperFunctions.resetPortStatus(CurrentPortStatus);
                    GetCurrentPortStatus();                 
                    string DisconnectedPort = GetPort(CurrentPortStatus, PortStatus);
                    if (!string.IsNullOrEmpty(DisconnectedPort))
                    {
                        NumberOfConnectedDevice--;
                        ClearFields(DisconnectedPort);
                    }



                }
            }

        }

        #endregion

        #region clear fields function

        /// <summary>
        /// Clear fields by port
        /// </summary>
        public void ClearFields(string port)
        {
            switch (port)
            {
                case "1":

                    connectedDevicesUDID.Remove(Device_1.UDID);
                    Device_1.Reset();
                    Header_1.Text = "Connect Device";
                    UdidTextBox_1.Text = "";
                    OemTextBox_1.Text = "";
                    ImeiTextBox_1.Text = "";
                    ModelTextBox_1.Text = "";
                    SerialNumberTextBox_1.Text = "";
                    SoftwareVersionTextBox_1.Text = "";
                    CapacityTextBox_1.Text = "";
                    ColorTextBox_1.Text = "";
                    StatusTextBox_1.Text = "";
                    CodeProTextBox_1.Text = "";
                    break;

                case "2":
                    connectedDevicesUDID.Remove(Device_2.UDID);
                    Device_1.Reset();
                    Header_2.Text = "Connect Device";
                    UdidTextBox_2.Text = "";
                    OemTextBox_2.Text = "";
                    ImeiTextBox_2.Text = "";
                    ModelTextBox_2.Text = "";
                    SerialNumberTextBox_2.Text = "";
                    SoftwareVersionTextBox_2.Text = "";
                    CapacityTextBox_2.Text = "";
                    ColorTextBox_2.Text = "";
                    StatusTextBox_2.Text = "";
                    CodeProTextBox_2.Text = "";
                    break;

                case "3":
                    connectedDevicesUDID.Remove(Device_3.UDID);
                    Device_1.Reset();
                    Header_3.Text = "Connect Device";
                    UdidTextBox_3.Text = "";
                    OemTextBox_3.Text = "";
                    ImeiTextBox_3.Text = "";
                    ModelTextBox_3.Text = "";
                    SerialNumberTextBox_3.Text = "";
                    SoftwareVersionTextBox_3.Text = "";
                    CapacityTextBox_3.Text = "";
                    ColorTextBox_3.Text = "";
                    StatusTextBox_3.Text = "";
                    CodeProTextBox_3.Text = "";
                    break;


            }
        }

        #endregion

        #region Get Properties and Display

        public void GetPropertiesAndDisplay(string port) { 

        }

        #endregion

    }
}
