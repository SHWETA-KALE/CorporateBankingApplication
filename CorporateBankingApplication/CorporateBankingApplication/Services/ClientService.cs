using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        
    }
}