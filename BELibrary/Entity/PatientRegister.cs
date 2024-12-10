using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BELibrary.Entity
{
    [Table("PatientRegister")]
    public class PatientRegister
    {
        public int maxCapacityPerRoom;

        public PatientRegister()
        {
            PickTime = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }

        public Guid PatientId { get; set; }
        
        public int Number {  get; set; }
        public DateTime PickTime { get; set; }

        public DateTime? StartTime { get; set; }

        [NotMapped]
        public int? ChangeRoomId { get; set; }
        [NotMapped]
        public string ChangeRoomName { get; set; }

        [ForeignKey(nameof(RoomId))]
        public virtual Room Room { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; }
    }
}
