using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Common.Messages
{
    public interface IHandler<in T> where T : IMessage
    {
        Task Handle(T message);
    }
}