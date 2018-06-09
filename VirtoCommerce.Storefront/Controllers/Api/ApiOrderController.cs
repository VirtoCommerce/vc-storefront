using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Order;
using orderModel = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [ValidateAntiForgeryToken]
    public class ApiOrderController : StorefrontControllerBase
    {
        private readonly IOrderModule _orderApi;
        private readonly IStoreModule _storeApi;

        public ApiOrderController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IOrderModule orderApi, IStoreModule storeApi)
            : base(workContextAccessor, urlBuilder)
        {
            _orderApi = orderApi;
            _storeApi = storeApi;
        }

        // POST: storefrontapi/orders/search
        [HttpPost]
        public async Task<ActionResult> SearchCustomerOrders([FromBody] OrderSearchCriteria criteria)
        {
            if (criteria == null)
            {
                criteria = new OrderSearchCriteria();
            }
            //Does not allow to see a other customer orders
            criteria.CustomerId = WorkContext.CurrentUser.Id;

            var result = await _orderApi.SearchAsync(criteria.ToSearchCriteriaDto());

            return Json(new
            {
                Results = result.CustomerOrders.Select(x => x.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage)),
                result.TotalCount
            });
        }

        // GET: storefrontapi/orders/{orderNumber}
        [HttpGet]
        public async Task<ActionResult> GetCustomerOrder(string orderNumber)
        {
            var retVal = await GetOrderByNumber(orderNumber);
            return Json(retVal);
        }

        // GET: storefrontapi/orders/{orderNumber}/newpaymentdata
        [HttpGet]
        public async Task<ActionResult> GetNewPaymentData(string orderNumber)
        {
            var order = await GetOrderByNumber(orderNumber);

            var storeDto = await _storeApi.GetStoreByIdAsync(order.StoreId);
            var paymentMethods = storeDto.PaymentMethods
                                        .Where(x => x.IsActive.Value)
                                        .Select(x => x.ToPaymentMethod(order));

            var paymentDto = await _orderApi.GetNewPaymentAsync(order.Id);
            var payment = paymentDto.ToOrderInPayment(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);

            return Json(new
            {
                Payment = payment,
                PaymentMethods = paymentMethods,
                Order = order
            });
        }

        // POST: storefrontapi/orders/{orderNumber}/payments/{paymentNumber}/cancel
        [HttpPost]
        public async Task<ActionResult> CancelPayment(string orderNumber, string paymentNumber)
        {
            //Need lock to prevent concurrent access to same object
            using (await AsyncLock.GetLockByKey(GetAsyncLockKey(orderNumber, WorkContext)).LockAsync())
            {
                var orderDto = await GetOrderDtoByNumber(orderNumber);
                var payment = orderDto.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(paymentNumber));
                if (payment != null)
                {
                    payment.IsCancelled = true;
                    payment.CancelReason = "Canceled by customer";
                    payment.CancelledDate = DateTime.UtcNow;
                    payment.PaymentStatus = "Cancelled";
                    await _orderApi.UpdateAsync(orderDto);
                }
            }
            return Ok();
        }

        // POST: storefrontapi/orders/{orderNumber}/payments/{paymentNumber}/process
        [HttpPost]
        public async Task<ActionResult> ProcessOrderPayment(string orderNumber, string paymentNumber, [FromBody] orderModel.BankCardInfo bankCardInfo)
        {
            //Need lock to prevent concurrent access to same order
            using (await AsyncLock.GetLockByKey(GetAsyncLockKey(orderNumber, WorkContext)).LockAsync())
            {
                var orderDto = await GetOrderDtoByNumber(orderNumber);
                var paymentDto = orderDto.InPayments.FirstOrDefault(x => x.Number.EqualsInvariant(paymentNumber));
                if (paymentDto == null)
                {
                    throw new StorefrontException("payment " + paymentNumber + " not found");
                }
                var processingResult = await _orderApi.ProcessOrderPaymentsAsync(orderDto.Id, paymentDto.Id, bankCardInfo);
                return Json(new { orderProcessingResult = processingResult, paymentMethod = paymentDto.PaymentMethod });
            }
        }

        // POST: storefrontapi/orders/{orderNumber}/payments
        [HttpPost]
        public async Task<ActionResult> AddOrUpdateOrderPayment(string orderNumber, [FromBody] PaymentIn payment)
        {
            if (payment.Sum.Amount == 0)
            {
                throw new StorefrontException("Valid payment amount is required");
            }
            //Need to lock to prevent concurrent access to same object
            using (await AsyncLock.GetLockByKey(GetAsyncLockKey(orderNumber, WorkContext)).LockAsync())
            {
                var orderDto = await GetOrderDtoByNumber(orderNumber);
                var paymentDto = orderDto.InPayments.FirstOrDefault(x => x.Id.EqualsInvariant(payment.Id));
                if (paymentDto == null)
                {
                    paymentDto = payment.ToOrderPaymentInDto();
                    paymentDto.CustomerId = WorkContext.CurrentUser.Id;
                    paymentDto.CustomerName = WorkContext.CurrentUser.UserName;
                    paymentDto.Status = "New";
                    orderDto.InPayments.Add((orderModel.PaymentIn)paymentDto);
                }
                else
                {
                    paymentDto.BillingAddress = payment.BillingAddress != null ? payment.BillingAddress.ToOrderAddressDto() : null;
                }

                await _orderApi.UpdateAsync(orderDto);
                //Need to return payment with generated id
                orderDto = await _orderApi.GetByIdAsync(orderDto.Id);
                if (string.IsNullOrEmpty(paymentDto.Id))
                {
                    //Because we don't know the new payment id we need to get latest payment with same gateway code
                    paymentDto = orderDto.InPayments.Where(x => x.GatewayCode.EqualsInvariant(payment.GatewayCode)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                }
                return Json(paymentDto);
            }

        }

        // GET: storefrontapi/orders/{orderNumber}/invoice
        [HttpGet]
        public async Task<ActionResult> GetInvoicePdf(string orderNumber)
        {
            var stream = await _orderApi.GetInvoicePdfAsync(orderNumber);

            return File(stream, "application/pdf");
        }

        private async Task<CustomerOrder> GetOrderByNumber(string number)
        {
            var order = await GetOrderDtoByNumber(number);

            WorkContext.CurrentOrder = order.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);
            return WorkContext.CurrentOrder;
        }

        private async Task<orderModel.CustomerOrder> GetOrderDtoByNumber(string number)
        {
            var order = await _orderApi.GetByNumberAsync(number);

            if (order == null || order.CustomerId != WorkContext.CurrentUser.Id)
            {
                throw new StorefrontException($"Order with number {{ number }} not found (or not belongs to current user)");
            }

            return order;
        }

        private static string GetAsyncLockKey(string orderNumber, WorkContext ctx)
        {
            return string.Join(":", "Order", orderNumber, ctx.CurrentStore.Id, ctx.CurrentUser.Id);
        }
    }
}
