using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TCECPortal.Infrastructure.Extensions;
using TCECPortal.Models;
using TCECPortal.Services;

namespace TCECPortal.Controllers
{
    public class UserProfile : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<UserProfile> _logger;
        UserModel user;
        public UserProfile(ILogger<UserProfile> logger
            , IRequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;

            user = new UserModel();
        }

        public IActionResult Index()
        {
            user = HttpContext.Session.GetObject<UserModel>("USER_DETAILS");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserModel model, int isAdmin = 0)
        {
            if (isAdmin == 0)
            {
                user = HttpContext.Session.GetObject<UserModel>("USER_DETAILS");

                model.UserId = user.UserId;
            }

            var register = await _requestService.PatchAsync("api/user", model, "", null);

            if (register != null)
            {
                TempData["AlertMessage"] = "Successfully updated!";

                return RedirectToAction("Profile", "UserProfile", model);
            }

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(UserModel model)
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View("Contacts");
        }

        [HttpPost]
        public async Task<IActionResult> AjaxMethod()
        {
            var register = await _requestService.GetAsync<List<UserModel>>("api/user", "", null);

            var json = Json(register.ToList());

            return json;
        }

        public async Task<IActionResult> Profile(int userId)
        {
            var register = await _requestService.GetAsync<UserModel>(string.Format("api/user/{0}", userId), "", null);

            return View("Profile", register);
        }
    }
}
