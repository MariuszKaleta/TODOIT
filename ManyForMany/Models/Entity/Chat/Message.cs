using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class Message : IId<int>
    {
        private Message()
        {

        }

        public Message(string text)
        {
            Text = text;
            CreateTime = DateTime.Now;
        }

        [Key]
        public int Id { get; private set; }

        public string Text { get; private set; }

        public DateTime CreateTime { get; private set; }
    }
}
