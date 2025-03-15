using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.UserFriend;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Persistance
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext() : base()
        {

        }

        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFriendEntity>()
            .HasKey(uf => new { uf.InitiatorId, uf.ReceiverId });

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.Initiator)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.InitiatorId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.Receiver)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoomEntity>()
            .HasKey(cr => cr.ChatRoomId);

            modelBuilder.Entity<TextMessageEntity>().HasKey(tm => tm.TextMessageId);

            modelBuilder.Entity<UserMessageEntity>()
            .HasKey(fm => fm.TextMessageId);

            modelBuilder.Entity<UserMessageEntity>()
            .HasOne(um => um.TextMessage)
            .WithMany()
            .IsRequired()
            .HasForeignKey(um => um.TextMessageId);

            modelBuilder.Entity<UserMessageEntity>()
            .HasOne(fm => fm.Receiver)
            .WithMany()
            .IsRequired()
            .HasForeignKey(fm => fm.ReceiverId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserFriendEntity> UserFriends { get; set; }
        public DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public DbSet<TextMessageEntity> TextMessages { get; set; }
        public DbSet<UserMessageEntity> UserMessages { get; set; }
    }
}