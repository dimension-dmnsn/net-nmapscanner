namespace DMNSN.Net.NmapScanner.Models
{
    public class DeviceEntityModel
    {
        public string IpAddress { get; set; } = string.Empty;
        public string? HostName { get; set; }
        public string MacAddress { get; set; } = string.Empty;
    }
}
