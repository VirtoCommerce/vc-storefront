namespace VirtoCommerce.Storefront.Model.Order
{
    using System.ComponentModel.DataAnnotations;

    public class OrderStatusChange
    {
        [Required]
        public string NewStatus { get; set; }
    }
}
