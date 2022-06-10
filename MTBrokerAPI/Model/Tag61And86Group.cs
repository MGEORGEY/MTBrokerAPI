using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTBrokerAPI.Model
{
    public class Tag61And86Group
    {
        public int ID { get; set; }



        [Required]
        public string AccOwnerInfo86Info { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal StatementLine61Amount { get; set; }


        [Required]
        public string StatementLine61BankRef { get; set; }


        [Required]
        public string StatementLine61CustomerRef { get; set; }


        [Required]
        public string StatementLine61DOrC { get; set; }


        [Required]
        public string StatementLine61EntryDate { get; set; }


        [Required]
        public string StatementLine61FundsCode { get; set; }


        [Required]
        public string StatementLine61Suppliment { get; set; }

        [Required]
        public string StatementLine61TrnsactnTypeID { get; set; }


        [Required]
        public DateTime StatementLine61ValueDate { get; set; }


        public virtual MT940 MT940 { get; set; }
        public int MT940ID { get; set; }

    }
}
