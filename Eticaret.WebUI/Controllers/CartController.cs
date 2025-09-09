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

        public CartController(IService<Product> serviceProduct, IService<Address> serviceAddress, IService<AppUser> serviceAppUser)
        {
            _serviceProduct = serviceProduct;
            _serviceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
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

        [Authorize,HttpPost]
        public async Task<IActionResult> Checkout(string CardNumber, string CardMonth, string CardYear, string CVV, string Addresses, string BillingAddress)
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

            if(string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardMonth) || string.IsNullOrWhiteSpace(CardYear) || string.IsNullOrWhiteSpace(CVV) || string.IsNullOrWhiteSpace(Addresses)|| string.IsNullOrWhiteSpace(BillingAddress))
            {
                return View(model);
            }
            var TeslimatAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == Addresses);
            var FaturaAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == BillingAddress);

            return View(model);
        }


        private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();

        }

    }
}
