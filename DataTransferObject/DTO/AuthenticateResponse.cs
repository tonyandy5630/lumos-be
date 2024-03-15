using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class AuthenticateResponse
    {
        public bool Authenticated { get; set; } = false;
        public string? Role {  get; set; }
        public object? UserDetails { get; set; }
        public bool emailExists { get; set; } = false;
        public bool passwordCorrect { get; set; } = false;
        public bool isBanned {  get; set; } = false;
    }
}
