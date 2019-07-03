using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Chat;
using ManyForMany.Models.Entity.Order;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Models
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions options) : base(options)
        {

        }

        #region Properties

        public DbSet<Order> Orders { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Skill> Skills { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = builder.Entity<ApplicationUser>();

            //user.HasMany(x => x.InterestedOrders);
           // user.HasMany(x => x.MemberOfOrders);
           user.HasMany(x => x.OpinionsAboutMe);

            user.HasMany(x => x.Chats);

            var order = builder.Entity<Order>();

            order.HasMany(x => x.ActualTeam);
            order.HasMany(x => x.InterestedByUsers);
            order.HasOne(x => x.Owner);

            var chat = builder.Entity<Chat>();

            chat.HasMany(x => x.Members);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}