namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class ContactResponseDto
    {
        public ContactDto Contact { get; set; }
    }

    public class ContactDto : Contact
    {
        public Organization[] Organizations { get; set; }
    }
}
