using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizeTester.Model.Error;
using ManyForMany.Controller;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.Entity.User;
using ManyForMany.Model.File;
using ManyForMany.ViewModel.Order;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.Model.Entity.Ofert
{
    public class Order
    {
        private Order()
        {

        }

        public Order(OrderViewModel model, ApplicationUser owner)
        {
            Title = model.Title;
            Describe = model.Describe;
            Owner = owner;
            CreateTime = DateTime.Now;
            DeadLine = model.DeadLine;
            Status = OrderStatus.LookingForOfert;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        [Required]
        public DateTime CreateTime { get; private set; }

        public DateTime DeadLine { get; set; }

        public OrderStatus Status { get; set; }

        [Required]
        public ApplicationUser Owner { get; private set; }

        public List<ApplicationUser> InterestedUsers { get; private set; }
    }

    public enum OrderStatus
    {
        LookingForOfert = 0,
        Suspend,
        InProgress,
        Completed,
        NotCompleted
    }

    public static class OrderExtension
    {
        public static OrderViewModel ToViewModel(this Order order, ImageManager imageManager)
        {
            return new OrderViewModel
            {
                Describe = order.Describe,
                Title = order.Title,
                Images = imageManager.DownladFiles(order.Owner.Id, order.Id).GetAwaiter().GetResult()
            };
        }

        public static Order GetOrder<T>(this List<Order> orders, int id, ILogger<T> logger)
        {
            var order = orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                logger.LogError(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), id);
                throw new MultiLanguageException(nameof(order), Error.ElementDoseNotExist);
            }

            return order;
        }

        public static void Edit(this Order order, OrderViewModel model)
        {
            if (order.Title != model.Title)
            {
                order.Title = model.Title;
            }

            if (order.Describe != model.Describe)
            {
                order.Describe = model.Describe;
            }
        }
    }
}
