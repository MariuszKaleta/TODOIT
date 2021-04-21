using System;
using System.Threading.Tasks;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.Order;
using TODOIT.ViewModel.Order;

namespace TODOIT.Repositories.Contracts
{
    public static class OrderRepositoryExtension
    {
        public static async Task<Order> Update(this IOrderRepository orderRepository, CreateOrderViewModel model, Guid orderId, string userId)
        {
            var iAmOwnerAsync = orderRepository.IAmOwner(orderId, userId);

            if (!await iAmOwnerAsync)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            return await orderRepository.Update(model, orderId);
        }

        public static async void Delete(this IOrderRepository orderRepository, Guid orderId, string userId)
        {
            var iAmOwnerAsync = orderRepository.IAmOwner(orderId, userId);

            if (!await iAmOwnerAsync)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            orderRepository.Delete(orderId, true);
        }
    }
}