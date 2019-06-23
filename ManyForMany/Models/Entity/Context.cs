using ManyForMany.Model.Entity.Ofert;
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

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = builder.Entity<ApplicationUser>();

            user.HasMany(x => x.InterestedOrders);
            user.HasMany(x => x.MemberOfOrders);
            user.HasMany(x => x.OwnOrders);
            user.HasMany(x => x.RejectedOrders);

            var order = builder.Entity<Order>();

            order.HasMany(x => x.ActualTeam);
            order.HasMany(x => x.InterestedUsers);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}