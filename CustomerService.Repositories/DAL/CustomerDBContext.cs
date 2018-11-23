using Microsoft.EntityFrameworkCore;
using CustomerService.Repositories.Entities;

namespace CustomerService.Repositories.DAL
{
    public class CustomerDBContext : DbContext, ICustomerDBContext
    {
        public CustomerDBContext(DbContextOptions<CustomerDBContext> options) : base(options)
        {
        }

        public DbSet<Token> Token { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Session> Session { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(t => t.Id).HasColumnName("Id");
                b.Property(t => t.Email).HasColumnName("Email");
                b.Property(t => t.Name).HasColumnName("Name");
                b.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
                b.Property(t => t.ActivationCode).HasColumnName("ActivationCode");
                b.Property(t => t.IsActive).HasColumnName("IsActive");
                b.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
                b.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
                b.ToTable("Client");
            });

            builder.Entity<Session>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(t => t.Id).HasColumnName("Id");
                b.Property(t => t.ClientId).HasColumnName("ClientId");
                b.Property(t => t.SessionKey).HasColumnName("SessionKey");
                b.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
                b.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
                b.Property(t => t.ExpiredDate).HasColumnName("ExpiredDate");
                b.Property(t => t.Confirmed).HasColumnName("Confirmed");
                b.Property(t => t.Enabled).HasColumnName("Enabled");
                b.Property(t => t.IP).HasColumnName("IP");
                b.ToTable("Session");
            });

            builder.Entity<Token>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(t => t.Id).HasColumnName("Id");
                b.Property(t => t.ClientId).HasColumnName("ClientId");
                b.Property(t => t.Value).HasColumnName("Value");
                b.Property(t => t.AuthenticationMethod).HasColumnName("AuthenticationMethod");
                b.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
                b.Property(t => t.IsActive).HasColumnName("IsActive");
                b.Property(t => t.IP).HasColumnName("IP");
                b.ToTable("Token");
            });
        }
    }
}
