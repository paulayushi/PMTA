﻿using PMTA.Core.Command;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class UserLoginCommand: BaseCommand
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
