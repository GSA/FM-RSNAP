using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSNAP.Models;
using GSA.FM.Utility.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace RSNAP.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IFMUtilityConfigService _configService;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public HomeController(IFMUtilityConfigService configService, ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _configService = configService;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult Index()
        {
            var warning = HttpContext.Session.GetString("warning");
            if (string.IsNullOrEmpty(warning))
            {
                return RedirectToAction("Warning");
            }

            // Query app status from the config service.
            var closeMessage = _configService.GetAppClosed("RSNAP");
            var warningMessage = _configService.GetAppWarning("RSNAP");

            // Are we closed (via CAAM)?
            if (closeMessage != "OPEN")
            {
                ViewData["closeMessage"] = closeMessage;
            }

            // Do we have a warning (via CAAM)?
            if (warningMessage != "OPEN")
            {
                ViewData["warningMessage"] = warningMessage;
            }

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmWarning()
        {
            HttpContext.Session.SetString("warning", "true");
            return Json(true);
        }

        public IActionResult Warning()
        {
            return View();
        }

        [Authorize]
        public IActionResult LoggedIn()
        {
            FillSessionInfo();
            if (string.IsNullOrEmpty(_RoleText))
            {
                return Redirect("/");
            }
            ViewData["Role"] = _RoleText;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
