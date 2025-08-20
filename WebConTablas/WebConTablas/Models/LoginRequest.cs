using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class LoginRequest
    {
    public required string Nombre { get; set; }
    public required string Email { get; set; }
    }
}