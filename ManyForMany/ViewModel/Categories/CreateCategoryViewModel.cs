using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ManyForMany.ViewModel.Categories
{
    public class CreateCategoryViewModel : CategoryViewModel
    {
        public IFormFile Logo { get; set; }
    }
}
