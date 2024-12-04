using DMNSN.Net.NmapScanner.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.NetworkInformation;

namespace DMNSN.Net.NmapScanner.xUnit
{
    public class UnitTests
    {
        [Fact]
        public void TestIsNmapInstalled()
        {
            var mock = new Mock<ILogger<NmapScanner>>();
            ILogger<NmapScanner> logger = mock.Object;
            // Arrange
            var scanner = new NmapScanner(logger);

            // Act
            bool isInstalled = scanner.IsNmapInstalled();

            // Assert
            // Note: The expected result depends on whether nmap is installed on the system where the test is run.
            // For demonstration purposes, we assume nmap is installed.
            Assert.True(isInstalled, "Nmap should be installed on the system.");
        }

        [Fact]
        public void TestGetOnlineDevices()
        {
            var mock = new Mock<ILogger<NmapScanner>>();
            ILogger<NmapScanner> logger = mock.Object;
            // Arrange
            var scanner = new NmapScanner(logger);

            // Get the local IP address and subnet
            var localIP = GetLocalIPAddress();
            var networkAddress = GetNetworkAddress(localIP.Address, localIP.IPv4Mask);
            var netMask = GetSubnetMaskLength(localIP.IPv4Mask);
            var subnetModel = new SubnetModel(networkAddress, netMask);

            // Act
            var devices = scanner.GetOnlineDevices(subnetModel);

            // Assert
            Assert.NotNull(devices);
            Assert.True(devices.Count > 0, "There should be at least one device online in the subnet.");
        }

        private UnicastIPAddressInformation GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip;
                        }
                    }
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        private string GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] baseAddressBytes = new byte[ipBytes.Length];

            for (int i = 0; i < ipBytes.Length; i++)
            {
                baseAddressBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(baseAddressBytes).ToString();
        }
        private int GetSubnetMaskLength(IPAddress subnetMask)
        {
            byte[] maskBytes = subnetMask.GetAddressBytes();
            int length = 0;

            foreach (byte b in maskBytes)
            {
                length += CountBits(b);
            }

            return length;
        }

        private int CountBits(byte b)
        {
            int count = 0;
            while (b != 0)
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }
    }
}