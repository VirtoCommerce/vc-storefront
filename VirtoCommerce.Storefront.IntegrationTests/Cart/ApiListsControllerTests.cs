using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model.Cart;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "CI")]
    [CollectionDefinition("ApiCartControllerTests", DisableParallelization = true)]
    public class ApiListsControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _type;
        private bool _isDisposed;

        public ApiListsControllerTests(StorefrontApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _type = new Fixture().Create<string>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _client?.Dispose();
            }

            _isDisposed = true;
        }

        ~ApiListsControllerTests()
        {
            Dispose(false);
        }

        [Fact]
        public async Task GetListByName_IfListWithNameExist_ShouldReturnExistedList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listName = new Fixture().Create<string>();

            _client.CreateList(listName, _type);

            //act
            var list = await _client.GetList(listName, _type);

            //assert
            GetListName(list).Should().BeEquivalentTo(listName);

            _client
                .DeleteListsByIds(new[] { GetId(list) })
                .Logout();
        }

        [Fact]
        public async Task GetListByName_IfListWithNameIsNotExist_ShouldReturnEmptyList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listName = new Fixture().Create<string>();

            //act
            var list = await _client.GetList(listName, _type);

            //assert
            GetListName(list).Should().BeEquivalentTo(listName);
            GetComparationResultFromFile(list, "GetEmptyListForAdmin", new[] { "$" }, new[] { "id", "name", "objectType", "type" })
                .Should()
                .BeNull();

            _client
                .Logout();
        }

        [Fact]
        public async Task GetListsWithProduct_IfListWithProductExist_ShouldReturnExistedList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listNames = new Fixture().Create<string[]>();

            foreach (var listName in listNames)
            {
                _client
                    .CreateList(listName, _type)
                    .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Quadcopter, Quantity = 1 });
            }

            var request = new GetCartsWithProductRequest()
            {
                ProductId = Product.Quadcopter,
                Type = _type
            };

            //act
            var result = await _client.GetListsWithProduct(request);

            //assert
            var lists = JArray.Parse(result)
                .Select(x => x.ToString())
                .ToArray();

            listNames.OrderBy(x => x).Should().Equal(lists.OrderBy(x => x));

            var listIds = new List<string>();

            foreach (var listName in lists)
            {
                var list = await _client.GetList(listName, _type);
                listIds.Add(GetId(list));
            }

            _client
                .DeleteListsByIds(listIds.ToArray())
                .Logout();
        }

        [Fact]
        public async Task GetListsWithProduct_IfListWithProductIsNotExist_ShouldReturnEmptyList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var request = new GetCartsWithProductRequest()
            {
                ProductId = Product.Quadcopter,
                Type = _type
            };

            //act
            var result = await _client.GetListsWithProduct(request);

            //assert
            var lists = JArray.Parse(result)
                .Select(x => x.ToString())
                .ToArray();

            lists.Should().BeNullOrEmpty();

            _client
                .Logout();
        }

        // TODO: Doesn't work - method of adding items (storefront) ignores the Quantity property in AddCartItem
        [Fact]
        public async Task AddItemToList_IfItemIsAvailable_ShouldReturnListItemsCount()
        {
            //arrange
            _client
                .Login("admin", "store");

            var randomizer = new Fixture();
            var listName = randomizer.Create<string>();

            _client
                .CreateList(listName, _type);

            var item = new AddCartItem
            {
                Id = Product.Quadcopter,
                Quantity = randomizer.Create<int>(),
                ListName = listName,
                Type = _type
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            //act
            var response = await _client.PostAsync<ShoppingCartItems>(TestEnvironment.ListItemsEndpoint, content);

            //assert
            var list = await _client.GetList(listName, _type);

            response.Should().BeEquivalentTo(new ShoppingCartItems { ItemsCount = item.Quantity });

            _client
                .DeleteListsByIds(new[] { GetId(list) })
                .Logout();
        }

        [Fact]
        public async Task AddItemToList_IfItemIsNotAvailable_ShouldNotUpdateListItems()
        {
            //arrange
            _client
                .Login("admin", "store");

            var randomizer = new Fixture();
            var listName = randomizer.Create<string>();

            _client
                .CreateList(listName, _type);

            var item = new AddCartItem
            {
                Id = randomizer.Create<string>(),
                Quantity = randomizer.Create<int>(),
                ListName = listName,
                Type = _type
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            //act
            var response = await _client.PostAsync<ShoppingCartItems>(TestEnvironment.ListItemsEndpoint, content);

            //assert
            var list = await _client.GetList(listName, _type);

            response.Should().BeEquivalentTo(new ShoppingCartItems { ItemsCount = 0 });

            _client
                .DeleteListsByIds(new[] { GetId(list) })
                .Logout();
        }

        [Fact]
        public async Task RemoveItemFromList_IfItemAvailable_ShouldReturnListItemsCount()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listName = new Fixture().Create<string>();

            _client
                .CreateList(listName, _type)
                .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Quadcopter, Quantity = 1 })
                .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Octocopter, Quantity = 1 });

            var list = await _client.GetList(listName, _type);
            var lineItemIds = GetLineItemIds(list);

            //act
            _client.DeleteListItem(lineItemIds.FirstOrDefault(), listName, _type);

            //assert
            var updatedList = await _client.GetList(listName, _type);

            lineItemIds.Count().Should().Be(2);
            GetLineItemIds(updatedList).Count().Should().Be(1);

            _client
                .DeleteListsByIds(new[] { GetId(updatedList) })
                .Logout();
        }

        [Fact]
        public async Task ClearList_IfListExists_ShouldReturnEmptyList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listName = new Fixture().Create<string>();

            _client
                .CreateList(listName, _type)
                .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Quadcopter, Quantity = 1 })
                .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Octocopter, Quantity = 1 });

            var list = await _client.GetList(listName, _type);

            //act
            _client.ClearList(listName, _type);

            //assert
            var updatedList = await _client.GetList(listName, _type);

            GetLineItemIds(list).Count().Should().Be(2);
            GetComparationResultFromFile(updatedList, "GetEmptyListForAdmin", new[] { "$" }, new[] { "id", "name", "objectType", "type" })
                .Should()
                .BeNull();

            _client
                .DeleteListsByIds(new[] { GetId(updatedList) })
                .Logout();
        }

        [Fact]
        public async Task SearchList_IfListExists_ShouldReturnList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listNames = new Fixture().Create<string[]>();

            foreach (var listName in listNames)
            {
                _client
                    .CreateList(listName, _type)
                    .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Quadcopter, Quantity = 1 });
            }

            var searchCriteria = new CartSearchCriteria()
            {
                Type = _type
            };

            //act
            var result = await _client.SearchLists(searchCriteria);

            //assert
            var lists = JObject.Parse(result)["results"]?
                .Select(x => GetIdAndName(x.ToString()))
                .ToArray();

            listNames.OrderBy(x => x).Should().Equal(lists.Select(x => x.name).OrderBy(x => x));

            _client
                .DeleteListsByIds(lists.Select(x => x.id).ToArray())
                .Logout();
        }

        [Fact]
        public async Task SearchList_IfListNotExists_ShouldReturnEmptyResponse()
        {
            //arrange
            _client
                .Login("admin", "store");

            var searchCriteria = new CartSearchCriteria()
            {
                Type = _type
            };

            //act
            var result = await _client.SearchLists(searchCriteria);

            //assert
            var lists = JObject.Parse(result)["results"]?
                .ToArray();

            lists.Should().BeNullOrEmpty();

            _client
                .Logout();
        }

        [Fact]
        public async Task CreateList_ShouldReturnEmptyList()
        {
            //arrange
            _client
                .Login("admin", "store");

            var listName = new Fixture().Create<string>();

            //act
            _client.CreateList(listName, _type);

            //assert
            var list = await _client.GetList(listName, _type);

            GetComparationResultFromFile(list, "GetEmptyListForAdmin", new[] { "$" }, new[] { "id", "name", "objectType", "type" })
                .Should()
                .BeNull();

            _client
                .DeleteListsByIds(new[] { GetId(list) })
                .Logout();
        }

        [Fact]
        public async Task MergeListWithCurrentCart_ShouldReturnMergedCart()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart()
                .InsertCartItem(new AddCartItem() { ProductId = Product.Quadcopter, Quantity = 1 });

            var listName = new Fixture().Create<string>();
            _client
                .CreateList(listName, _type)
                .InsertListItem(new AddCartItem() { ListName = listName, Type = _type, ProductId = Product.Octocopter, Quantity = 1 });

            //act
            _client.MergeWithCurrentCart(listName, _type);

            //assert
            var cart = await _client.GetCart();

            GetLineItemIds(cart).Count().Should().Be(2);

            _client
                .DeleteListsByIds(new[] { GetId(cart) })
                .Logout();
        }

        private string GetComparationResultFromFile(string actualJson, string fileNameWithExpectation, IEnumerable<string> pathsForExclusion = null, IEnumerable<string> excludedProperties = null)
        {
            var expectedResponse = File.ReadAllText($"Responses\\{fileNameWithExpectation}.json");
            return ComparationResult(actualJson, expectedResponse, pathsForExclusion, excludedProperties);
        }

        private string ComparationResult(string actualJson, string expectedJson, IEnumerable<string> pathsForExclusion = null, IEnumerable<string> excludedProperties = null)
        {
            var actualResult = JToken.Parse(actualJson).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            var expectedResult = JToken.Parse(expectedJson).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            return new JsonDiffPatch().Diff(actualResult, expectedResult);
        }

        private (string id, string name) GetIdAndName(string actualJson)
        {
            return (GetId(actualJson), GetListName(actualJson));
        }

        private string GetId(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["id"]?.ToString();
        }

        private string GetListName(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["name"]?.ToString();
        }

        private string[] GetLineItemIds(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["items"]?.Children()["id"].Select(x => x.ToString()).ToArray();
        }
    }
}
