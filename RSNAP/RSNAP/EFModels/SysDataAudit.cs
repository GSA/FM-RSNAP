using System;
using System.Collections.Generic;

namespace RSNAP.EFModels
{
    public partial class SysDataAudit
    {
        public int SysDataAuditId { get; set; }
        public string ExternalUserId { get; set; }
        public string SystemOfRecord { get; set; }
        public string ChangingTable { get; set; }
        public string ChangingColumn { get; set; }
        public string RecordPk { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
