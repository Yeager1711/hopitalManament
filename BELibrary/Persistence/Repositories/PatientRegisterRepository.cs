using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BELibrary.Persistence.Repositories
{
    public class PatientRegisterRepository : Repository<PatientRegister>, IPatientRegisterRepository
    {
        public PatientRegisterRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}
