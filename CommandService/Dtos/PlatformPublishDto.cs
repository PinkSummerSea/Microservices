using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandService.Dtos
{
    public class PlatformPublishDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Event { get; set; }
    }
}