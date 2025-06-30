using System;

namespace PRN231_SU25_SE181818.api.DTO.Login;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
