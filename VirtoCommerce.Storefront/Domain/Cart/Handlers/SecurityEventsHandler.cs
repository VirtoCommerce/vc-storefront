using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Domain.Cart.Handlers
{
    public class SecurityEventsHandler : IEventHandler<UserLoginEvent>
    {
        private readonly ICartModule _cartApi;
        private readonly ICartBuilder _cartBuilder;
        public SecurityEventsHandler(ICartBuilder cartBuilder, ICartModule cartApi)
        {
            _cartBuilder = cartBuilder;
            _cartApi = cartApi;
        }

        #region IEventHandler<UserLoginEvent>

        /// <summary>
        /// Merge an anonymous cart into a shopping cart belonging to a registered customer
        /// </summary>
        /// <param name="userLoginEvent"></param>
        public virtual async Task Handle(UserLoginEvent @event)
        {
            if (@event == null)
                return;

            var workContext = @event.WorkContext;
            var prevUser = @event.WorkContext.CurrentUser;
            var prevUserCart = @event.WorkContext.CurrentCart?.Value;
            var newUser = @event.User;

            //If previous user was anonymous and it has not empty cart need merge anonymous cart to personal
            if (prevUser?.IsRegisteredUser != true && prevUserCart != null && prevUserCart.Items.Any())
            {
                //we load or create cart for new user
                await _cartBuilder.LoadOrCreateNewTransientCartAsync(prevUserCart.Name, workContext.CurrentStore, newUser, workContext.CurrentLanguage, workContext.CurrentCurrency);
                await _cartBuilder.MergeWithCartAsync(prevUserCart);
                await _cartBuilder.SaveAsync();
                await _cartApi.DeleteCartsAsync(new[] { prevUserCart.Id }.ToList());
            }
        }

        #endregion
    }
}
