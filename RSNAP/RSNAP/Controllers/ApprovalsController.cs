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
using System;
using RSNAP.EFModels;
using System.Data;
using RSNAP.Helper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace RSNAP.Controllers
{
    [Authorize]
    public class ApprovalsController : BaseController
    {
        private readonly IFMUtilityConfigService _configService;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly RSNAPContext _context;
        private IMemoryCache _cache;

        public ApprovalsController(IFMUtilityConfigService configService, ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> sharedLocalizer, RSNAPContext context, IMemoryCache memoryCache)
        {
            _configService = configService;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            FillSessionInfo();

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

            
             ViewData["Role"] = _RoleText;
            ViewData["RoleText"] =string.IsNullOrEmpty(_ROLE) ? "": $" as { _ROLE.ToLower()}";
            return View(model);
        }

        [HttpPost]
        public IActionResult Approvals_Read([DataSourceRequest] DataSourceRequest request, SearchDto searchModel)
        {
            var list = new List<ApprovalsModel>();
            
            FillSessionInfo();
            
            List<PagerCount> pagerCount = _context.Set<PagerCount>().FromSqlRaw("call Get_ROList(@PageIndex, @PageSize,@PopStartDate,@PopEndDate,@PIID,@IDV,@PDocNo,@VendorName,@FoApprovalStatus,@CoApprovalStatus,@NotificationStatus,@isDefaultList,@Role,@SortBy,@SortDirection,@IdList,@isCount)"
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
               , new MySqlParameter("@isDefaultList", searchModel.IsPostBack)
               , new MySqlParameter("@Role", _RoleText)
               , new MySqlParameter("@SortBy", request.Sorts.Count !=0 ? request.Sorts.FirstOrDefault().Member:null )
               , new MySqlParameter("@SortDirection", request.Sorts.Count != 0 ? request.Sorts.FirstOrDefault().SortDirection.ToString() : null)
               , new MySqlParameter("@IdList", IdListStringHelper(searchModel.IdList))
               , new MySqlParameter("@isCount", true)).ToList();

            list = _context.Set<ApprovalsModel>().FromSqlRaw("call Get_ROList(@PageIndex, @PageSize,@PopStartDate,@PopEndDate,@PIID,@IDV,@PDocNo,@VendorName,@FoApprovalStatus,@CoApprovalStatus,@NotificationStatus,@isDefaultList,@Role,@SortBy,@SortDirection,@IdList,@isCount)"
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
               , new MySqlParameter("@isDefaultList", searchModel.IsPostBack)
               , new MySqlParameter("@Role", _RoleText)
              , new MySqlParameter("@SortBy", request.Sorts.Count != 0 ? request.Sorts.FirstOrDefault().Member : null)
               , new MySqlParameter("@SortDirection", request.Sorts.Count != 0 ? request.Sorts.FirstOrDefault().SortDirection.ToString() : null)
               , new MySqlParameter("@IdList", IdListStringHelper(searchModel.IdList))
               , new MySqlParameter("@isCount", false)).ToList();

            DataSourceResult data = new DataSourceResult();
            data.Data = list;
            data.Total = pagerCount.FirstOrDefault().CountNum;
            return Json(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">ProActId list</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApproveProcess(List<ProcessModel> modes)
        {
            try
            {
                FillSessionInfo();
                
                AddComments(modes);
                _context.SaveChanges();
                int needNewSEQ=0;
                List<string> ids = modes.Select(x => x.Id).ToList();
                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (_RoleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        if (pendingroAction.FundApproverNote!= "Approval by the Funding Officer certifies that funds have been certified as available and correct for the corresponding contracts")
                        {
                            needNewSEQ++;
                        }
                        pendingroAction.FundApprovalStatus = "Approved";
                        pendingroAction.FundApprover = _Name;
                    }
                    if (_RoleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            if (pendingroAction.ContractingApproverNote != "Approval by the Administrative Contracting Officer authorizes the Budget Analyst to release the incremental fund amount(s) to the Contractor for the corresponding contracts")
                            {
                                needNewSEQ++;
                            }
                            pendingroAction.ContractingApprovalStatus = "Approved";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }

                

                int count = _context.SaveChanges();
                
                // Because the modification will produce a log record, so the number of lines display here is less than the actual one. So it is divided by two here.
                // casue by trg_pendingro_actions_update trg_pendingro_actions_insert
                return Json(pendingROActions.Count.ToString() + " record(s) approved by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record approved by " + _ROLE);
            }

        }

        [HttpPost]
        public JsonResult NnapprovedProcess(List<ProcessModel> modes)
        {
            try
            {
                FillSessionInfo();
                AddComments(modes);
                _context.SaveChanges();
                List<string> ids = modes.Select(x => x.Id).ToList();


                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (_RoleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        // When ACO Approval Status is "Approved",change Status to Under Review
                        if (pendingroAction.ContractingApprovalStatus.ToUpper() == "APPROVED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                        }
                        pendingroAction.FundApprovalStatus = "Not Approved";
                        pendingroAction.FundApprover = _Name;

                    }
                    if (_RoleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Not Approved";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }

                //AddComments(modes);

                int count = _context.SaveChanges();
                // Because the modification will produce a log record, so the number of lines display here is less than the actual one. So it is divided by two here.
                // casue by trg_pendingro_actions_update trg_pendingro_actions_insert
                return Json(pendingROActions.Count.ToString() + " record(s) unapproved by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record unapproved by " + _ROLE);
            }

        }

        [HttpPost]
        public JsonResult UnderReviewProcess(List<ProcessModel> modes)
        {
            try
            {
                FillSessionInfo();
                AddComments(modes);
                _context.SaveChanges();
                List<string> ids = modes.Select(x => x.Id).ToList();


                var pendingROActions = _context.PendingroActions.Where(x => ids.Contains(x.ProActId)).ToList();
                foreach (var pendingroAction in pendingROActions)
                {
                    if (_RoleText == "FO" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                    {
                        // When ACO Approval Status is "Approved",change Status to Under Review
                        if (pendingroAction.ContractingApprovalStatus.ToUpper() == "APPROVED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                        }
                        pendingroAction.FundApprovalStatus = "Under Review";
                        pendingroAction.FundApprover = _Name;

                    }
                    if (_RoleText == "CO")
                    {
                        if (pendingroAction.FundApprovalStatus.ToUpper() == "APPROVED" && pendingroAction.NotificationStatus.ToUpper() == "NOT GENERATED")
                        {
                            pendingroAction.ContractingApprovalStatus = "Under Review";
                            pendingroAction.ContractingApprover = _Name;
                        }

                    }
                }

                int count = _context.SaveChanges();
                // Because the modification will produce a log record, so the number of lines display here is less than the actual one. So it is divided by two here.
                // casue by trg_pendingro_actions_update trg_pendingro_actions_insert
                return Json(pendingROActions.Count.ToString() + " record(s) under review by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No record under review by " + _ROLE);
            }

        }

        [HttpPost]
        public JsonResult SaveComments(List<ProcessModel> modes)
        {
            try
            {
                FillSessionInfo();
                int commentedRows = 0;
                if (_RoleText == "RO")
                {
                    AddComments(modes);
                }
                commentedRows=_context.SaveChanges();
                commentedRows = commentedRows / 6;
                return Json(commentedRows.ToString() + " comment(s) added by " + _ROLE);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json("No comment added by " + _ROLE);
            }



        }

        private int AddComments(List<ProcessModel> modes)
        {
            FillSessionInfo();
            int commentLogCount = 0;
            foreach (var item in modes)
            {
                if (item.NewComments != null && item.NewComments.Trim() != string.Empty)
                {
                    List<SeqModel> seqModels= _context.Set<SeqModel>().FromSqlRaw("call GetCommentLogSeq()").ToList();
                    PendingroCommentLog pendingroCommentLog = new PendingroCommentLog();
                    pendingroCommentLog.ProCommentId = seqModels.FirstOrDefault().Id;
                    pendingroCommentLog.ProId = item.ProId;
                    pendingroCommentLog.UserId = _Name;
                    pendingroCommentLog.CommentDate = DateTime.Now;
                    pendingroCommentLog.ProComment = item.NewComments;
                    _context.PendingroCommentLog.Add(pendingroCommentLog);
                    commentLogCount++;
                }
            }
            return commentLogCount;
        }

        private string IdListStringHelper(List<ProcessModel> IdList)
        {
            string str = null;
            if (IdList == null || IdList.Count == 0)
            {
                str = null;
            }
            else
            {
                str= string.Join(',', IdList.Select(x => x.Id).ToList());
            }
            return str;

        }
        [HttpPost]
        public JsonResult ExportExcelData(SearchDto searchModel)
        {
            FillSessionInfo();
            
            string sortStr = null;

            switch (searchModel.Dir)
            {
                case "asc":
                    sortStr= "Ascending";
                    break;

                case "desc":
                    sortStr = "Descending";
                    break;
            }

            List <ExcelDataModel> list = new List<ExcelDataModel>();
            
            list = _context.Set<ExcelDataModel>().FromSqlRaw("call Get_ROListForExcel(@PopStartDate,@PopEndDate,@PIID,@IDV,@PDocNo,@VendorName,@FoApprovalStatus,@CoApprovalStatus,@NotificationStatus,@isDefaultList,@Role,@SortBy,@SortDirection,@IdList)"

               , new MySqlParameter("@PopStartDate", searchModel.ScheduledStartDate)
               , new MySqlParameter("@PopEndDate", searchModel.ScheduledEndDate)
               , new MySqlParameter("@PIID", searchModel.PIID)
               , new MySqlParameter("@IDV", searchModel.IDVContractNumber)
               , new MySqlParameter("@PDocNo", searchModel.PDN)
               , new MySqlParameter("@VendorName", searchModel.VendorName)
               , new MySqlParameter("@FoApprovalStatus", searchModel.FOApprovalStatus)
               , new MySqlParameter("@CoApprovalStatus", searchModel.ACOApprovalStatus)
               , new MySqlParameter("@NotificationStatus", searchModel.NotificationStatus)
               , new MySqlParameter("@isDefaultList", searchModel.IsPostBack)
               , new MySqlParameter("@Role", _RoleText)
                , new MySqlParameter("@SortBy", searchModel.Field)
               , new MySqlParameter("@SortDirection", sortStr)
               , new MySqlParameter("@IdList", IdListStringHelper(searchModel.IdList))).ToList();

            DataTable dtSource = DataTableExtend.ToDataTable<ExcelDataModel>(list);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");

            IRow dataRow = sheet1.CreateRow(0);
            foreach (DataColumn column in dtSource.Columns)
            {
                dataRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            }



            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                dataRow = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    dataRow.CreateCell(j).SetCellValue(dtSource.Rows[i][j].ToString());
                }
            }
            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            var ms = new MemoryStream(memoryStream.ToArray());
            var uuidN = Guid.NewGuid().ToString("N");
            _cache.Set(uuidN, ms.ToArray());

            return Json(new { Success = true, Id = uuidN });


        }

        public FileResult GetExcel(string Id)
        {
            var cacheByte = _cache.Get<byte[]>(Id);
            var excel = new MemoryStream(cacheByte);
            return File(excel, "application/ms-excel", string.Concat("RSNAP_", DateTime.Now.ToShortDateString(), ".xls"));
        }

    }
}
