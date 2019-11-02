namespace TODOIT.ViewModel.Team
{
    public class PublicUserViewModel : UserViewModel
    {
        private PublicUserViewModel()
        {

        }

        /*
        public PublicUserViewModel(ApplicationUser user, Context context)
        {
            Id = user.Id;
            UserName = user.UserName;
            Name = user.Name;
            SurName = user.Surrname;
            Picture = user.Picture;
            Skills = user.Skills.Select(x => x.ToThumbnail()).ToArray();
            LikedCategories = user.InterestedCategories.Select(x => x.ToThumbnail()).ToArray();
            Opinions = context.Opinions
                .Include(x=>x.Author)
                .Include(x=>x.Order)
                .Where(x => x.Order.Owner.Id == Id)
                .Select(x=>x.ToThumbnail())
                .ToArray();
        }

        public string Id { get;  private set; }

        public string Picture { get; private set; }

        public SkillThumbnailViewModel[] Skills { get; private set; }

        public ThumbnailCategoryViewModel[] LikedCategories { get; private set; }

        //TODO 
        public ThumbnailOpinionViewModel[] Opinions { get; private set; }
    
    */
    }
}
