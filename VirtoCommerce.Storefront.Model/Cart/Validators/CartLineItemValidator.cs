using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Cart.Validators
{
    public class CartLineItemValidator : AbstractValidator<LineItem>
    {
        public CartLineItemValidator(ShoppingCart cart)
        {
            RuleSet("strict", () =>
            {
                RuleFor(x => x).Custom((lineItem, context) =>
                {
                    lineItem.ValidationErrors.Clear();

                    if (lineItem.Product == null || !lineItem.Product.IsActive || !lineItem.Product.IsBuyable)
                    {
                        var unavailableError = new UnavailableError();
                        lineItem.ValidationErrors.Add(unavailableError);
                        context.AddFailure(new ValidationFailure(nameof(lineItem.Product), "The product is not longer available for purchase"));
                    }
                    else if (lineItem.Product.Price == null || lineItem.Product.Price.GetTierPrice(lineItem.Quantity).Price == 0)
                    {
                        var unavailableError = new UnavailableError();
                        lineItem.ValidationErrors.Add(unavailableError);
                    }
                    else
                    {
                        var isProductAvailable = new ProductIsAvailableSpecification(lineItem.Product).IsSatisfiedBy(lineItem.Quantity);
                        if (!isProductAvailable)
                        {
                            var availableQuantity = lineItem.Product.AvailableQuantity;
                            var qtyError = new QuantityError(availableQuantity);
                            lineItem.ValidationErrors.Add(qtyError);
                            context.AddFailure(new ValidationFailure(nameof(lineItem.Product.AvailableQuantity), "The product available qty is changed"));
                        }

                        var tierPrice = lineItem.Product.Price.GetTierPrice(lineItem.Quantity);
                        if (tierPrice.Price > lineItem.SalePrice)
                        {
                            var priceError = new PriceError(lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price, tierPrice.PriceWithTax);
                            lineItem.ValidationErrors.Add(priceError);
                            context.AddFailure(new ValidationFailure(nameof(lineItem.SalePrice), "The product price is changed"));
                        }
                    }
                });
            });


        }
    }
}
