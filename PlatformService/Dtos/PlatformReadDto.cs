namespace PlatformService.Dtos
{
    public class PlatformReadDto
    {
        // This DTO is used to transfer data from the API to the client
        // It does not contain any navigation properties or complex types
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Cost { get; set; } = string.Empty;

    }
}