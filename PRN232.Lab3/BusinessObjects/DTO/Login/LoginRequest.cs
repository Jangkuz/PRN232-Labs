using System.ComponentModel.DataAnnotations;

namespace PRN231_SU25_SE181818.api.DTO.Login;

public class LoginRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
