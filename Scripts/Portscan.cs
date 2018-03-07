using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace dotConsole.Network
{
    class PortScan
    {
        
        public static void Execute(object[] args)
        {
            Console.WriteLine(args[0]);
            Console.WriteLine("Port Scanner 1.0");
            string IpAddress = args[0].ToString();

            System.Threading.Tasks.Parallel.For(1, 30, x => {
                TcpClient TcpScan = new TcpClient();
                try
                {
                    TcpScan.Connect(IpAddress, x);
                    Console.Write("\r Port " + x + " open   \n");
                }

                catch
                {
                    // An exception occured, thus the port is probably closed 	
                    Console.Write("\r Scanning Port " + x + " ");
                }
            });

            /*for (int CurrPort = 69; CurrPort <= 85; CurrPort++)
            {
                TcpClient TcpScan = new TcpClient();
                try
                {
                    TcpScan.Connect(IpAddress, CurrPort);
                    Console.Write("\r Port " + CurrPort + " open\n");
                }

                catch
                {
                    // An exception occured, thus the port is probably closed 	
                    Console.Write("\r Port " + CurrPort + " closed"); 
                }

            }*/
            Console.WriteLine("Portscanner completed..");
            Console.ReadLine();
        }
    }
}





