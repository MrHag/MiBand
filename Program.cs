using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using Windows.Devices.Enumeration;

namespace MiBand_Heartrate_2
{

    public class StartPr
    {
        public delegate void Notif(string val);
        static public event Notif ondata;

        static public event Notif onsend;

        static Connection connection = new Connection();

        static Devices.Device? device = null;

        static public Devices.Device? SelectedDevice
        {
            get { return device; }
            set
            {
                device = value;
                Start();
            }
        }

        static public void Log(string val)
        {
            ondata?.Invoke(val);
        }

        static public void Send(string json)
        {
            onsend?.Invoke(json);
        }

        static public void Main()
        {

            MiBandServer server = new MiBandServer();

            var serverThread = new Thread(new ThreadStart(server.StartListening));

            serverThread.Start();

            Console.WriteLine("Listenning on 127.0.0.1:58689");

            ondata += (data) =>
            {
                var json = JsonSerializer.Serialize(new Packet(Types.MESSAGE, data));
                Console.WriteLine(json);
                server.SendLine(json);
            };

            onsend += (json) =>
            {
                Console.WriteLine(json);
                server.SendLine(json);
            };

            ondata?.Invoke("STARTING");

            connection.Devices.CollectionChanged += (s, e) => { Write(s, e, connection.Devices); };
            connection.Devices.CollectionChanged += TryConnect;

            connection.Start();

            // var device = connection.Connect();

            // device.PropertyChanged += (_,_)=>{ondata?.Invoke(device.Heartrate.ToString());};

            // device.StartHeartrateMonitor(true);


            // BandDock dock = new BandDock();

            // dock.

            // dock.Connect();

            // BLE bluetooth = new BLE(new string[] {"System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected"});

            // bluetooth.

            Console.ReadLine();
        }

        static void Write(object? sender, NotifyCollectionChangedEventArgs e, ObservableCollection<DeviceInformation> coll)
        {
            foreach (var item in coll)
            {
                //StartPr.Log($"{item.Name} : {item.Id}");
                ondata?.Invoke($"{item.Name} : {item.Id}");
            }
        }

        static void TryConnect(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                DeviceInformation dev = (DeviceInformation)e.NewItems[0];
                ondata?.Invoke(dev.Name);
                connection.SelectedDevice = dev;
                Devices.Device devi = connection.Connect();
                ondata?.Invoke(devi.Name);
                connection.Devices.CollectionChanged -= TryConnect;
                SelectedDevice = devi;
            }
        }


        static void Start()
        {
            BandDock band = new BandDock();
            band.Device = SelectedDevice;

            band.Start();
        }
    }
}