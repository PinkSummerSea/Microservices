using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        // platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExists(int PlatformId);

        // commands
        IEnumerable<Command> GetCommandsForPlatform(int PlatformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);



    }
}