using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.ChatRoomMessage;
using ChatApp.Domain.Entities.User;
using ChatApp.Domain.Entities.UserFriend;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"SERVER=(localdb)\mssqllocaldb;DATABASE=development;Trusted_Connection=true",
                o => o.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)
            );
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserEntity>().ToTable("User");

            modelBuilder.Entity<UserFriendEntity>()
            .HasKey(uf => new { uf.User1Id, uf.User2Id });

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.User1)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.User2)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFriendEntity>().ToTable("UserFriends");

            modelBuilder.Entity<ChatRoomEntity>()
            .HasKey(cr => cr.ChatRoomId);

            modelBuilder.Entity<ChatRoomEntity>().ToTable("ChatRooms");

            modelBuilder.Entity<ChatRoomTextMessageEntity>().HasKey(crt => crt.ChatRoomTextMessageId);
            modelBuilder.Entity<ChatRoomTextMessageEntity>()
            .HasOne(crt => crt.Sender)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoomTextMessageEntity>().ToTable("ChatRoomTextMessages");
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserFriendEntity> UserFriends { get; set; }
        public DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public DbSet<ChatRoomTextMessageEntity> ChatRoomTextMessages { get; set; }
    }
}