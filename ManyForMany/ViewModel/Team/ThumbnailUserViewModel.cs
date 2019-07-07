namespace ManyForMany.Models.Entity.User
{
    public class ThumbnailUserViewModel
    {
        public ThumbnailUserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName();
            Picture = user.Picture;
        }

        public string Id { get; private set; }

        public string FirstName { get; set; }

        public string Picture { get; private set; }
    }
}
