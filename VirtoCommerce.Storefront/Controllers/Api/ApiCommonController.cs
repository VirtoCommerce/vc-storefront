using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Filters;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Infrastructure.Swagger;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiCommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly Country[] _countriesWithoutRegions;
        private readonly PlatformModuleClient _platformModuleClient;
        private readonly FormFileStorageOptions _formFileStorageOptions;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public ApiCommonController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi, PlatformModuleClient platformModuleClient, IOptions<FormFileStorageOptions> formFileStorageOptions)
            : base(workContextAccessor, urlBuilder)
        {
            _storeApi = storeApi;
            _platformModuleClient = platformModuleClient;
            _formFileStorageOptions = formFileStorageOptions.Value;
            _countriesWithoutRegions = WorkContext.AllCountries
             .Select(c => new Country { Name = c.Name, Code2 = c.Code2, Code3 = c.Code3, RegionType = c.RegionType })
             .ToArray();
        }

        // GET: storefrontapi/countries
        [HttpGet("countries")]
        public ActionResult<Country[]> GetCountries()
        {
            return _countriesWithoutRegions;
        }

        // GET: storefrontapi/countries/{countryCode}/regions
        [HttpGet("countries/{countryCode}/regions")]
        public ActionResult<CountryRegion[]> GetCountryRegions(string countryCode)
        {
            var country = WorkContext.AllCountries.FirstOrDefault(c => c.Code2.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase) || c.Code3.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase));
            if (country != null)
            {
                return country.Regions;
            }
            return Ok();
        }

        // POST: storefrontapi/feedback
        [HttpPost("feedback")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Feedback([FromBody] ContactForm model)
        {
            await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));

            return Ok();
        }

        // POST: storefrontapi/customnotification?formType=...
        // Don't validate antiforgery token otherwise MultipartReader generate the exception
        // "Unexpected end of Stream, the content may have already been read by another component."
        [HttpPost("customnotification")]
        [IgnoreAntiforgeryToken]
        [DisableFormValueModelBinding]
        [UploadFile]
        public async Task<ActionResult> SendCustomNotificationAsync([FromQuery] string formType = null)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var model = new ContactForm { FormType = formType };
            using var sendForm = new MultipartFormDataContent();

            try
            {
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                    {
                        continue;
                    }

                    if (contentDisposition.IsFormDisposition())
                    {
                        // Don't limit the key name length because the
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = MultipartRequestHelper.GetEncoding(section) ?? Encoding.UTF8;

                        using var streamReader = new StreamReader(section.Body, encoding, true, 1024, true);
                        // The value length limit is enforced by MultipartBodyLengthLimit
                        var value = await streamReader.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            model.Contact.Add(key, new[] { value });
                        }
                    }
                    else if (contentDisposition.IsFileDisposition())
                    {
                        // Don't trust the file name sent by the client. To display the file name, HTML-encode the value.
                        var trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        trustedFileName =
                            $"{Path.GetFileNameWithoutExtension(trustedFileName)}_{DateTime.UtcNow.Ticks:X16}{Path.GetExtension(trustedFileName)}";

                        var fileContent = new ByteArrayContent(
                            await MultipartRequestHelper.ProcessStreamedFile(section, contentDisposition,
                                _formFileStorageOptions.PermittedExtensions, _formFileStorageOptions.FileSizeLimit));
                        fileContent.Headers.ContentType =
                            System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                        sendForm.Add(fileContent, "file", Path.GetFileName(trustedFileName));
                    }

                    // Drain any remaining section body that hasn't been consumed and
                    // read the headers for the next section.
                    section = await reader.ReadNextSectionAsync();
                }

                if (sendForm.Any())
                {
                    var blobs = await UploadAssetAsync(sendForm);
                    foreach (var blob in blobs)
                    {
                        model.Contact.Add("File", new[] { blob.Url });
                    }
                }

                await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        private async Task<IList<BlobInfo>> UploadAssetAsync(HttpContent content, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Construct URL
            var baseUrl = _platformModuleClient.BaseUri.AbsoluteUri;
            var url = new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "api/platform/assets").ToString();
            var queryParameters = new List<string>();
            if (_formFileStorageOptions.BlobFolderUrl != null)
            {
                queryParameters.Add($"folderUrl={Uri.EscapeDataString(_formFileStorageOptions.BlobFolderUrl)}");
            }
            if (queryParameters.Count > 0)
            {
                url = $"{url}?{string.Join("&", queryParameters)}";
            }
            // Create HTTP transport objects
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = content;
            // Set Credentials
            if (_platformModuleClient.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _platformModuleClient.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            using var httpResponse = await _platformModuleClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var statusCode = httpResponse.StatusCode;

            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 200)
            {
                var ex = new HttpOperationException($"Operation returned an invalid status code '{statusCode}'");
                if (httpResponse.Content != null) {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, null);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                throw ex;
            }

            // Deserialize Response
            responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                return SafeJsonConvert.DeserializeObject<IList<BlobInfo>>(responseContent, _platformModuleClient.DeserializationSettings);
            }
            catch (JsonException ex)
            {
                throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
            }
        }
    }
}
