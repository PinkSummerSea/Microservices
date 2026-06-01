using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommandService.Dtos
{
    public class CommandCreateDto
    {
        [Required]
        public required string HowTo { get; set; }
        [Required]
        public required string CommandLine { get; set; }
    }
}