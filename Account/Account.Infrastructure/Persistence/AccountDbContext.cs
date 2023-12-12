﻿using Account.Entity.RoleEntity;
using Account.Entity.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence
{
    public sealed class AccountDbContext(DbContextOptions<AccountDbContext> options)
        : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.UsernameNormalized).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder
                .Entity<User>()
                .HasOne(user => user.Profile)
                .WithOne()
                .HasForeignKey<Profile>(profile => profile.Id);

            modelBuilder.Entity<Profile>().HasKey(profile => profile.Id);
            modelBuilder.Entity<Profile>().ToTable("profiles");

            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        }
    }
}
