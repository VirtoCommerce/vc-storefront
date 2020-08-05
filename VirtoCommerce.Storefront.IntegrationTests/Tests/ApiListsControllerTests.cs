using System;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "IntegrationTest")]
    [CollectionDefinition("ApiCartControllerTests", DisableParallelization = true)]
    public class ApiListsControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private bool _isDisposed;

        public ApiListsControllerTests(StorefrontApplicationFactory factory)
        {
            _client = factory.CreateClient();
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
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetListByName_IfListWithNameIsNotExist_ShouldReturnEmptyList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetListsWithProduct_IfListWithProductExist_ShouldReturnExistedList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetListsWithProduct_IfListWithProductIsNotExist_ShouldReturnEmptyList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetListItemsCount_IfListHasNoItems_ShouldReturnZero()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetListItemsCount_IfListHasItems_ShouldReturnExactItemsCount()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddItemToList_IfItemIsAvailable_ShouldReturnListItemsCount()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddItemToList_IfItemIsNotAvailable_ShouldNotUpdateListItems()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task RemoveItemFromList_IfItemAvailable_ShouldReturnListItemsCount()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ClearList_IfListExists_ShouldReturnEmptyList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task SearchList_IfListExists_ShouldReturnList()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task SearchList_IfListNotExists_ShouldReturnEmptyResponse()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task CreateList_ShouldCreateListFromCart()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task MergeListWithCurrentCart_ShouldReturnMergedCart()
        {
            throw new NotImplementedException();
        }
    }
}
