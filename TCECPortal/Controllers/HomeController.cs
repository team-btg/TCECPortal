using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TCECPortal.Models;
using TCECPortal.Services;

namespace TCECPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger
            , IRequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;
        }

        public async Task<IActionResult> Index()
        {
            var register = await _requestService.GetAsync<IEnumerable<UserModel>>("api/user", "", null);

            return View(register);
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
