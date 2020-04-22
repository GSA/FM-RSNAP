using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSNAP.Models
{
    public class ApprovalsModel
    {
        [NotMapped]
        public int Id { get; set; }
        public string ProID { get; set; }
        public string ProActID { get; set; }
        public string ScheduledStartDate { get; set; }
        public string ScheduledEndDate { get; set; }
        public string OrderNumber { get; set; }
        public string IDVContractNumber { get; set; }
        public string PDN { get; set; }
        public string VendorName { get; set; }
        public string FOApprovalStatus { get; set; }
        public string ACOApprovalStatus { get; set; }
        public string NotificationStatus { get; set; }
        public string ScheduleDate { get; set; }
        public string Amount { get; set; }
        public string VendorEmail { get; set; }
        public string COEmail { get; set; }
        public string FundingOfficer { get; set; }
        
        public string FONote { get; set; }
        public string ContractingOfficer { get; set; }
        public bool CheckboxStatus { get; set; }
        public string Pop { set; get; }
        [NotMapped]
        public string Comments { set; get; }
        [NotMapped]
        public string NewComments { set; get; }

        public string CONote { get; set; }

        public List<SelectListItem> FOApprovalStatusAvailable { get; set; }
        public List<SelectListItem> ACOApprovalStatusAvailable { get; set; }
        public List<SelectListItem> NotificationStatusAvailable { get; set; }       
    }
}