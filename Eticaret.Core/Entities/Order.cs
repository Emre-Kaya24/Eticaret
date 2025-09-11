using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Order : IEntitiy
    {
        public int Id { get; set; }
        [Display(Name="Sipariş Numarası"),StringLength(50)]
        public string OrderNumber { get; set; }
        [Display(Name = "Sipariş Toplamı")]
        public Decimal TotalPrice { get; set; }
        [Display(Name = "Müşteri Numarası")]
        public int AppUserId { get; set; }
        [Display(Name = "Müşteri"), StringLength(50)]
        public string CustomerId { get; set; }
        [Display(Name = "Fatura Adresi"), StringLength(50)]
        public string BillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi"), StringLength(50)]
        public string DeliveryAddress { get; set; }
        [Display(Name = "Sipariş Tarihi")]
        public DateTime OrderDate { get; set; }
        public List<OrderLine> OrderLines { get; set; }  = new List<OrderLine>();
        [Display(Name = "Müşteri")]
        public AppUser? AppUser { get; set; }

        [Display(Name = "Sipariş Durumu")]
        public EnumOrderState OrderState { get; set; }

    }
    public enum EnumOrderState
    {
        [Display(Name = "Onay Bekliyor")]
        waiting,
        [Display(Name = "Onaylandı")]
        Approved, 
        [Display(Name = "Kargoya Verildi.")]
        Shipped, 
        [Display(Name = "Tamamlandı.")]
        completed, 
        [Display(Name = "İptal Edildi.")]
        canceled,
        [Display(Name = "İade Edildi.")]
        refunded
    }

}
