using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class MemberIdIdentityResult : IdentityResult
    {
        public string MemberId { get; set; }
        public new IEnumerable<IdentityError> Errors { get; protected set; }

        public static MemberIdIdentityResult Instance(IdentityResult identityResult)
        {
            var result = new MemberIdIdentityResult {
                Succeeded = identityResult.Succeeded,
                Errors = identityResult.Errors.ToList()
            };

            return result;
        }
    }
}
