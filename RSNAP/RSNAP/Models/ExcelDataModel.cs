namespace RSNAP.Models
{
    public class ExcelDataModel
    {
        public string ScheduleDate { get; set; }
        public string OrderNumber { get; set; }
        public string IDVContractNumber { get; set; }
        public string PDN { get; set; }
        public string Pop { set; get; }
        public string Amount { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string COEmail { get; set; }
        public string FundingOfficer { get; set; }
        public string FOApprovalStatus { get; set; }
        public string FONote { get; set; }
        public string ContractingOfficer { get; set; }
        public string ACOApprovalStatus { get; set; }
        public string CONote { get; set; }
        public string NotificationStatus { get; set; }
    }
}
