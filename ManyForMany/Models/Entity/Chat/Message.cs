using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class Message : IId<int>
    {
        private Message()
        {

        }

        public Message(ApplicationUser  author, string text)
        {
            Text = text;
            AuthorId = author.Id;
            CreateTime = DateTime.Now;
        }

        [Key]
        public int Id { get; private set; }

        public string AuthorId { get; private set; }

        public string Text { get; private set; }

        public DateTime CreateTime { get; private set; }
    }
}
