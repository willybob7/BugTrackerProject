using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<BugAttributes> Bugs { get; set; }
        public DbSet<ProjectAttributes> Projects { get; set; }
        public DbSet<ScreenShots> ScreenShots { get; set; }
        public DbSet<ProjectBugs> ProjectBugs { get; set; }
        public DbSet<UsersAssigned> UsersAssigned { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BugHistory> BugHistory { get; set; }
        public DbSet<ProjectHistory> ProjectHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }

    }
}
