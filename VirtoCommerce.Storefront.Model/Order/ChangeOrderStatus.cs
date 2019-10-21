namespace VirtoCommerce.Storefront.Model.Order
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeOrderStatus
    {
        [Required]
        public string NewStatus { get; set; }
    }
}
