using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.ChatRoomMessage;
using ChatApp.Domain.Entities.UserFriend;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"SERVER=tcp:127.0.0.1,1433;DATABASE=development;User ID=sa;Password=DevelopmentPassword.2025;TrustServerCertificate=True",
                o => o.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)
            );
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

            modelBuilder.Entity<ChatRoomTextMessageEntity>().HasKey(crt => crt.ChatRoomTextMessageId);
            modelBuilder.Entity<ChatRoomTextMessageEntity>()
            .HasOne(crt => crt.Sender)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserFriendEntity> UserFriends { get; set; }
        public DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public DbSet<ChatRoomTextMessageEntity> ChatRoomTextMessages { get; set; }
    }
}