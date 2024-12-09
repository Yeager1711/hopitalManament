using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BELibrary.Entity
{
    // Phòng khám
    [Table("Room")]
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        //Bác sỹ phụ trách
        public Guid? DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; }

        // Phòng khám thuộc khoa
        public Guid? FacultyId { get; set; }

        [ForeignKey(nameof(FacultyId))]
        public virtual Faculty Faculty { get; set; }
    }
}
