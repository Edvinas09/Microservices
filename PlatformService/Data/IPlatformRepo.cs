using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        //CRUD operations
        // SaveChanges method to commit changes to the database
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        Platform? GetPlatformById(int id);
        void CreatePlatform(Platform platform);
    }
}