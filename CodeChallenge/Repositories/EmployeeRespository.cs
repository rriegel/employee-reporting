using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees
                // added eager loading for the DirectReports array
                .Include(e => e.DirectReports)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        private int countReports(string employeeId, int currCount)
        {
            // get the list of directReports for the current employee
            Employee currEmployee = GetById(employeeId);
            List<Employee> currReports = currEmployee.DirectReports;
            // loop through the directReports and return the recursive function call
            foreach(Employee directReport in currReports)
            {
                currCount++;
                currCount = countReports(directReport.EmployeeId, currCount);
            }
            // if the directReport list is empty the currCount will be returned
            return currCount;
        }

        public ReportingStructure GetNumberOfReports(string id)
        {
            var employee = GetById(id);
            var numberOfReports = countReports(id, 0);
            ReportingStructure newReport = new ReportingStructure
            {
              Employee = employee,
              NumberOfReports = numberOfReports
            };
            return newReport;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
