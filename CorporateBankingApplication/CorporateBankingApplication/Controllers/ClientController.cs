using CorporateBankingApplication.Models;
using CorporateBankingApplication.Services;
using CorporateBankingApplication.DTOs;
using System;
using System.Linq;
using System.Web.Mvc;
using NHibernate.Mapping;
using System.Collections.Generic;
using CorporateBankingApplication.Enum;
using NHibernate.Transform;

namespace CorporateBankingApplication.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllEmployees()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var employees = _clientService.GetAllEmployees(clientId);

            var employeeDtos = employees.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Position = e.Position,
                Phone = e.Phone,
                IsActive = e.IsActive
            }).ToList();

            return Json(employeeDtos, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add(EmployeeDTO employeedto) //employeedto
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);

            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }
            employeedto.IsActive = true;
            //employeedto.Client = client;

            _clientService.AddEmployee(employeedto, client);

            return Json(new
            {
                Id = employeedto.Id,
                FirstName = employeedto.FirstName,
                LastName = employeedto.LastName,
                Email = employeedto.Email,
                Position = employeedto.Position,
                Phone = employeedto.Phone,
                IsActive = employeedto.IsActive
            });

        }
        [HttpGet]
        public ActionResult GetEmployeeById(Guid id)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            var employee = _clientService.GetEmployeeById(id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                employee = new
                {
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email,
                    employee.Position,
                    employee.Phone
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeDTO employeeDto)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);

            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }
            var existingEmployee = _clientService.GetEmployeeById(employeeDto.Id);

            try
            {
                _clientService.UpdateEmployee(employeeDto, client);
                return Json(new { success = true, message = "Employee updated successfully" });
            }
            catch(InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
         
        }

        [HttpPost]
        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            _clientService.UpdateEmployeeStatus(id, isActive);
            return Json(new { success = true });
        }


        //***************************SALARY DISBURSEMENT***************************
        [HttpPost]
        public ActionResult DisburseSalaryBatch(List<Guid> employeeIds, double totalAmount, Guid clientId)
        {
            var result = _clientService.DisburseSalaryBatch(employeeIds, totalAmount, clientId);
            if (!result.success)
            {
                return Json(new { success = false, message = result.message });
            }
            return Json(new { success = true, message = result.message });
        }
    }
}
