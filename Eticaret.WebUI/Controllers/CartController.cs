using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Address> _serviceAddress;
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Order> _serviceOrder;

        public CartController(IService<Product> serviceProduct, IService<Address> serviceAddress, IService<AppUser> serviceAppUser, IService<Order> serviceOrder)
        {
            _serviceProduct = serviceProduct;
            _serviceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
            _serviceOrder = serviceOrder;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var model = new CartViewModel()
            {
                CartLines = cart.CartLines,
                TotalPrice = cart.TotalPrice()
            };
            return View(model);
        }
        public IActionResult Add(int ProductId, int quantitiy = 1)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product, quantitiy);
                HttpContext.Session.SetJson("Cart", cart);
                //kullanıcı sepete ürün ekledikten sonra Anasayfada kalmasını sağlıyor 
                return Redirect(Request.Headers["Referer"].ToString());
            }

            return RedirectToAction("Index");
        }
        public IActionResult Update(int ProductId, int quantitiy = 1)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product, quantitiy);
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }
        public IActionResult Remove(int ProductId)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            var appuser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appuser == null) 
            {
                return RedirectToAction("Index", "Account");
            }
            var addresses = await _serviceAddress.GetAllAsync(a => a.AppUserId == appuser.Id && a.IsActive);
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };
            return View(model);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Checkout(string CardNumber, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
        {
            var cart = GetCart();
            var appuser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appuser == null)
                return RedirectToAction("Index", "Account");

            var addresses = await _serviceAddress.GetAllAsync(a => a.AppUserId == appuser.Id && a.IsActive);
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };

            // Form doğrulaması
            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardMonth) || string.IsNullOrWhiteSpace(CardYear) || string.IsNullOrWhiteSpace(CVV) ||string.IsNullOrWhiteSpace(DeliveryAddress) || string.IsNullOrWhiteSpace(BillingAddress))
            {
                TempData["Message"] = "Lütfen tüm alanları doldurun!";
                return View(model);
            }

            var TeslimatAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == DeliveryAddress);
            var FaturaAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == BillingAddress);

            // Yeni sipariş oluştur
            var siparis = new Order
            {
                AppUserId = appuser.Id,
                CustomerId = appuser.UserGuid.ToString(),
                BillingAddress = FaturaAdresi != null ? $"{FaturaAdresi.OpenAddress} {FaturaAdresi.District}/{FaturaAdresi.City}" : BillingAddress,
                DeliveryAddress = TeslimatAdresi != null ? $"{TeslimatAdresi.OpenAddress} {TeslimatAdresi.District}/{TeslimatAdresi.City}" : DeliveryAddress,
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice(),
                OrderNumber = Guid.NewGuid().ToString(),
                OrderLines = cart.CartLines.Select(item => new OrderLine
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList()
            };

            try
            {
                
                await _serviceOrder.AddAsync(siparis);
                var sonuc = await _serviceOrder.SaveChangesAsync();

                if (sonuc > 0)
                {
                    // Sepeti temizle
                    HttpContext.Session.Remove("Cart");
                    return RedirectToAction("Thanks");
                }
                else
                {
                    TempData["Message"] = "Sipariş kaydedilemedi!";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Hata Oluştu";
            }

            return View(model);
        }



        public IActionResult Thanks()
        {       
            return View();
        }


        private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();

        }

    }
}
