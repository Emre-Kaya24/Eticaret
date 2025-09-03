using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    [Authorize]
    public class MyAddressesController : Controller
    {
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Address> _serviceAddress;

        public MyAddressesController(IService<AppUser> service, IService<Address> serviceAddress)
        {
            _serviceAppUser = service;
            _serviceAddress = serviceAddress;
        }
        public async Task<IActionResult> Index()
        {
            var appuser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appuser == null)
            {
                return NotFound("Kullanıcı Datası bulunamadı! Oturumunuzu Kapatıp Lütfen Tekrar Giriş Yapınız.");
            }
            var model = await _serviceAddress.GetAllAsync(u => u.AppUserId == appuser.Id);
            return View(model);
        }
    }
}
