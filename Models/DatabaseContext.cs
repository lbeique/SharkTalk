using Microsoft.EntityFrameworkCore;

namespace SharkTalk.Models
{
  public class DatabaseContext : DbContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      modelBuilder.Entity<Channel>()
        .Property(e => e.Created)
        .HasDefaultValueSql("now()");

      modelBuilder.Entity<Message>()
        .Property(e => e.Created)
        .HasDefaultValueSql("now()");

      modelBuilder.Entity<User>()
        .Property(e => e.Created)
        .HasDefaultValueSql("now()");

      modelBuilder.Entity<User>()
        .HasIndex(e => e.Name)
        .IsUnique();

      modelBuilder.Entity<ChannelUser>()
        .HasKey(uc => new { uc.UserId, uc.ChannelId });

      modelBuilder.Entity<ChannelUser>()
        .HasOne(uc => uc.User)
        .WithMany(u => u.ChannelUsers)
        .HasForeignKey(uc => uc.UserId);

      modelBuilder.Entity<ChannelUser>()
        .HasOne(uc => uc.Channel)
        .WithMany(c => c.ChannelUsers)
        .HasForeignKey(uc => uc.ChannelId);

    }

    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ChannelUser> ChannelUsers => Set<ChannelUser>();
  }
}