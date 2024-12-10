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
//        public Guid SymptonsId { get; set; } //Triệu chứng

//        [Required]
//        public Guid FacultyId { get; set; } //Khoa khám bệnh

//        [ForeignKey("SymptonsId")]
//        public virtual Symptons Symptons { get; set; } 

//        [ForeignKey("FacultyId")]
//        public virtual Faculty Faculty { get; set; } 
//    }
//}
