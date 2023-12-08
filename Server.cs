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

        Byte[] bytes = new Byte[1024];
        String data = null;

        while (true)
        {
            using TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");

            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            writer = new StreamWriter(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // // Translate data bytes to a ASCII string.
                // data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                // Console.WriteLine("Received: {0}", data);

                // // Process the data sent by the client.
                // data = data.ToUpper();

                // byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // // Send back a response.
                // stream.Write(msg, 0, msg.Length);
                // Console.WriteLine("Sent: {0}", data);
            }
            Console.WriteLine("Disconnected!");
            writer.Close();
            writer = null;
        }
    }
}