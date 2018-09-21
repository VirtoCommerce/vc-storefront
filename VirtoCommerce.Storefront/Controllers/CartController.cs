using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order.Services;
using orderModel = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute("cart")]
    public class CartController : StorefrontControllerBase
    {
        private readonly IOrderModule _orderApi;
        private readonly ICustomerOrderService _orderService;

        public CartController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IOrderModule orderApi, ICustomerOrderService orderService)
            : base(workContextAccessor, urlBuilder)
        {
            _orderApi = orderApi;
            _orderService = orderService;
        }

        // GET: /cart
        [HttpGet]
        public ActionResult Index()
        {
            return View("cart", WorkContext);
        }

        // GET: /cart/checkout
        [HttpGet("checkout")]
        public ActionResult Checkout()
        {
            WorkContext.Layout = "checkout_layout";
            return View("checkout", WorkContext);
        }

        // GET: /cart/checkout/paymentform?orderNumber=...
        [HttpGet("checkout/paymentform")]
        public async Task<ActionResult> PaymentForm(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                return NotFound("Order with number " + orderNumber + " not found.");
            }

            var incomingPayment = order.InPayments?.FirstOrDefault(x => x.PaymentMethodType.EqualsInvariant("PreparedForm"));
            if (incomingPayment == null)
            {
                return BadRequest("Order doesn't have any payment of type: PreparedForm");
            }
            var processingResult = await _orderApi.ProcessOrderPaymentsAsync(order.Id, incomingPayment.Id);

            WorkContext.PaymentFormHtml = processingResult.HtmlForm;

            return View("payment-form", WorkContext);
        }

        // GET/POST: /cart/externalpaymentcallback
        [HttpGet("externalpaymentcallback")]
        [HttpPost("externalpaymentcallback")]
        public async Task<ActionResult> ExternalPaymentCallback()
        {
            var callback = new orderModel.PaymentCallbackParameters
            {
                Parameters = new List<orderModel.KeyValuePair>()
            };

            foreach (var pair in HttpContext.Request.Query)
            {
                callback.Parameters.Add(new orderModel.KeyValuePair
                {
                    Key = pair.Key,
                    Value = pair.Value
                });
            }
            if (HttpContext.Request.HasFormContentType)
            {
                foreach (var pair in HttpContext.Request.Form)
                {
                    callback.Parameters.Add(new orderModel.KeyValuePair
                    {
                        Key = pair.Key,
                        Value = pair.Value
                    });
                }
            }

            var postProcessingResult = await _orderApi.PostProcessPaymentAsync(callback);
            if (postProcessingResult.IsSuccess.HasValue && postProcessingResult.IsSuccess.Value)
            {
                return StoreFrontRedirect("~/cart/thanks/" + postProcessingResult.OrderId);
            }
            else
            {
                return View("error");
            }
        }

        // GET: /cart/thanks/{orderNumber}
        [HttpGet("thanks/{orderNumber}")]
        public async Task<ActionResult> Thanks(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);

            if (order == null || order.CustomerId != WorkContext.CurrentUser.Id)
            {
                return NotFound("Order with number " + orderNumber + " not found.");
            }

            WorkContext.CurrentOrder = order;

            return View("thanks", WorkContext);
        }
    }
}
