namespace VirtoCommerce.Storefront.Model.Security
{
    public class SignInResult: Microsoft.AspNetCore.Identity.SignInResult
    {
        public bool IsRejected { get; protected set; }

        public static SignInResult Rejected => new SignInResult { IsRejected = true };
    }
}
