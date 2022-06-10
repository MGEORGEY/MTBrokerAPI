using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTBrokerAPI.Model
{
    public class MT940
    {
        public int ID { get; set; }


        [Required]
        public string AccountID25 { get; set; }


        [Required]
        public string FinBranchCode { get; set; }


        [Required]
        public string FinLTCode { get; set; }


        [Required]
        public string FinSwiftAddress { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal OpeningBalance60FAmount { get; set; }


        [Required]
        public string OpeningBalance60FCurrency { get; set; }


        [Required]
        public DateTime OpeningBalance60FDate { get; set; }


        [Required]
        public string OpeningBalance60FDOrC { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal ClosingBalance62FAmount { get; set; }


        [Required]
        public string ClosingBalance62FCurrency { get; set; }


        [Required]
        public DateTime ClosingBalance62FDate { get; set; }


        [Required]
        public string ClosingBalance62FDOrC { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal AvailableBalance64Amount { get; set; }


        [Required]
        public string AvailableBalance64Currency { get; set; }


        [Required]
        public DateTime AvailableBalance64Date { get; set; }


        [Required]
        public string AvailableBalance64DOrC { get; set; }


        [Required]
        public string SendersSwiftAddress { get; set; }


        [Required]
        public string SequenceNumber { get; set; }


        [Required]
        public string SessionNumber { get; set; }


        [Required]
        public string StatementOrSeqNo28CMsgSeq { get; set; }


        [Required]
        public string StatementOrSeqNo28CStmntSeq { get; set; }


        [Required]
        public string TransactionRefNo20 { get; set; }


    }
}
