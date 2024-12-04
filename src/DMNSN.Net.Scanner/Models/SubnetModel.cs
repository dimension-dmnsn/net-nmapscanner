using System.Net;

namespace DMNSN.Net.NmapScanner.Models
{
    public class SubnetModel
    {
        private string BaseAddress { get; set; } = string.Empty;
        private int NetMask { get; set; } = 24;

        public SubnetModel(string baseAddress, int netMask = 24)
        {
            BaseAddress = baseAddress;
            NetMask = netMask;
            if (netMask < 0 || netMask > 32)
            {
                throw new System.Exception("Invalid netmask");
            }
            if (!ValidateBaseAddress())
            {
                throw new Exception("Invalid base address");
            }
        }

        public string GetSubnetAddress()
        {
            return $"{BaseAddress}/{NetMask}";
        }

        public string GetStartAddress()
        {
            var baseAddressBytes = IPAddress.Parse(BaseAddress).GetAddressBytes();
            var maskBytes = GetMaskBytes(NetMask);
            var startAddressBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                startAddressBytes[i] = (byte)(baseAddressBytes[i] & maskBytes[i]);
            }

            startAddressBytes[3] += 1; // Start address is the first address in the subnet

            return new IPAddress(startAddressBytes).ToString();
        }

        public string GetEndAddress()
        {
            var baseAddressBytes = IPAddress.Parse(BaseAddress).GetAddressBytes();
            var maskBytes = GetMaskBytes(NetMask);
            var endAddressBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                endAddressBytes[i] = (byte)(baseAddressBytes[i] | ~maskBytes[i]);
            }

            endAddressBytes[3] -= 1; // End address is the last address in the subnet

            return new IPAddress(endAddressBytes).ToString();
        }

        private bool ValidateBaseAddress()
        {
            return IPAddress.TryParse(BaseAddress, out _);
        }

        private byte[] GetMaskBytes(int netMask)
        {
            var maskBytes = new byte[4];
            for (int i = 0; i < netMask; i++)
            {
                maskBytes[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }
            return maskBytes;
        }
    }
}
