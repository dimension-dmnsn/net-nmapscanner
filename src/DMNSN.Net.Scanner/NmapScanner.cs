using DMNSN.Net.NmapScanner.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DMNSN.Net.NmapScanner
{
    public partial class NmapScanner
    {
        [GeneratedRegex(@"Nmap scan report for (?:(?<hostname>[^\s]+) \()?(?<ip>\d+\.\d+\.\d+\.\d+)\)?\r?\n.*\r?\n(?:.*MAC Address: (?<mac>[0-9A-Fa-f:]+))?", RegexOptions.Multiline)]
        private static partial Regex NmapRegex();

        private readonly Process namp;
        private readonly ILogger<NmapScanner> logger;

        public NmapScanner(ILogger<NmapScanner> _logger)
        {
            logger = _logger;
            logger.LogDebug("Initalizing NmapScanner");
            namp = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "nmap",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            if (!IsNmapInstalled())
            {
                logger.LogError("Nmap is not installed");
                throw new Exception("Nmap is not installed");
            }
            logger.LogDebug("NmapScanner initialized");
        }

        public List<DeviceEntityModel> GetOnlineDevices(SubnetModel subnet)
        {
            var onlineDevices = new List<DeviceEntityModel>();
            namp.StartInfo.Arguments = $"-sn {subnet.GetSubnetAddress()}";
            namp.Start();
            var output = namp.StandardOutput.ReadToEnd();
            namp.WaitForExit();

            var regex = NmapRegex();
            var matches = regex.Matches(output);

            foreach (Match match in matches)
            {
                var device = new DeviceEntityModel()
                {
                    IpAddress = match.Groups["ip"].Value,
                    MacAddress = match.Groups["mac"].Success ? match.Groups["mac"].Value : null,
                    HostName = match.Groups["hostname"].Success ? match.Groups["hostname"].Value : null
                };
                onlineDevices.Add(device);
            }

            return onlineDevices;
        }

        public List<DeviceEntityModel> GetOnlineDevices(List<SubnetModel> subnets)
        {
            var onlineDevices = new List<DeviceEntityModel>();
            foreach (var subnet in subnets)
            {
                onlineDevices.AddRange(GetOnlineDevices(subnet));
            }
            return onlineDevices;
        }

        public bool IsNmapInstalled()
        {
            try
            {
                namp.StartInfo.Arguments = "--version";
                namp.Start();
                namp.WaitForExit();
                return namp.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
