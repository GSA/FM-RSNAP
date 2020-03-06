using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace RSNAP.Models
{
    public class ApprovalsModel
    {
        public int Id { get; set; }
        public string ScheduledStartDate { get; set; }
        public string ScheduledEndDate { get; set; }
        public string OrderNumber { get; set; }
        public string IDVContractNumber { get; set; }
        public string PDN { get; set; }
        public string VendorName { get; set; }
        public string FOApprovalStatus { get; set; }
        public string ACOApprovalStatus { get; set; }
        public string NotificationStatus { get; set; }

        public List<SelectListItem> FOApprovalStatusAvailable { get; set; }
        public List<SelectListItem> ACOApprovalStatusAvailable { get; set; }
        public List<SelectListItem> NotificationStatusAvailable { get; set; }       
    }
}