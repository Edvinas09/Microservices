namespace PlatformService.Dtos
{
    public class PlatformPublishedDto
    {
        // This DTO is used to transfer data when a platform is published
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
    }
}