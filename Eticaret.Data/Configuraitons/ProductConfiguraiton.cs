using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configuraitons
{
    internal class ProductConfiguraiton : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150);
            builder.Property(x => x.Image).HasMaxLength(100);
            builder.Property(x => x.ProductCode).HasMaxLength(50);
        }
    }
}
