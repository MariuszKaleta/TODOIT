using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Skill;

namespace ManyForMany.ViewModel.Categories
{
    public class PublicCategoryViewModel :ThumbnailCategoryViewModel
    {
        public PublicCategoryViewModel(Category category) : base(category)
        {
        }
    }
}
