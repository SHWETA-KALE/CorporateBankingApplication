using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public interface IClientService
    {
        List<Employee> GetAllEmployees(Guid clientId);
        void AddEmployee(EmployeeDTO employeeDto, Client client);
        Employee GetEmployeeById(Guid id);

        void UpdateEmployee(EmployeeDTO employeeDto, Client client);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        Client GetClientById(Guid clientId);



    }
}