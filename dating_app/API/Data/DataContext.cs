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
    public DbSet<Photo> Photos { get; set; }
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
        Builder.Entity<AppRole>() //Build an AppRole Entity
        .HasMany(r=> r.UserRoles) // that has Many user Roles
        .WithOne(u => u.Role) // With one role
        .HasForeignKey(k => k.RoleId)// and a RoleId ForeignKey
        .IsRequired();// it's required


        Builder.Entity<UserLike>() //Build an UserLike Entity
        .HasKey(k => new { k.SourceUserId, k.TargetUserId }); // that has a 2 keys 
        Builder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)// has one sourceUser 
        .WithMany(l => l.LikedUsers)//with Many Liked Users
        .HasForeignKey(s => s.SourceUserId) // and a SourceUserId ForeignKey
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<UserLike>()
        .HasOne(s => s.TorgetUser)// has one TorgetUser 
        .WithMany(l => l.LikedBy)// with many likeby
        .HasForeignKey(k => k.TargetUserId)//  and a TargetUserId ForeignKey
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<Message>()//Build an Message Entity
        .HasKey(k => new { k.Id });//Has Id as it's key
        Builder.Entity<Message>()
        .HasOne(s => s.Sender) //has one Sender
        .WithMany(l => l.MessagesSent)// with many MessagesSent
        .OnDelete(DeleteBehavior.Cascade);

        Builder.Entity<Message>()
        .HasKey(k => new { k.Id });
        Builder.Entity<Message>()
        .HasOne(s => s.Recipient)//has one Recipient
        .WithMany(l => l.MessagesRecieved)//with many MessagesSent
        .OnDelete(DeleteBehavior.Restrict);

        Builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
    }

}
