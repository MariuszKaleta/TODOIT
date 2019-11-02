using TODOIT.ViewModel.Order;

namespace TODOIT.Model.Entity.Order
{
    public static class OrderExtension
    {
        public static void Assign(this Order order, OrderViewModel model)
        {
            order.Name = model.Name;
            order.Describe = model.Describe;
            order.DeadLine = model.DeadLine;
            order.Status = model.OrderStatus;

            //order.RequiredSkills = new List<Skill.Skill>(context.Skills.Get(x => x.Id, model.RequiredSkills));
            //order.GoodIfHave = new List<Skill.Skill>(context.Skills.Get(x => x.Id, model.GoodIfHave));
            //order.Categories = new List<Category.Category>(context.Categories.Get(x => x.Id, model.Categories));
        }
    }
}