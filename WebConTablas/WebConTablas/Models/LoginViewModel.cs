using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    public string Nombre { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
