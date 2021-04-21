using TODOIT.ViewModel.User;

namespace TODOIT.Model.Entity.User
{
    public static class UserExtension
    {
        public static void Assign(this ApplicationUser user, UserViewModel model)
        {
            user.Surrname = model.Surrname;
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
        }
    }
}