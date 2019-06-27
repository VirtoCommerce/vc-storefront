using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public static partial class SecurityErrorDescriber
    {
        public static FormError LoginFailed()
        {
            return new FormError
            {
                Code = nameof(LoginFailed).PascalToKebabCase(),
                Description = "Login attempt failed"
            };
        }

        public static FormError UserNotFound()
        {
            return new FormError
            {
                Code = nameof(UserNotFound).PascalToKebabCase(),
                Description = "User not found"
            };
        }
        public static FormError UserCannotLoginInStore()
        {
            return new FormError
            {
                Code = nameof(UserCannotLoginInStore).PascalToKebabCase(),
                Description = "User cannot login to current store"
            };
        }

        public static FormError AccountIsBlocked()
        {
            return new FormError
            {
                Code = nameof(AccountIsBlocked).PascalToKebabCase(),
                Description = "Account is blocked"
            };
        }

        public static FormError OperationFailed()
        {
            return new FormError
            {
                Code = nameof(OperationFailed).PascalToKebabCase(),
                Description = "Operation failed"
            };
        }

        public static FormError ResetPasswordIsTurnedOff()
        {
            return new FormError
            {
                Code = nameof(ResetPasswordIsTurnedOff).PascalToKebabCase(),
                Description = "Reset password by code is turned off"
            };
        }

        public static FormError InvalidToken()
        {
            return new FormError
            {
                Code = nameof(InvalidToken).PascalToKebabCase(),
                Description = "Token is invalid or expired"
            };
        }
        public static FormError InvalidUrl()
        {
            return new FormError
            {
                Code = nameof(InvalidUrl).PascalToKebabCase(),
                Description = "Url is invalid"
            };
        }
        public static FormError ResetPasswordInvalidData()
        {
            return new FormError
            {
                Code = nameof(ResetPasswordInvalidData).PascalToKebabCase(),
                Description = "Reset password data is invalid"
            };
        }
        public static FormError PasswordAndConfirmPasswordDoesNotMatch()
        {
            return new FormError
            {
                Code = nameof(PasswordAndConfirmPasswordDoesNotMatch).PascalToKebabCase(),
                Description = "Password and Confirm password doesn't match"
            };
        }
        public static FormError InvitationHasAreadyBeenUsed()
        {
            return new FormError
            {
                Code = nameof(InvitationHasAreadyBeenUsed).PascalToKebabCase(),
                Description = "Invitation has already been used"
            };
        }

    }
}
