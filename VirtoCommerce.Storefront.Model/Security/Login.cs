using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security;

public partial class Login
{
    [EmailAddress]
    public string Email { get; set; }

    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class AuthResponseDto
{
    public bool IsAuthSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
}
