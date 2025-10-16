using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Eticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected DatabaseContext()
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        
        // Veri Tabanı bağlantı ayarını yaptığımız yer => OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //local
            //optionsBuilder.UseSqlServer(@"Server=DESKTOP-LGO92CF; Database=EticaretDb; Trusted_Connection=True; TrustServerCertificate=True;");
            
            //Server
            //optionsBuilder.UseSqlServer(@"workstation id=PumPcTicaret.mssql.somee.com;packet size=4096;user id=emrekaya_SQLLogin_2;pwd=bey3hih9lf;data source=PumPcTicaret.mssql.somee.com;persist security info=False;initial catalog=PumPcTicaret;TrustServerCertificate=True");


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

            modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderLines)
            .WithOne(ol => ol.Order)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // Order silindiğinde OrderLine’lar da silinsin

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
