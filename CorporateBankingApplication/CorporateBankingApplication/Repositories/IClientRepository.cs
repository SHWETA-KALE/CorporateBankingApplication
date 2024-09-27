using CorporateBankingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public interface IClientRepository
    {
        List<Employee> GetAllEmployees(Guid clientId);
        void AddEmployee(Employee employee);
        Employee GetEmployeeById(Guid id);

        Client GetClientById(Guid clientId);
        void UpdateEmployee(Employee employee);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        //****************SALARY DISBURSEMENTS **********************
        void Save(Client client);
    }
}