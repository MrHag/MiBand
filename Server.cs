using System.Net;
using System.Net.Sockets;
using System.Text;

class MiBandServer
{
    TcpListener server;

    public MiBandServer()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 58689);
    }

    StreamWriter writer = null;

    public void SendLine(string strLine)
    {
        if (writer != null)
            writer.WriteLine(strLine);
    }

    public void StartListening()
    {
        server.Start();

        Console.WriteLine("Listenning stated");

        byte[] bytes = new byte[1024];

        while (true)
        {
            using TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            writer = new StreamWriter(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            int i;
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            { }
            Console.WriteLine("Disconnected!");
            writer.Close();
            writer = null;
        }
    }
}