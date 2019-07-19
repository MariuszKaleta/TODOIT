using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.User;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class Message 
    {
        private Message()
        {

        }

        public Message(ApplicationUser  author, Chat chat, string text)
        {
            Text = text;
            AuthorId = author.Id;
            CreateTime = DateTime.Now;
            Chat = chat;
        }

        [Key]
        public string Id { get; private set; }

        [Required]
        public string AuthorId { get; private set; }

        [Required]
        public string Text { get; private set; }

        [Required]
        public Chat Chat { get; set; }

        public DateTime CreateTime { get; private set; }
    }
}
