using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel
{
    public class FileViewModel
    {
        [Required]
        public string Data { get; set; }

        [Required]
        public string Extension { get; set; }
    }
}
