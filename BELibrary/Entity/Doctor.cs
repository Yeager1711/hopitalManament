﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("Doctor")]
    public class Doctor
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Descriptions { get; set; }

        public string Address { get; set; }

        public bool Gender { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsDelete { get; set; }

        public Guid FacultyId { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual List<DoctorSchedule> DoctorSchedules { get; set; }
    }
}