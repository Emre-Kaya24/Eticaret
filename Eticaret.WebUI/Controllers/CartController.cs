using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Core.Entities.Address> _serviceAddress;
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Order> _serviceOrder;
        private readonly IConfiguration _configuration;

        public CartController(IService<Product> serviceProduct, IService<Core.Entities.Address> serviceAddress, IService<AppUser> serviceAppUser, IService<Order> serviceOrder, IConfiguration configuration)
        {
            _serviceProduct = serviceProduct;
            _serviceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
            _serviceOrder = serviceOrder;
            _configuration = configuration;
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
        public async Task<IActionResult> Checkout(string CardNameSurname,string CardNumber, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
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
                OrderState = 0,
                OrderLines = cart.CartLines.Select(item => new OrderLine
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList()
            };

            #region OdemeIslemi
            Options options = new Options();
            options.ApiKey = _configuration["IyzicoOptions:ApiKey"];
            options.SecretKey = _configuration["IyzicoOptions:SecretKey"];
            options.BaseUrl = _configuration["IyzicoOptions:BaseUrl"];

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = HttpContext.Session.Id;
            request.Price = siparis.TotalPrice.ToString().Replace(",", ".");
            request.PaidPrice = siparis.TotalPrice.ToString().Replace(",", ".");
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B"+HttpContext.Session.Id;
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            // Kart Bilgileri Dinamik olarak Alınıyor
            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = CardNameSurname; //"John Doe";
            paymentCard.CardNumber = CardNumber;//"5528790000000008";
            paymentCard.ExpireMonth = CardMonth;//"12";
            paymentCard.ExpireYear = CardYear;//"2030";
            paymentCard.Cvc = CVV;//"123";
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY"+ appuser.Id;
            buyer.Name = appuser.Name;
            buyer.Surname = appuser.Surname;
            buyer.GsmNumber = appuser.Phone;
            buyer.Email = appuser.Email;
            buyer.IdentityNumber = "11111111111";
            buyer.LastLoginDate = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss"); //"2015-10-05 12:43:35";
            buyer.RegistrationDate = appuser.CreateDate.ToString("yyyy-mm-dd hh:mm:ss"); //"2013-04-21 15:12:09";
            buyer.RegistrationAddress = siparis.DeliveryAddress;
            buyer.Ip = HttpContext.Connection.RemoteIpAddress?.ToString(); //"85.34.78.112";
            buyer.City = TeslimatAdresi.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            var shippingAddress = new Iyzipay.Model.Address();
            shippingAddress.ContactName = appuser.Name + " " +appuser.Surname;
            shippingAddress.City = TeslimatAdresi.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = TeslimatAdresi.OpenAddress;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            var billingAddress = new Iyzipay.Model.Address();
            billingAddress.ContactName = appuser.Name + " " + appuser.Surname;
            billingAddress.City = FaturaAdresi.City;
            billingAddress.Country = "Turkey";
            billingAddress.Description = FaturaAdresi.OpenAddress;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            //BasketItem firstBasketItem = new BasketItem();
            //firstBasketItem.Id = "BI101";
            //firstBasketItem.Name = "Binocular";
            //firstBasketItem.Category1 = "Collectibles";
            //firstBasketItem.Category2 = "Accessories";
            //firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            //firstBasketItem.Price = "0.3";
            //basketItems.Add(firstBasketItem);

            foreach (var item in cart.CartLines)
            {
                basketItems.Add(new BasketItem
                {
                    Id = item.Product.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = "Collectibles",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Product.Price * item.Quantity).ToString().Replace(",",".")
                });
            }

                request.BasketItems = basketItems;

            Payment payment = await Payment.Create(request, options);
            
            #endregion

            try
            {
                if (payment.Status == "success")
                {
                    //sipariş oluştur
                    await _serviceOrder.AddAsync(siparis);
                    var sonuc = await _serviceOrder.SaveChangesAsync();

                    if (sonuc > 0)
                    {
                        // Sepeti temizle
                        HttpContext.Session.Remove("Cart");
                        return RedirectToAction("Thanks");
                    }
                }

                
                else
                {
                    TempData["Message"] = $"<div class='alert alert-danger'>Ödeme İşlemi Başarısız!</div> ({payment.ErrorMessage})";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "<div class='alert alert-danger'>Hata Oluştu</div>";
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
