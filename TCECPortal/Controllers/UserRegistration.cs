using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCECPortal.Models;
using TCECPortal.Services;
using Scrypt;

namespace TCECPortal.Controllers
{
    public class UserRegistration : Controller
    {
        private readonly IRequestService _requestService;
        public UserRegistration(IRequestService requestService)
        {
            _requestService = requestService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            ScryptEncoder encoder = new ScryptEncoder();

            var checkEmail = await _requestService.PostAsync("api/user/getemail", model, "", null);

            if (checkEmail != null)
            {
                ViewBag.Error = "Email address already exist.";
                
                return View("Index");

            } 

            model.Pssword = encoder.Encode(model.Pssword);

            var register = await _requestService.PostAsync("api/user", model, null);

            return RedirectToAction("Index", "Login");
        }
    }
}
