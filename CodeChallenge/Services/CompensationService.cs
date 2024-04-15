using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Compensation Create(Compensation compensation)
        {
            if(compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public Compensation GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetById(id);
            }

            return null;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            if(!String.IsNullOrEmpty(employeeId))
            {
                return _compensationRepository.GetByEmployeeId(employeeId);
            }

            return null;
        }

        public Compensation Replace(Compensation originalCompensation, Compensation newCompensation)
        {
            if(originalCompensation != null)
            {
                _compensationRepository.Remove(originalCompensation);
                if (newCompensation != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _compensationRepository.SaveAsync().Wait();

                    _compensationRepository.Add(newCompensation);
                    // overwrite the new id with previous employee id
                    newCompensation.Employee.EmployeeId = originalCompensation.Employee.EmployeeId;
                }
                _compensationRepository.SaveAsync().Wait();
            }

            return newCompensation;
        }
    }
}
