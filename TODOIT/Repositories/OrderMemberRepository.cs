using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.Skill;

namespace TODOIT.Repositories.Contracts
{
    public static class OrderMembersRepositoryExtension
    {
        public static async Task InviteUserToMakeOrder(this IOrderMembersRepository repository , IOrderRepository orderRepository, IChatRepository chatRepository ,Guid orderId, string ownerId, string[] usersId)
        {
            if (!await orderRepository.IAmOwner(orderId, ownerId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            var task =chatRepository.AddUserToChat(orderId, false, usersId);

            await repository.InviteUserToMakeOrder(orderId, usersId);
            await task;



        }

        public static async Task KickUserFromMakeOrder(this IOrderMembersRepository repository , IOrderRepository orderRepository, IChatRepository chatRepository ,Guid orderId, string ownerId, string[] usersId)
        {
            if (!await orderRepository.IAmOwner(orderId, ownerId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            var task =chatRepository.RemoveUserFromChat(orderId, false, usersId);

            await repository.KickUserFromMakeOrder(orderId, usersId);
            await task;
        }
    }
}
