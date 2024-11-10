using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_cSharp.DTOs
{
    public partial class UserForLoginDto
    {
        public string Email {get; set;} = "";
        public string Password {get; set;} = "";
    }
}