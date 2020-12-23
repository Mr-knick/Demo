using System;

using System.Net;

using System.Net.Sockets;

using System.Text;

 

namespace MultiCast

{

    class Program

    {

        static void Main(string[] args)

        {

 

            UdpClient udpclient = new UdpClient();

 

            IPAddress multicastaddress = IPAddress.Parse("224.1.1.1");

            udpclient.JoinMulticastGroup(multicastaddress);

            IPEndPoint remoteep = new IPEndPoint(multicastaddress, 1234);

 

            Byte[] buffer = null;

 

            Console.WriteLine("Press ENTER to start sending messages");

            Console.ReadLine();

 

            for (int i = 0; i <= 8000; i++)

            {

                buffer = Encoding.Unicode.GetBytes(i.ToString());

                udpclient.Send(buffer, buffer.Length, remoteep);

                Console.WriteLine("Sent " + i);

            }

 

            Console.WriteLine("All Done! Press ENTER to quit.");

            Console.ReadLine();

        }

    }

}