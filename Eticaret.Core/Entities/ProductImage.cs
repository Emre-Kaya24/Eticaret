using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class ProductImage : IEntitiy
    {
        public int Id { get; set; }
        [Display(Name = "Resim Adı"),StringLength(240)]
        public string? Name { get; set; }
        [Display(Name = "Resim Açıklama (Alt Tagı)"), StringLength(300)]
        public string? Alt { get; set; }
        [Display(Name = "Ürün")]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
