using System;
using System.Collections.Generic;

namespace RSNAP.Models
{
    public class SearchDto
    {
        public int? PIID { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public string OrderNumber { get; set; }
        public string IDVContractNumber { get; set; }
        public string PDN { get; set; }
        public string VendorName { get; set; }
        public string FOApprovalStatus { get; set; }
        public string ACOApprovalStatus { get; set; }
        public string NotificationStatus { get; set; }
        public bool IsPostBack { get; set; }
        public List<ProcessModel> IdList { get; set; }
        public string Dir { get; set; }
        public string Field { get; set; }

    }
}
