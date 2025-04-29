using ChatApp.Application.Utils;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.UserFriend;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Persistence;

public class DatabaseContext : IdentityDbContext
{
    private readonly MsSqlOptions _msSSqlOptions;

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserFriendEntity> UserFriends { get; set; }
    public DbSet<ChatRoomEntity> ChatRooms { get; set; }
    public DbSet<ChatRoomMemberEntity> ChatRoomMembers { get; set; }
    public DbSet<TextMessageEntity> TextMessages { get; set; }
    public DbSet<PublicUserInfoEntity> PublicUserInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFriendEntity>()
            .HasKey(uf => new { uf.InitiatorId, uf.ReceiverId });

        modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.Initiator)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.InitiatorId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.Receiver)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.ReceiverId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<ChatRoomEntity>()
            .HasKey(cr => cr.ChatRoomId);

        modelBuilder.Entity<ChatRoomEntity>()
            .HasOne(cr => cr.AdminUser)
            .WithMany()
            .HasForeignKey(cr => cr.AdminUserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasKey(crm => new { crm.ChatRoomId, crm.MemberId });

        modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasOne(crm => crm.ChatRoom)
            .WithMany()
            .HasForeignKey(crm => crm.ChatRoomId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<ChatRoomMemberEntity>()
            .HasOne(crm => crm.Member)
            .WithMany()
            .HasForeignKey(crm => crm.MemberId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<TextMessageEntity>().HasKey(tm => tm.TextMessageId);

        modelBuilder.Entity<TextMessageEntity>()
            .HasKey(tm => tm.TextMessageId);

        modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.ChatRoom)
            .WithMany()
            .HasForeignKey(tm => tm.ChatRoomId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.Sender)
            .WithMany()
            .HasForeignKey(tm => tm.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<TextMessageEntity>()
            .HasOne(tm => tm.ReceiverUser)
            .WithMany()
            .HasForeignKey(tm => tm.ReceiverUserId)
            .OnDelete(DeleteBehavior.ClientCascade);
        modelBuilder.Entity<TextMessageEntity>()
            .Property(tm => tm.CreatedAt)
            .HasDefaultValueSql("getutcdate()");

        modelBuilder.Entity<PublicUserInfoEntity>().HasKey(pui => pui.UserId);
        modelBuilder.Entity<PublicUserInfoEntity>()
            .HasIndex(pui => pui.Username)
            .IsUnique();
        modelBuilder.Entity<PublicUserInfoEntity>()
            .HasOne(pui => pui.User)
            .WithOne()
            .HasForeignKey<PublicUserInfoEntity>(pui => pui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}