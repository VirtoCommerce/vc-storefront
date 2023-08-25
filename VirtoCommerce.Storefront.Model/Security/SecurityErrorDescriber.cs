using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public static partial class SecurityErrorDescriber
    {
        public static FormError UsernameOrEmailIsRequired()
        {
            return new FormError
            {
                Code = nameof(UsernameOrEmailIsRequired).PascalToKebabCase(),
                Description = "Please provide a username or email"
            };
        }
        public static FormError LoginFailed()
        {
            return new FormError
            {
                Code = nameof(LoginFailed).PascalToKebabCase(),
                Description = "Login attempt failed. Please check your credentials."
            };
        }

        public static FormError UserNotFound()
        {
            return new FormError
            {
                Code = nameof(UserNotFound).PascalToKebabCase(),
                Description = "User not found. Please ensure you've entered the correct information."
            };
        }
        public static FormError UserCannotLoginInStore()
        {
            return new FormError
            {
                Code = nameof(UserCannotLoginInStore).PascalToKebabCase(),
                Description = "Access denied. You cannot sign in to the current store"
            };
        }

        public static FormError PhoneNumberNotFound()
        {
            return new FormError
            {
                Code = nameof(PhoneNumberNotFound).PascalToKebabCase(),
                Description = "Password reset failed. Phone number not found for verification."
            };
        }

        public static FormError AccountIsBlocked()
        {
            return new FormError
            {
                Code = nameof(AccountIsBlocked).PascalToKebabCase(),
                Description = "Your account has been blocked. Please contact support for assistance."
            };
        }

        public static FormError EmailVerificationIsRequired()
        {
            return new FormError
            {
                Code = nameof(EmailVerificationIsRequired).PascalToKebabCase(),
                Description = "Email verification required. Please verify your email address."
            };
        }

        public static FormError OperationFailed()
        {
            return new FormError
            {
                Code = nameof(OperationFailed).PascalToKebabCase(),
                Description = "Oops, something went wrong. The operation could not be completed."
            };
        }

        public static FormError ResetPasswordIsTurnedOff()
        {
            return new FormError
            {
                Code = nameof(ResetPasswordIsTurnedOff).PascalToKebabCase(),
                Description = "Password reset by code is currently unavailable."
            };
        }

        public static FormError InvalidToken()
        {
            return new FormError
            {
                Code = nameof(InvalidToken).PascalToKebabCase(),
                Description = "Sorry, the token is invalid or has expired. Please request a new one."
            };
        }
        public static FormError InvalidUrl()
        {
            return new FormError
            {
                Code = nameof(InvalidUrl).PascalToKebabCase(),
                Description = "The URL you provided is not valid."
            };
        }
        public static FormError ResetPasswordInvalidData()
        {
            return new FormError
            {
                Code = nameof(ResetPasswordInvalidData).PascalToKebabCase(),
                Description = "Password reset data is invalid. Please try again."
            };
        }
        public static FormError PasswordAndConfirmPasswordDoesNotMatch()
        {
            return new FormError
            {
                Code = nameof(PasswordAndConfirmPasswordDoesNotMatch).PascalToKebabCase(),
                Description = "Passwords don't match. Please ensure both passwords are the same."
            };
        }
        public static FormError InvitationHasAreadyBeenUsed()
        {
            return new FormError
            {
                Code = nameof(InvitationHasAreadyBeenUsed).PascalToKebabCase(),
                Description = "This invitation has already been used. Please contact the sender if you need assistance."
            };
        }
        public static FormError PhoneNumberVerificationFailed()
        {
            return new FormError
            {
                Code = nameof(PhoneNumberVerificationFailed).PascalToKebabCase(),
                Description = "Phone number verification failed. Please try again or contact support."
            };
        }
        public static FormError ErrorSendNotification(string error)
        {
            return new FormError
            {
                Code = nameof(ErrorSendNotification).PascalToKebabCase(),
                Description = error
            };
        }
        public static FormError UserIsLockedOut()
        {
            return new FormError
            {
                Code = nameof(UserIsLockedOut).PascalToKebabCase(),
                Description = "Your account has been locked. Please contact support for assistance."
            };
        }
        public static FormError UserIsTemporaryLockedOut()
        {
            return new FormError
            {
                Code = nameof(UserIsLockedOut).PascalToKebabCase(),
                Description = "Your account has been temporarily locked. Please try again after some time."
            };
        }

    }
}
