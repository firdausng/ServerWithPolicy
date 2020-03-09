using Microsoft.EntityFrameworkCore;
using ServerWithPolicy.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerWithPolicy.Infra.Data
{
    public class AuthorizationDbContext : DbContext
    {
        public AuthorizationDbContext(DbContextOptions<AuthorizationDbContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Subject> Subjects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // many to many mapping for role and permission
            modelBuilder.Entity<PermissionRole>()
                .HasKey(bc => new { bc.PermissionId, bc.RoleId });
            modelBuilder.Entity<PermissionRole>()
                .HasOne(bc => bc.Permission)
                .WithMany(b => b.Roles)
                .HasForeignKey(bc => bc.RoleId);
            modelBuilder.Entity<PermissionRole>()
                .HasOne(bc => bc.Role)
                .WithMany(c => c.Permissions)
                .HasForeignKey(bc => bc.PermissionId);
        }
    }
}