using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Categories;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Skill
{
    public class Category
    {
        public Category()
        {

        }

        public Category(CategoryViewModel model)
        {
            Name = model.Name;
        }
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class CategoryExtension
    {
        public static Category Get(this IQueryable<Category> categories, int id)
        {
            return categories.Get(x => x.Id, id, Errors.CategoryDoseNotExist);
        }

        public static ThumbnailCategoryViewModel ToThumbnail(this Category category)
        {
            return new ThumbnailCategoryViewModel(category);
        }

        public static PublicCategoryViewModel ToViewModel(this Category category)
        {
            return  new PublicCategoryViewModel(category);
        }
    }
}
