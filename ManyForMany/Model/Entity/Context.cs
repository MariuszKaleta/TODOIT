using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManyForMany.Model.Entity
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var order = builder.Entity<Order>();
            order.HasOne(u => u.Owner);
            order.HasMany(u => u.InterestedUsers);

            var user = builder.Entity<ApplicationUser>();
            user.HasMany(x => x.InterestedOrders);
            user.HasMany(x => x.RejectedOrders);
            user.HasMany(x => x.OwnOrders);
            user.HasMany(x => x.InterestedCooperators);
            user.HasMany(x => x.RejectedCooperators);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}