using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.Validators
{
    public class ChangeCartItemPriceValidator : AbstractValidator<ChangeCartItemPrice>
    {
        public ChangeCartItemPriceValidator(ShoppingCart cart)
        {
            RuleFor(x => x.NewPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LineItemId).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((newPriceRequest, context) =>
                {
                    var lineItem = cart.Items.FirstOrDefault(x => x.Id == newPriceRequest.LineItemId);
                    if (lineItem != null)
                    {
                        var newSalePrice = new Money(newPriceRequest.NewPrice, cart.Currency);
                        var oldPrice = lineItem.SalePrice;
                        if (lineItem.Product?.Price != null)
                        {
                            oldPrice = lineItem.Product.Price.GetTierPrice(lineItem.Quantity).Price;
                        }
                        if (oldPrice > newSalePrice)
                        {
                            context.AddFailure(new ValidationFailure(nameof(lineItem.SalePrice), "Unable to set less price"));
                        }

                    }
                });
            });

        }
    }
}
