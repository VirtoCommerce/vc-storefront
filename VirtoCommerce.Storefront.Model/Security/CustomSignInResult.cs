namespace VirtoCommerce.Storefront.Model.Security
{
    public class CustomSignInResult : Microsoft.AspNetCore.Identity.SignInResult
    {
        public bool IsRejected { get; protected set; }

        public static CustomSignInResult Rejected => new CustomSignInResult { IsRejected = true };
    }
}
