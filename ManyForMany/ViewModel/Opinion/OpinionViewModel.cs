using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Rate;

namespace ManyForMany.ViewModel.Opinion
{
    public class OpinionViewModel
    {
        public string Text { get; set; }

        public Rate Quality { get;  set; }

        public Rate Salary { get;set; }
    }
}
