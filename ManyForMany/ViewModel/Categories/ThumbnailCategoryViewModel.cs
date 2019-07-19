using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Skill;
using Microsoft.AspNetCore.Http;

namespace ManyForMany.ViewModel.Categories
{
    public class ThumbnailCategoryViewModel :CategoryViewModel
    {
        public ThumbnailCategoryViewModel(Category category)
        {
            Id = category.Id;
            Name = category.Name;

        }

        public int Id { get; set; }

        public IFormFile Logo { get; set; }
    }
}
