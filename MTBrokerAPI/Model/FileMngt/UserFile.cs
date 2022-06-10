using System.ComponentModel.DataAnnotations;

namespace MTBrokerAPI.Model.FileMngt
{
    public class UserFile
    {
        public int ID { get; set; }


        [Required]
        public string ContentType { get; set; }


        [Required]
        public string FileExtension { get; set; }


        [Required]
        public string FileName { get; set; }


        [Required]
        public string FileUri { get; set; }


        [Required]
        public ulong FileSize { get; set; }


        [Required]
        public string FileSizeReadable { get; set; }


        public virtual ApplicationUser Owner { get; set; }
        public int OwnerID { get; set; }

    }
}
