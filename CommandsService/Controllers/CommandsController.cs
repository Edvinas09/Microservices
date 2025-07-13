using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        //Dependency injection for repository and mapper
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Getting Commands for Platform with ID: {platformId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting Command with ID: {commandId} for Platform with ID: {platformId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var Command = _repo.GetCommand(platformId, commandId);

            if (Command == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(Command));
        }

        [HttpPost]
        public ActionResult<CommandCreateDto> CreateCommandForPlatform(int platformId, CommandCreateDto command)
        {
            Console.WriteLine($"--> Creating Command for Platform with ID: {platformId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commandModel = _mapper.Map<Command>(command);

            _repo.CreateCommand(platformId, commandModel);
            _repo.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtAction(nameof(GetCommandForPlatform), new
            {
                platformId = platformId,
                commandId = commandReadDto.Id
            }, commandReadDto);
        }
    }
}