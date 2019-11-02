using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }


        public Context()
        {

        }

        #region Properties

        public DbSet<Order.Order> Orders { get; set; }

        public DbSet<InterestedOrder> InterestedOrders { get; set; }

        public DbSet<Skill.Skill> Skills { get; set; }

        public DbSet<ReuiredSkill> UsedSkills { get; set; }

        public DbSet<HeadSkill> HeadSkills { get; set; }

        public DbSet<Opinion> Opinions { get; set; }

        //  public DbSet<Chat.TeamChat> TeamChats { get; set; }

        // public DbSet<Chat.SingleChat> SingleChats { get; set; }

        //     public DbSet<Message> Messages { get; set; }

        //      public DbSet<MathcedCoWorkers> MathcedCoWorkerses { get; set; }

        //      public DbSet<Category.Category> Categories { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var interestedOrder = builder.Entity<InterestedOrder>();

            interestedOrder
                .HasOne<Order.Order>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            interestedOrder
                .HasOne<ApplicationUser>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);



            /*
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

    */

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


    }
}