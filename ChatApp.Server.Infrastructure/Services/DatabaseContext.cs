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
            .HasKey(uf => new { uf.User1Id, uf.User2Id });

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.User1)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFriendEntity>()
            .HasOne(uf => uf.User2)
            .WithMany()
            .IsRequired()
            .HasForeignKey(uf => uf.User2Id)
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

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<UserFriendEntity>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (String.Compare(entry.Entity.User1Id, entry.Entity.User2Id) > 0)
                    {
                        (entry.Entity.User1Id, entry.Entity.User2Id) = (entry.Entity.User2Id, entry.Entity.User1Id);
                    }
                }
            }

            return base.SaveChanges();
        }

        public DbSet<UserFriendEntity> UserFriends { get; set; }
        public DbSet<ChatRoomEntity> ChatRooms { get; set; }
        public DbSet<ChatRoomTextMessageEntity> ChatRoomTextMessages { get; set; }
    }
}