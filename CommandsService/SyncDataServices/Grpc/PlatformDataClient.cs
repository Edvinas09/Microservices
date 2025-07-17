using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var address = _config["GrpcPlatform"];
            Console.WriteLine($"--> Calling GRPC Service to fetch platforms... {address}");

            if (!string.IsNullOrEmpty(address))
            {
                var channel = GrpcChannel.ForAddress(address);
                var client = new GrpcPlatform.GrpcPlatformClient(channel);
                var request = new GetAllRequest();
                try
                {
                    var reply = client.GetAllPlatforms(request);
                    return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not call GRPC Server: {ex.Message}");

                }
            }
            return Enumerable.Empty<Platform>();
        }
    }
}