using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GraphQlHelper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
{
    public class Message
    {
        private Message()
        {

        }

        public Message(string  authorId, Guid chatId, string text)
        {
            Text = text;
            AuthorId = authorId;
            CreateTime = DateTime.Now;
            ChatId = chatId;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        [ForeignKey(nameof(Author))]
        public string AuthorId { get; private set; }

        [Required]
        public ApplicationUser Author { get; private set; }

        [Required]
        public string Text { get; private set; }

        [Required]
        [ForeignKey(nameof(Chat))]
        public Guid ChatId { get; set; }

        [Required]
        public Chat Chat { get; set; }

        public DateTime CreateTime { get; private set; }
    }
}
