using CorporateBankingApplication.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

namespace CorporateBankingApplication.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ISession _session;

        public ClientRepository(ISession session)
        {
            _session = session;
        }
        public void AddEmployee(Employee employee)
        {
            using(var transaction = _session.BeginTransaction())
            {
                _session.Save(employee);
                transaction.Commit();
            }
           
        }

        public List<Employee> GetAllEmployees(Guid clientId)
        {
            var client = _session.Query<Client>().FetchMany(c => c.Employees).SingleOrDefault(c => c.Id == clientId);
            return client?.Employees.ToList() ?? new List<Employee>();
        }

        public Employee GetEmployeeById(Guid id)
        {
            return _session.Get<Employee>(id);
        }

        public Client GetClientById(Guid clientId)
        {
            return _session.Get<Client>(clientId);
        }

        public void UpdateEmployee(Employee employee)
        {
            using(var transaction = _session.BeginTransaction())
            {
                _session.Update(employee);
                transaction.Commit();
            }
            
        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            using(var transaction = _session.BeginTransaction())
            {
                var employee = _session.Get<Employee>(id);
                if (employee != null)
                {
                    employee.IsActive = isActive;
                    _session.Update(employee);
                    transaction.Commit();
                }
            }
            
        }


        /************************************Re-editing of details on rejection***************************************/
        public void UpdateClientRegistrationDetails(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                //if (!string.IsNullOrEmpty(client.Password))
                //{
                //    client.Password = PasswordHelper.HashPassword(client.Password);
                //}

                _session.Update(client);
                transaction.Commit();
            }
        }

        //******************SALARY DISBURSEMENTS *********************

        public List<Employee> GetEmployeesByIds(List<Guid> employeeIds)
        {
            return _session.Query<Employee>().Where(e=>employeeIds.Contains(e.Id)).ToList();
        }

        public void AddSalaryDisbursement(SalaryDisbursement salaryDisbursement)
        {
            using(var transaction = _session.BeginTransaction())
            {
                _session.Save(salaryDisbursement);
                transaction.Commit();
            }
        }

        public SalaryDisbursement GetSalaryDisbursementForEmployee(Guid employeeId, DateTime currentDate)
        {
            // Get the first day of the month of the current date
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            // Get the first day of the next month
            var endOfMonth = startOfMonth.AddMonths(1);

            return _session.Query<SalaryDisbursement>().FirstOrDefault(sd=>sd.Employee.Id == employeeId && sd.DisbursementDate >= startOfMonth && sd.DisbursementDate < endOfMonth);
        }
    }
}