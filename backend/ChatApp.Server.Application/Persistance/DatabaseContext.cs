using ChatApp.Server.Application.Utils;
using ChatApp.Server.Domain.Entities;
using ChatApp.Server.Domain.Entities.ChatRoom;
using ChatApp.Server.Domain.Entities.UserFriend;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Application.Persistance
{
    public class DatabaseContext : IdentityDbContext
    {
        readonly MsSqlOptions _msSSqlOptions;

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

            modelBuilder.Entity<ChatRoomEntity>()
            .HasOne(cr => cr.AdminUser)
            .WithMany()
            .HasForeignKey(cr => cr.AdminUserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasKey(crm => new { crm.ChatRoomId, crm.MemberId });

            modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasOne(crm => crm.ChatRoom)
            .WithMany()
            .HasForeignKey(crm => crm.ChatRoomId);

            modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasOne(crm => crm.Member)
            .WithMany()
            .HasForeignKey(crm => crm.MemberId);

            modelBuilder.Entity<TextMessageEntity>().HasKey(tm => tm.TextMessageId);

            modelBuilder.Entity<TextMessageEntity>()
            .HasKey(tm => tm.TextMessageId);

            modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.ChatRoom)
            .WithMany()
            .HasForeignKey(tm => tm.ChatRoomId);

            modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.Sender)
            .WithMany()
            .HasForeignKey(tm => tm.SenderId);

            modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.ReceiverUser)
            .WithMany()
            .HasForeignKey(tm => tm.ReceiverUserId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserFriendEntity> UserFriends { get; set; }
        public DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public DbSet<ChatRoomMemberEntity> ChatRoomMembers { get; set; }
        public DbSet<TextMessageEntity> TextMessages { get; set; }
    }
}