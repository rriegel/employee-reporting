
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange data by creating employee to be associated with compensation
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var eRequestContent = new JsonSerialization().ToJson(employee);

            var ePostRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(eRequestContent, Encoding.UTF8, "application/json"));
            var eResponse = ePostRequestTask.Result;
            var newEmployee = eResponse.DeserializeContent<Employee>();

            var compensation = new Compensation()
            {
                Employee = newEmployee,
                Salary = "120000",
                EffectiveDate = DateTime.Now,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute post request for compensation
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert results
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var Compensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(Compensation.CompensationId);
            Assert.AreEqual(compensation.Employee.FirstName, Compensation.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, Compensation.Employee.LastName);
            Assert.AreEqual(compensation.Salary, Compensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate.ToString(), Compensation.EffectiveDate.ToString());
        }

        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            // Arrange data by creating employee and compensation for employee
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var eRequestContent = new JsonSerialization().ToJson(employee);

            var ePostRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(eRequestContent, Encoding.UTF8, "application/json"));
            var eResponse = ePostRequestTask.Result;
            var newEmployee = eResponse.DeserializeContent<Employee>();

            var expectedSalary = "120000";
            var expectedEffectiveDate = DateTime.Now;
            var compensation = new Compensation()
            {
                Employee = newEmployee,
                Salary = expectedSalary,
                EffectiveDate = expectedEffectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var pResponse = postRequestTask.Result;
            var Compensation = pResponse.DeserializeContent<Compensation>();

            // Execute GET request for previously created compensation
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{Compensation.CompensationId}");
            var response = getRequestTask.Result;

            // Assert results
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var finalCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employee.FirstName, finalCompensation.Employee.FirstName);
            Assert.AreEqual(employee.LastName, finalCompensation.Employee.LastName);
            Assert.AreEqual(expectedEffectiveDate.ToString(), finalCompensation.EffectiveDate.ToString());
            Assert.AreEqual(expectedSalary, finalCompensation.Salary);
        }

[TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange data by creating employee and compensation for employee
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var eRequestContent = new JsonSerialization().ToJson(employee);

            var ePostRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(eRequestContent, Encoding.UTF8, "application/json"));
            var eResponse = ePostRequestTask.Result;
            var newEmployee = eResponse.DeserializeContent<Employee>();
            var newEmployeeId = newEmployee.EmployeeId;

            var expectedSalary = "120000";
            var expectedEffectiveDate = DateTime.Now;
            var compensation = new Compensation()
            {
                Employee = newEmployee,
                Salary = expectedSalary,
                EffectiveDate = expectedEffectiveDate,
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var pResponse = postRequestTask.Result;
            var Compensation = pResponse.DeserializeContent<Compensation>();

            // Execute GET byEmployee request for previously created compensation
            var getRequestTask = _httpClient.GetAsync($"api/compensation/byEmployee/{newEmployeeId}");
            var response = getRequestTask.Result;

            // Assert results
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var finalCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employee.FirstName, finalCompensation.Employee.FirstName);
            Assert.AreEqual(employee.LastName, finalCompensation.Employee.LastName);
            Assert.AreEqual(expectedEffectiveDate.ToString(), finalCompensation.EffectiveDate.ToString());
            Assert.AreEqual(expectedSalary, finalCompensation.Salary);
        }
    }
}
