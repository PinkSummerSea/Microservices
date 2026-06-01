using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreateCommand(int platformId, Command command)
        {
            ArgumentNullException.ThrowIfNull(command);
            command.PlatformId=platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform plat)
        {
            ArgumentNullException.ThrowIfNull(plat);
            _context.Platforms.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _context.Commands.FirstOrDefault(c => c.PlatformId== platformId && c.Id==commandId)
                ??throw new Exception("no command found");
        }

        public IEnumerable<Command> GetCommandsForPlatform(int PlatformId)
        {
            return _context.Commands
                .Where(c => c.PlatformId==PlatformId)
                .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int PlatformId)
        {
            return _context.Platforms.Any(p => p.Id == PlatformId);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}