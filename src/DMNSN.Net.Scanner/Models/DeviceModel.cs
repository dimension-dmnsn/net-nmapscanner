namespace DMNSN.Net.NmapScanner.Models
{
    public class DeviceModel
    {
        public string IpAddress { get; set; } = string.Empty;
        public string? HostName { get; set; }
        public string? MacAddress { get; set; }
    }
}
