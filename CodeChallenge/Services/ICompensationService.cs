using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation GetById(string id);
        Compensation GetByEmployeeId(string employeeId);
        Compensation Create(Compensation compensation);
        Compensation Replace(Compensation originalCompensation, Compensation newCompensation);
    }
}
