using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class SearchMemberResponseDto
    {
        public int TotalCount { get; set; }
        //public IList<MemberDto> Items { get; set; } = new List<MemberDto>();
        public IList<Contact> Items { get; set; }
    }
}
