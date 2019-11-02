using System;
using System.ComponentModel.DataAnnotations;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
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
