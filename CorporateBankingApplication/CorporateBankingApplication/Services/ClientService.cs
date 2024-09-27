using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CorporateBankingApplication.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public void AddEmployee(EmployeeDTO employeeDto, Client client)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Position = employeeDto.Position,
                Phone = employeeDto.Phone,
                IsActive = true,
                Client = client
            };
            _clientRepository.AddEmployee(employee);
        }

        public List<Employee> GetAllEmployees(Guid clientId)
        {
            return _clientRepository.GetAllEmployees(clientId);
        }

        public Client GetClientById(Guid clientId)
        {
           return _clientRepository.GetClientById(clientId);
           
        }

        public Employee GetEmployeeById(Guid id)
        {
            return _clientRepository.GetEmployeeById(id);
        }


        public void UpdateEmployee(EmployeeDTO employeeDto, Client client)
        {
            var existingEmployee = _clientRepository.GetEmployeeById(employeeDto.Id);

            if (existingEmployee != null)
            {
                // Map the fields from the DTO to the existing Employee model
                existingEmployee.FirstName = employeeDto.FirstName;
                existingEmployee.LastName = employeeDto.LastName;
                existingEmployee.Email = employeeDto.Email;
                existingEmployee.Position = employeeDto.Position;
                existingEmployee.Phone = employeeDto.Phone;
                existingEmployee.Client = client;
                _clientRepository.UpdateEmployee(existingEmployee);
            }
        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            _clientRepository.UpdateEmployeeStatus(id, isActive);
        }

       // **************SALARY DISBURSEMNETS*****************
       
        public(bool success, string message)DisburseSalaryBatch(List<Guid> employeeIds, double totalAmount, Guid clientId)
        {
            var client = GetClientById(clientId);
            if(client == null)
            {
                return (false, "Client not found");
            }

            if(client.Balance < totalAmount)
            {
                return (false, "Insufficient balance for salary disbursement.");
            }

            bool isBatch = employeeIds.Count > 1;
            double salaryPerEmployee = isBatch ? totalAmount / employeeIds.Count : totalAmount;

            foreach(var employeeId in employeeIds)
            {
                var employee = client.Employees.FirstOrDefault(e=>e.Id == employeeId);
                if(employee != null)
                {
                    var salaryDisbursement = new SalaryDisbursement
                    {
                        Id = Guid.NewGuid(),
                        Employee = employee,
                        Salary = salaryPerEmployee,
                        DisbursementDate = DateTime.Now,
                        IsBatch = isBatch,
                        SalaryStatus = CorporateStatus.PENDING
                    };

                    AddSalaryDisbursement(client, salaryDisbursement);
                }
            }

            client.Balance -= totalAmount;
            _clientRepository.Save(client);

            return (true, "Salary disbursement request sent to admin.");
        }

        public void AddSalaryDisbursement(Client client, SalaryDisbursement salaryDisbursement)
        {
            client.Employees.FirstOrDefault(e => e.Id == salaryDisbursement.Employee.Id)?.SalaryDisbursements.Add(salaryDisbursement);
            _clientRepository.Save(client); // Save the changes after adding salary disbursement
        }
    }
}