using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSNAP.Models;
using GSA.FM.Utility.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSNAP.Controllers
{
    [Authorize]
    public class ApprovalsController : Controller
    {
        private readonly IFMUtilityConfigService _configService;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public ApprovalsController(IFMUtilityConfigService configService, ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _configService = configService;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult Index()
        {
            var model = new ApprovalsModel();

            model.FOApprovalStatusAvailable = new List<SelectListItem>();
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Not Reviewed", "1"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Under Review", "2"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Approved", "3"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Not Approved", "4"));
            model.FOApprovalStatusAvailable.Insert(0, new SelectListItem("Select Status", "-1"));

            model.ACOApprovalStatusAvailable = new List<SelectListItem>();
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Not Reviewed", "1"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Under Review", "2"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Approved", "3"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Not Approved", "4"));
            model.ACOApprovalStatusAvailable.Insert(0, new SelectListItem("Select Status", "-1"));

            model.NotificationStatusAvailable = new List<SelectListItem>();
            model.NotificationStatusAvailable.Add(new SelectListItem("Not Generated", "1"));
            model.NotificationStatusAvailable.Add(new SelectListItem("Generated", "2"));
            model.NotificationStatusAvailable.Add(new SelectListItem("Missed", "3"));
            model.NotificationStatusAvailable.Add(new SelectListItem("CO Missed", "4"));
            model.NotificationStatusAvailable.Add(new SelectListItem("CO/Vendor Missed", "5"));
            model.NotificationStatusAvailable.Insert(0, new SelectListItem("Select Status", "-1"));

            return View(model);
        }


        public IActionResult Approvals_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var list = new List<ApprovalsModel>();

            //stubbed
            var i = 1;
            while (i < 100)
            {
                list.Add(new ApprovalsModel()
                {
                    ScheduledStartDate = "1/1/20",
                    ScheduledEndDate = "2/1/20",
                    OrderNumber = "12345",
                    IDVContractNumber = "1A",
                    PDN = "123",
                    VendorName = "Test Vendor",
                    FOApprovalStatus = "Approved",
                    ACOApprovalStatus = "Pending",
                    NotificationStatus = "Sent"
                });

                i++;
            }

            return Json(list.ToDataSourceResult(request));
        }
    }
}
