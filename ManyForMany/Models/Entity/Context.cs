using ManyForMany.Models.Entity.Chat;
using ManyForMany.Models.Entity.Rate;
using ManyForMany.Models.Entity.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManyForMany.Models.Entity
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions options) : base(options)
        {

        }

        #region Properties

        public DbSet<Order.Order> Orders { get; set; }

        public DbSet<Chat.TeamChat> TeamChats { get; set; }

        public DbSet<Chat.SingleChat> SingleChats { get; set; }

        public DbSet<Skill.Skill> Skills { get; set; }

        public DbSet<Opinion> Opinions { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<MathcedCoWorkers> MathcedCoWorkerses { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = builder.Entity<ApplicationUser>();

            user.HasMany(x => x.InterestedByOtherUsers);
            user.HasMany(x => x.RejectedByOtherUsers);

            var order = builder.Entity<Order.Order>();

            order.HasMany(x => x.ActualTeam);
            order.HasMany(x => x.InterestedByUsers);
            order.HasOne(x => x.Owner);

            var chat = builder.Entity<Chat.TeamChat>();

            chat.HasMany(x => x.Members);
            chat.HasOne(x => x.Admin);

            var match = builder.Entity<MathcedCoWorkers>();



            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}