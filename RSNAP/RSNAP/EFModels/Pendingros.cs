using System;
using System.Collections.Generic;

namespace RSNAP.EFModels
{
    public partial class Pendingros
    {
        public string ProId { get; set; }
        public string Pdocno { get; set; }
        public string CtrcNum { get; set; }
        public decimal? OrddAm { get; set; }
        public DateTime? LastGnrdDt { get; set; }
        public DateTime NextExpdDt { get; set; }
        public string AddrNm { get; set; }
        public string VendorEmail { get; set; }
        public string CoEmail { get; set; }
        public DateTime? PopStdt { get; set; }
        public DateTime? PopEndt { get; set; }
        public DateTime? ContractEfctStdt { get; set; }
        public DateTime? ContractEfctEndt { get; set; }
        public string Region { get; set; }
        public DateTime? MaterialChangeDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string EtlUpdateFl { get; set; }
        public string MfIoFrmUidy { get; set; }
        public string Idv { get; set; }
        public string Piid { get; set; }
    }
}
