using System;
using System.Collections.Generic;

namespace RSNAP.EFModels
{
    public partial class PendingroApprovalLog
    {
        public string ProApprId { get; set; }
        public string ProId { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionTaken { get; set; }
        public string FundApprover { get; set; }
        public string ContractingApprover { get; set; }
        public string Note { get; set; }
    }
}
