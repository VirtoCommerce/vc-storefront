using FluentValidation;
using VirtoCommerce.Storefront.Model.Cart.Services;

namespace VirtoCommerce.Storefront.Model.Cart.Validators
{
    public class CartValidator : AbstractValidator<ShoppingCart>
    {
        public CartValidator(ICartService cartService)
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Currency).NotNull();
            RuleFor(x => x.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Items).SetValidator(cart => new CartLineItemValidator(cart));
                RuleForEach(x => x.Shipments).SetValidator(cart => new CartShipmentValidator(cart, cartService));
            });
        }
    }
}
