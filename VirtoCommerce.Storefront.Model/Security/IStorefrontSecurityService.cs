using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Security
{
    public interface IStorefrontSecurityService
    {
        Task<bool> CanLoginOnBehalfAsync(string storeId, string customerId);
        Task<SecurityResult> CreateAsync(User user);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> PasswordSignInAsync(string userName, string password);
        Task<User> GetUserByLoginAsync(string loginProvider, string providerKey);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<SecurityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task GeneratePasswordResetTokenAsync(string userId, string storeId, Language Language, string callbackUrl);
        Task<SecurityResult> ResetPasswordAsync(User user, string token, string newPassword);
    }
}
