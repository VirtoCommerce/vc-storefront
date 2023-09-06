using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model;

public class SlugInfoRequest
{
    public string CultureName { get; set; }

    [Required]
    public string Slug { get; set; }
}
