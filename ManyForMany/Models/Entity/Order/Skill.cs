using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Order
{
    public class Skill : IId<int>
    {
        private Skill()
        {

        }

        public Skill(string name)
        {
            Name = name;
        }

        [Key]
        public int Id { get; private set; }

        public string Name { get; private set; }
    }
}
