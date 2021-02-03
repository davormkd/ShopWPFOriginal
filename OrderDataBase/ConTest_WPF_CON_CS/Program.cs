using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ConTest_WPF_CON_CS
{
    public static class Program
    {
        public static void DisplayGatewayAddresses()
        {
            Console.WriteLine("Gateways");
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                if (addresses.Count > 0)
                {
                    Console.WriteLine(adapter.Description);
                    foreach (GatewayIPAddressInformation address in addresses)
                    {
                        Console.WriteLine("  Gateway Address ......................... : {0}",
                            address.Address.ToString());
                    }
                    Console.WriteLine();
                }
            }
        }



        public static void Main(string[] args)
        {
            Console.WriteLine("Hello");

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            for (int i = 0; i < localIPs.Length; i++)
            {
                Console.WriteLine("No.: " + i + "  :" +  localIPs[i]);
            }

            Console.WriteLine("Hello\n\n");
            DisplayGatewayAddresses();
            Console.ReadLine();
        }
    }
}
