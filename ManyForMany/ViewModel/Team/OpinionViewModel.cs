using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Rate;

namespace ManyForMany.ViewModel.Team
{
    public class OpinionViewModel
    {
        public string Text { get; private set; }

        public Rate Quality { get; private set; }

        public Rate Salary { get; private set; }

        public string OrderId { get; private set; }
    }
}
