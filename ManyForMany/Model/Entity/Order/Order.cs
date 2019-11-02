using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GraphQlHelper;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Order;

namespace TODOIT.Model.Entity.Order
{
    public class Order : IBaseElement<Guid>
    {
        private Order()
        {

        }

        public Order(OrderViewModel model, ApplicationUser owner)
        {
            this.Assign(model);

            Owner = owner;
            CreateTime = DateTime.Now;
        }

        /*
        public Order(CreateOrderViewModel model,  ApplicationUser owner, Context context)
        {//TODO Add Required Skills
            Title = model.Title;
            Describe = model.Describe;
            Owner = owner;
            CreateTime = DateTime.Now;
            DeadLine = model.DeadLine;
            Status = OrderStatus.CompleteTeam;
            
            RequiredSkills = new List<Skill.Skill>(context.Skills.Get(x => x.Id, model.RequiredSkills));
            GoodIfHave = new List<Skill.Skill>(context.Skills.Get(x => x.Id, model.GoodIfHave));
            Categories = new List<Category.Category>(context.Categories.Get(x => x.Id, model.Categories));
        }
        */

        [Key] public Guid Id { get; private set; }

        [Required] public string Name { get; set; }

        public string Describe { get; set; }

        [Required] public ApplicationUser Owner { get; private set; }

        //public int ProjectChatId { get; set; }

        //[Required]
        public DateTime CreateTime { get; private set; }

        public DateTime DeadLine { get; set; }

        public OrderStatus Status
        {
            get;
            set;

            //todo dodać do usera index obejżanych ofert zamiast listy rejected interested, zostawić tylko added elements

            //  public List<ApplicationUser> RejectedByUsers { get; private set; } 

            //public List<ApplicationUser> InterestedByUsers { get; private set; }

            //public List<ApplicationUser> ActualTeam { get; private set; }

            //public List<Category.Category> Categories { get; private set; }

            //public List<Skill.Skill> RequiredSkills { get; private set; }

            //public List<Skill.Skill> GoodIfHave { get; private set; }

            //public List<ApplicationUser> UsersWhichCanComment { get; private set; }


        }


    }
}

