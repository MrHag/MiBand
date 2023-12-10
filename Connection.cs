using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;

using MiBand_Heartrate_2.Devices;

namespace MiBand_Heartrate_2
{
    public class Connection
    {
        ObservableCollection<DeviceInformation> _devices = new ObservableCollection<DeviceInformation>();

        public ObservableCollection<DeviceInformation> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                //InvokePropertyChanged("Devices");
            }
        }

        DeviceModel _deviceModel = DeviceModel.MIBAND_4;

        public DeviceModel DeviceModel
        {
            get { return _deviceModel; }
            set
            {
                _deviceModel = value;
                //InvokePropertyChanged("DeviceModel");
            }
        }


        BLE _bluetooth;

        // --------------------------------------

        public Connection()
        {
            _bluetooth = new BLE(new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" });

            _bluetooth.Watcher.Added += OnBluetoothAdded;
            _bluetooth.Watcher.Updated += OnBluetoothUpdated;
            _bluetooth.Watcher.Removed += OnBluetoothRemoved;

        }

        public void Start()
        {
            _bluetooth.StartWatcher();
        }

        ~Connection()
        {
            if (_bluetooth.Watcher != null)
            {
                _bluetooth.Watcher.Added -= OnBluetoothAdded;
                _bluetooth.Watcher.Updated -= OnBluetoothUpdated;
                _bluetooth.Watcher.Removed -= OnBluetoothRemoved;
            }

            _bluetooth.StopWatcher();
        }

        private void OnBluetoothRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            DeviceInformation[] pending = Devices.Where(x => x.Id == args.Id).ToArray();
            for (int i = 0; i < pending.Length; ++i)
            {
                Devices.Remove(pending[i]);
            }
        }

        private void OnBluetoothUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            foreach (DeviceInformation d in Devices)
            {
                if (d.Id == args.Id)
                {
                    d.Update(args);
                    break;
                }
            }
        }

        private void OnBluetoothAdded(DeviceWatcher sender, DeviceInformation args)
        {
            Devices.Add(args);
        }

        // --------------------------------------

    }
}