using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSNAP.Models;
using GSA.FM.Utility.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc.Rendering;
using RSNAP.EFData;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RSNAP.Controllers
{
    [Authorize]
    public class ApprovalsController : BaseControllerController
    {
        private readonly IFMUtilityConfigService _configService;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly RSNAPContext _context;

        public ApprovalsController(IFMUtilityConfigService configService, ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> sharedLocalizer, RSNAPContext context)
        {
            _configService = configService;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ApprovalsModel();

            model.FOApprovalStatusAvailable = new List<SelectListItem>();
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Not Reviewed", "Not Reviewed"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Under Review", "Under Review"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Approved", "Approved"));
            model.FOApprovalStatusAvailable.Add(new SelectListItem("Not Approved", "Not Approved"));
            model.FOApprovalStatusAvailable.Insert(0, new SelectListItem("Select Status", ""));

            model.ACOApprovalStatusAvailable = new List<SelectListItem>();
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Not Reviewed", "Not Reviewed"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Under Review", "Under Review"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Approved", "Approved"));
            model.ACOApprovalStatusAvailable.Add(new SelectListItem("Not Approved", "Not Approved"));
            model.ACOApprovalStatusAvailable.Insert(0, new SelectListItem("Select Status", ""));

            model.NotificationStatusAvailable = new List<SelectListItem>();
            model.NotificationStatusAvailable.Add(new SelectListItem("Not Generated", "Not Generated"));
            model.NotificationStatusAvailable.Add(new SelectListItem("Generated", "Generated"));
            model.NotificationStatusAvailable.Add(new SelectListItem("Missed", "Missed"));
            model.NotificationStatusAvailable.Add(new SelectListItem("CO Missed", "CO Missed"));
            model.NotificationStatusAvailable.Add(new SelectListItem("CO/Vendor Missed", "CO/Vendor Missed"));
            model.NotificationStatusAvailable.Insert(0, new SelectListItem("Select Status", ""));
            ViewData["total"] = 0;
            return View(model);
        }

        [HttpPost]
        public IActionResult Approvals_Read([DataSourceRequest] DataSourceRequest request, SearchDto searchModel)
        {
            var list = new List<ApprovalsModel>();
            
            FillSessionInfo();
            string roleText = null;
            if (_ROLE == RoleEnum.FO.GetDescription())
            {
                roleText = "FO";
            }
            else if (_ROLE == RoleEnum.CO.GetDescription()) 
            {
                roleText = "CO";
            }
            else if (_ROLE == RoleEnum.RO.GetDescription())
            {
                roleText = "RO";
            }

            List<PagerCount> pagerCount = _context.Set<PagerCount>().FromSqlRaw("call Get_ROList(@PageIndex, @PageSize,@PopStartDate,@PopEndDate,@PIID,@IDV,@PDocNo,@VendorName,@FoApprovalStatus,@CoApprovalStatus,@NotificationStatus,@isDefaultList,@Role,@isCount)"
               , new MySqlParameter("@PageIndex", request.Page)
               , new MySqlParameter("@PageSize", request.PageSize)
               , new MySqlParameter("@PopStartDate", searchModel.ScheduledStartDate)
               , new MySqlParameter("@PopEndDate", searchModel.ScheduledEndDate)
               , new MySqlParameter("@PIID", searchModel.PIID)
               , new MySqlParameter("@IDV", searchModel.IDVContractNumber)
               , new MySqlParameter("@PDocNo", searchModel.PDN)
               , new MySqlParameter("@VendorName", searchModel.VendorName)
               , new MySqlParameter("@FoApprovalStatus", searchModel.FOApprovalStatus)
               , new MySqlParameter("@CoApprovalStatus", searchModel.ACOApprovalStatus)
               , new MySqlParameter("@NotificationStatus", searchModel.NotificationStatus)
               , new MySqlParameter("@isDefaultList", searchModel.isDefaultList)
               , new MySqlParameter("@Role", roleText)
               , new MySqlParameter("@isCount", true)).ToList();
            list = _context.Set<ApprovalsModel>().FromSqlRaw("call Get_ROList(@PageIndex, @PageSize,@PopStartDate,@PopEndDate,@PIID,@IDV,@PDocNo,@VendorName,@FoApprovalStatus,@CoApprovalStatus,@NotificationStatus,@isDefaultList,@Role,@isCount)"
               , new MySqlParameter("@PageIndex", request.Page)
               , new MySqlParameter("@PageSize", request.PageSize)
               , new MySqlParameter("@PopStartDate", searchModel.ScheduledStartDate)
               , new MySqlParameter("@PopEndDate", searchModel.ScheduledEndDate)
               , new MySqlParameter("@PIID", searchModel.PIID)
               , new MySqlParameter("@IDV", searchModel.IDVContractNumber)
               , new MySqlParameter("@PDocNo", searchModel.PDN)
               , new MySqlParameter("@VendorName", searchModel.VendorName)
               , new MySqlParameter("@FoApprovalStatus", searchModel.FOApprovalStatus)
               , new MySqlParameter("@CoApprovalStatus", searchModel.ACOApprovalStatus)
               , new MySqlParameter("@NotificationStatus", searchModel.NotificationStatus)
               , new MySqlParameter("@isDefaultList", searchModel.isDefaultList)
               , new MySqlParameter("@Role", roleText)
               , new MySqlParameter("@isCount", false)).ToList();

            DataSourceResult data = new DataSourceResult();
            data.Data= list;
            data.Total = pagerCount.FirstOrDefault().CountNum;
            return Json(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">ProActId list</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApproveProcess(List<string> ids)
        {
            try
            {
                FillSessionInfo();
                string roleText = null;
                if (_ROLE == RoleEnum.FO.GetDescription())
                {
                    roleText = "FO";
                }
                else if (_ROLE == RoleEnum.CO.GetDescription())
                {
                    roleText = "CO";
                }

                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (roleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        pendingroAction.FundApprovalStatus = "Approved";
                        pendingroAction.FundApprover = _Name;
                    }
                    if (roleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Approved";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }
                int count = _context.SaveChanges();
                return Json(count.ToString() + " record(s) approved by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record approved by " + _ROLE);
            }
           
        }

        [HttpPost]
        public JsonResult NnapprovedProcess(List<string> ids)
        {
            try
            {
                FillSessionInfo();
                string roleText = null;
                if (_ROLE == RoleEnum.FO.GetDescription())
                {
                    roleText = "FO";
                }
                else if (_ROLE == RoleEnum.CO.GetDescription())
                {
                    roleText = "CO";
                }

                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (roleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        // When ACO Approval Status is "Approved",change Status to Under Review
                        if (pendingroAction.ContractingApprovalStatus.ToUpper() == "APPROVED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                        }
                        pendingroAction.FundApprovalStatus = "Not Approved";
                        pendingroAction.FundApprover = _Name;
                        
                    }
                    if (roleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Not Approved";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }
                int count = _context.SaveChanges();
                return Json(count.ToString() + " record(s) unapproved by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record unapproved by " + _ROLE);
            }

        }

        [HttpPost]
        public JsonResult UnderReviewProcess(List<string> ids)
        {
            try
            {
                FillSessionInfo();
                string roleText = null;
                if (_ROLE == RoleEnum.FO.GetDescription())
                {
                    roleText = "FO";
                }
                else if (_ROLE == RoleEnum.CO.GetDescription())
                {
                    roleText = "CO";
                }

                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (roleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        // When ACO Approval Status is "Approved",change Status to Under Review
                        if (pendingroAction.ContractingApprovalStatus.ToUpper() == "APPROVED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                        }
                        pendingroAction.FundApprovalStatus = "Under Review";
                        pendingroAction.FundApprover = _Name;

                    }
                    if (roleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }
                int count = _context.SaveChanges();
                return Json(count.ToString() + " record(s) under review by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record under review by " + _ROLE);
            }

        }
    }
}
