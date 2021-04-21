using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
{
    public class ChatMember
    {
        private ChatMember()
        {

        }

        public ChatMember(string userId, Guid chatId)
        {
            UserId = userId;
            ChatId = chatId;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; private set; }

        [Required]
        public ApplicationUser User { get; private set; }

        [Required]
        [ForeignKey(nameof(Chat))]
        public Guid ChatId { get; private set; }

        [Required]
        public Chat Chat { get; private set; }
    }
}
