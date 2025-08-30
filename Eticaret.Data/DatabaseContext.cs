using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Eticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        
        // Veri Tabanı bağlantı ayarını yaptığımız yer => OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-LGO92CF; Database=EticaretDb; Trusted_Connection=True; TrustServerCertificate=True;");

            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new AppUserConfiguraiton());
            //modelBuilder.ApplyConfiguration(new BrandConfiguraiton());

            // tek tek classları eklemek yerine aşağıdaki kodla çalışan dll'in içinden bulacak.

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AppUser>()
            .Property(u => u.UserGuid)
            .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Brand>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

             modelBuilder.Entity<Category>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

             modelBuilder.Entity<Contact>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

             modelBuilder.Entity<News>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

             modelBuilder.Entity<Product>()
            .Property(u => u.CreateDate)
            .HasDefaultValueSql("GETDATE()");

            



        }

    }
}
