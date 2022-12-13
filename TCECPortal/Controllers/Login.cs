using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCECPortal.Infrastructure.Extensions;
using TCECPortal.Models;
using TCECPortal.Services;

namespace TCECPortal.Controllers
{
    public class Login : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<Login> _logger;
        public Login(ILogger<Login> logger
            , IRequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel login)
        {
            UserModel user = new UserModel();
            user.Email = login.Email;

            ScryptEncoder encoder = new ScryptEncoder();

            var register = await _requestService.PostAsync("api/user/getemail", user, "", null);

            if(register != null)
            {
                if(encoder.Compare(login.Pssword, register.Pssword))
                {
                    HttpContext.Session.SetObject("USER_DETAILS", register);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Incorrect login";

            return View();
        } 
        public IActionResult ExternalLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            string email = string.Empty;
            string facebookId = string.Empty;

            foreach (var claim in claims)
            {
                if (claim.Type.Contains("emailaddress"))
                {
                    email = claim.Value.ToString();
                }
                if (claim.Type.Contains("nameidentifier"))
                {
                    facebookId = claim.Value.ToString();
                }
            }

            UserModel user = new UserModel();
            user.Email = email;

            //if already existing redirect to Dashboard
            var getUserDetails = await _requestService.PostAsync("api/user/getemail", user, "", null);
            if (getUserDetails != null)
            {
                if(getUserDetails.FacebookId == "" || getUserDetails.FacebookId == null)
                {
                    //Update user profile
                    user = new UserModel();
                    user.UserId = getUserDetails.UserId;
                    user.FacebookId = facebookId;
                    var update = await _requestService.PatchAsync("api/user", user, "", null);

                    HttpContext.Session.SetObject("USER_DETAILS", update);

                    return RedirectToAction("Index", "Home");
                }

                HttpContext.Session.SetObject("USER_DETAILS", getUserDetails);

                return RedirectToAction("Index", "Home");

            }

            //if not update user
            return RedirectToAction("Index", "UserRegistration");
             
        }
    }
}
