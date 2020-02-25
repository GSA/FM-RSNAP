using System;
using System.Collections.Generic;

namespace RSNAP.EFModels
{
    public partial class PendingroActions
    {
        public string ProActId { get; set; }
        public string ProId { get; set; }
        public string FundApprover { get; set; }
        public string FundApprovalStatus { get; set; }
        public string FundApproverNote { get; set; }
        public string ContractingApprover { get; set; }
        public string ContractingApprovalStatus { get; set; }
        public string ContractingApproverNote { get; set; }
        public string NotificationStatus { get; set; }
        public DateTime? NotificationDate { get; set; }
    }
}
