namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class OrganizationDto : MemberDto
    {
        public string Description { get; set; }
        public string BusinessCategory { get; set; }
        public string OwnerId { get; set; }
        public string ParentId { get; set; }
        public string ObjectType { get; }
    }
}
