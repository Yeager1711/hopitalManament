//namespace BELibrary.Entity
//{
//    using System;
//    using System.ComponentModel.DataAnnotations;
//    using System.ComponentModel.DataAnnotations.Schema;

//    [Table("detailsFacultySymptons")]
//    public partial class DetailsFacultySymptons
//    {
//        [Key]
//        public Guid Id { get; set; }

//        [Required]
//        public Guid SymptonsId { get; set; } // Id của triệu chứng

//        [Required]
//        public Guid FacultyId { get; set; } // Id của khoa khám

//        [ForeignKey("SymptonsId")]
//        public virtual Symptons Symptons { get; set; } // Liên kết tới Symptoms

//        [ForeignKey("FacultyId")]
//        public virtual Faculty Faculty { get; set; } // Liên kết tới Faculty
//    }
//}
