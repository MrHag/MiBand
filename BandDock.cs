using System.ComponentModel;
using System.Text.Json;

enum Types{
    MESSAGE,
    HEARTRATE
}

class Packet
{
    public Types type {get; set;}
    public string value {get; set;}

    public Packet(Types type, object value){
        this.type = type;
        this.value = value.ToString();
    }

}

namespace MiBand_Heartrate_2
{
    public class BandDock
    {
        Devices.Device _device = null;

        public Devices.Device Device
        {
            get { return _device; }
            set
            {
                if (_device != null)
                {
                    _device.PropertyChanged -= OnDevicePropertyChanged;
                    _device.Dispose();
                }

                _device = value;

                if (_device != null)
                {
                    _device.PropertyChanged += OnDevicePropertyChanged;
                }

                DeviceUpdate();

                //InvokePropertyChanged("Device");
            }
        }

        bool _isConnected = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                //InvokePropertyChanged("IsConnected");
            }
        }

        string _statusText = "No device connected";

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                StartPr.Log(_statusText);
                //InvokePropertyChanged("StatusText");
            }
        }

        bool _continuousMode = true;

        public bool ContinuousMode
        {
            get { return _continuousMode; }
            set
            {
                _continuousMode = value;

                //Setting.Set("ContinuousMode", _continuousMode);

                //InvokePropertyChanged("ContinuousMode");
            }
        }

        bool _enableFileOutput = false;

        public bool EnableFileOutput
        {
            get { return _enableFileOutput; }
            set
            {
                _enableFileOutput = value;

                //Setting.Set("FileOutput", _enableFileOutput);

                //InvokePropertyChanged("EnableFileOutput");
            }
        }

        bool _enableCSVOutput = false;

        public bool EnableCSVOutput
        {
            get { return _enableCSVOutput; }
            set
            {
                _enableCSVOutput = value;

                //Setting.Set("CSVOutput", _enableCSVOutput);

                //InvokePropertyChanged("EnableCSVOutput");
            }
        }

        bool _guard = false;

        // DeviceHeartrateFileOutput _fileOutput = null;

        // DeviceHeartrateCSVOutput _csvOutput = null;

        // --------------------------------------

        public BandDock()
        {
            // ContinuousMode = Setting.Get("ContinuousMode", true);
            // EnableFileOutput = Setting.Get("FileOutput", false);
            // EnableCSVOutput = Setting.Get("CSVOutput", false);
        }

        ~BandDock()
        {
            Device = null;
        }

        void UpdateStatusText()
        {
            if (Device != null)
            {
                switch (Device.Status)
                {
                    case Devices.DeviceStatus.OFFLINE:
                        StatusText = "No device connected";
                        break;
                    case Devices.DeviceStatus.ONLINE_UNAUTH:
                        StatusText = string.Format("Connected to {0} | Not auth", Device.Name);
                        break;
                    case Devices.DeviceStatus.ONLINE_AUTH:
                        StatusText = string.Format("Connected to {0} | Auth", Device.Name);
                        break;
                }
            }
            else
            {
                StatusText = "No device connected";
            }
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                // System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                // {
                //     DeviceUpdate();
                // });

                DeviceUpdate();

                if (Device != null)
                {
                    // Connection lost, we try to re-connect
                    if (Device.Status == Devices.DeviceStatus.OFFLINE && _guard)
                    {
                        _guard = false;
                        Device.Connect();
                    }
                    else if (Device.Status != Devices.DeviceStatus.OFFLINE)
                    {
                        _guard = true;
                    }
                }
            }
            else if (e.PropertyName == "HeartrateMonitorStarted")
            {
                //CommandManager.InvalidateRequerySuggested();
                 StartPr.Log($"Heartrate started");
            }
            else if (e.PropertyName == "Heartrate")
            {
                //StartPr.Log($"Heartrate: {Device.Heartrate}");
                var json = JsonSerializer.Serialize(new Packet(Types.HEARTRATE, Device.Heartrate));
                Console.WriteLine(json);
            }
        }

        private void DeviceUpdate()
        {
            if (Device != null)
            {
                IsConnected = Device.Status != Devices.DeviceStatus.OFFLINE;
            }

            UpdateStatusText();
            //CommandManager.InvalidateRequerySuggested();
        }

        // --------------------------------------

        public void Connect()
        {
            Device.Connect();
        }

        public void Disconnect()
        {
            Device.Disconnect();
        }

        public void Start()
        {
            Device.StartHeartrateMonitor(ContinuousMode);
        }

        public void Stop()
        {
            Device.StopHeartrateMonitor();
        }
    }
}