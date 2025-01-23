using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;// namespace is used to make all the file visible to each other

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, 
AppRole,int,IdentityUserClaim<int>,AppUserRole,
IdentityUserLogin<int>,IdentityRoleClaim<int>,
IdentityUserToken<int>>(options) //class for datacontext the uses the dependency injection to use the dbCOntextoptions 
{
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder Builder)
    {
        base.OnModelCreating(Builder);


        //build the   table for the AppUser
        Builder.Entity<AppUser>()
        .HasMany(r=> r.UserRoles)
        .WithOne(u => u.User)
        .HasForeignKey(k => k.UserId)
        .IsRequired();

        //build the   table for the AppRole
        Builder.Entity<AppRole>()
        .HasMany(r=> r.UserRoles)
        .WithOne(u => u.Role)
        .HasForeignKey(k => k.RoleId)
        .IsRequired();


        Builder.Entity<UserLike>()
        .HasKey(k => new { k.SourceUserId, k.TargetUserId });
        Builder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<UserLike>()
        .HasOne(s => s.TorgetUser)
        .WithMany(l => l.LikedBy)
        .HasForeignKey(k => k.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<Message>()
        .HasKey(k => new { k.Id });
        Builder.Entity<Message>()
        .HasOne(s => s.Sender)
        .WithMany(l => l.MessagesSent)
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<Message>()
        .HasKey(k => new { k.Id });
        Builder.Entity<Message>()
        .HasOne(s => s.Recipient)
        .WithMany(l => l.MessagesRecieved)
        .OnDelete(DeleteBehavior.Restrict);
    }

}
