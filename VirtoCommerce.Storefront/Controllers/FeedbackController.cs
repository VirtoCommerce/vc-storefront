using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Domain.Feedback;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    [ValidateAntiForgeryToken]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackItemFactory _feedbackItemFactory;
        private readonly IFeedbackItemService<FeedbackItem, (HttpStatusCode StatusCode, string Content)> _feedbackItemService;

        public FeedbackController(IFeedbackItemFactory feedbackItemFactory, IFeedbackItemService<FeedbackItem, (HttpStatusCode StatusCode, string Content)> feedbackItemService)
        {
            _feedbackItemFactory = feedbackItemFactory;
            _feedbackItemService = feedbackItemService;
        }

        [HttpPost("call")]
        public async Task<IActionResult> CallService(Dictionary<string, string> data)
        {
            var name = Request.Headers["service"];
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            var item = _feedbackItemFactory.GetItem(name);
            item.AdditionalParams = data?.Select(p => $"{p.Key}={data[p.Key]}").ToList();
            var serviceResponse = await _feedbackItemService.SendAsync(item);
            var statusCode = (int)serviceResponse.StatusCode;
            if (statusCode == 200)
            {
                if (TryParseJson(serviceResponse.Content, out var content))
                {
                    return Json(content);
                }
                return Ok(serviceResponse.Content);
            }
            else
            {
                return new StatusCodeResult(statusCode);
            }
        }

        private bool TryParseJson(string json, out object result)
        {
            try
            {
                result = JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
