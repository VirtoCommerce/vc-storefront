using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Domain.Quote.Handlers
{
    public class SecurityEventsHandler : IEventHandler<UserLoginEvent>
    {
        private readonly IQuoteRequestBuilder _quoteBuilder;
        public SecurityEventsHandler(IQuoteRequestBuilder quoteBuilder)
        {
            _quoteBuilder = quoteBuilder;
        }

        #region IEventHandler<UserLoginEvent> Members

        /// <summary>
        /// Merge anonymous user quote to newly log in user quote by log in event
        /// </summary>
        /// <param name="userLoginEvent"></param>
        public virtual async Task Handle(UserLoginEvent @event)
        {
            if (@event == null)
                return;

            var workContext = @event.WorkContext;
            var prevUser = @event.WorkContext.CurrentUser;
            var prevUserCart = @event.WorkContext.CurrentCart?.Value;

            //If previous user was anonymous and it has not empty cart need merge anonymous cart to personal
            if (workContext.CurrentStore.QuotesEnabled && prevUser?.IsRegisteredUser != true && prevUserCart != null
                 && workContext.CurrentQuoteRequest != null && workContext.CurrentQuoteRequest.Value.Items.Any())
            {
                await _quoteBuilder.GetOrCreateNewTransientQuoteRequestAsync(workContext.CurrentStore, @event.User, workContext.CurrentLanguage, workContext.CurrentCurrency);
                await _quoteBuilder.MergeFromOtherAsync(workContext.CurrentQuoteRequest.Value);
                await _quoteBuilder.SaveAsync();
            }
        }

        #endregion
    }
}
