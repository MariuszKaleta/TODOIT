namespace TODOIT.Model.Entity.Chat
{
    public class TeamChat : Chat
    {
        private TeamChat()
        {

        }
        /*
        public TeamChat(ApplicationUser creator, Context context, CreateChatViewModel model)
        {
            var users = context.Users.Get(model.MemeberUsersId).ToList();
            Members = new List<ApplicationUser>();
            this.Add(users);
            this.Add(creator);

            Admin = creator;
        }

        [Required]
        public ApplicationUser Admin { get; private set; }
    
    */
    }
}
