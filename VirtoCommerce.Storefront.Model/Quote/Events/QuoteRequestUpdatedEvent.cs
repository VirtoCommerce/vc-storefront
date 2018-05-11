using VirtoCommerce.Storefront.Model.Common.Events;

namespace VirtoCommerce.Storefront.Model.Quote.Events
{
    public class QuoteRequestUpdatedEvent : DomainEvent
    {
        public QuoteRequestUpdatedEvent(QuoteRequest quoteRequest)
        {
            QuoteRequest = quoteRequest;
        }

        public QuoteRequest QuoteRequest { get; set; }
    }
}