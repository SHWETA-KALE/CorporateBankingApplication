//using CorporateBankingApplication.Data;
//using CorporateBankingApplication.DTOs;
//using CorporateBankingApplication.Models;
//using NHibernate.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace CorporateBankingApplication.Controllers
//{
//    public class ClientController : Controller
//    {
//        // GET: Client

//        // [Authorize(Roles ="Client")]

//        [AllowAnonymous]
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AllowAnonymous]
//        public ActionResult GetAllEmployees()
//        {
//            if (Session["UserId"] == null)
//            {
//                return RedirectToAction("Login", "User");
//            }
//            //storing the userid that is coming from the session
//            Guid userId = (Guid)Session["UserId"]; //clientId

//            //do this in client repo
//            using (var session = NHibernateHelper.CreateSession())
//            {
//                //fetching us particular clients ka employees
//                // var clientWithEmployees = session.Query<Client>().Where(c => c.Id == userId).FetchMany(c => c.Employees).ThenFetch(e => e.Client).ToFuture().FirstOrDefault();
//                var clientWithEmployees = session.Query<Client>().FetchMany(c => c.Employees).SingleOrDefault(u => u.Id == userId);

//                if (clientWithEmployees != null)
//                {
//                    //mapping models to DTOs
//                    var employeeDto = clientWithEmployees.Employees.Select(employee => new EmployeeDTO
//                    {
//                        Id = employee.Id,
//                        FirstName = employee.FirstName,
//                        LastName = employee.LastName,
//                        Email = employee.Email,
//                        Position = employee.Position,
//                        Phone = employee.Phone
//                    }).ToList();

//                    return Json(employeeDto, JsonRequestBehavior.AllowGet);
//                }
//                return new HttpStatusCodeResult(500);
//            }

//        }

//        public ActionResult Add(Employee employee)
//        {
//            if (Session["UserId"] == null)
//            {
//                return new HttpStatusCodeResult(401, "Unauthorized");
//            }
//            Guid userId = (Guid)Session["UserId"]; //clientId

//            using (var session = NHibernateHelper.CreateSession())
//            {
//                using (var transaction = session.BeginTransaction())
//                {
//                    try
//                    {
//                        //fetch the client 
//                        var client = session.Query<Client>().SingleOrDefault(c => c.Id == userId);
//                        if (client == null)
//                        {
//                            return HttpNotFound("User not found");
//                        }

//                        employee.Client = client;
//                        session.Save(employee);
//                        transaction.Commit();

//                        return Json(new
//                        {
//                            Id = employee.Id,
//                            FirstName = employee.FirstName,
//                            LastName = employee.LastName,
//                            Email = employee.Email,
//                            Position = employee.Position,
//                            Phone = employee.Phone
//                        });
//                    }
//                    catch (Exception ex)
//                    {
//                        transaction.Rollback();
//                        return new HttpStatusCodeResult(500, "Error adding contact: " + ex.Message);
//                    }
//                }

//            }
//        }

//        [HttpGet]
//        public ActionResult GetEmployeeById(Guid id)
//        {
//            if (Session["UserId"] == null)
//            {
//                return new HttpStatusCodeResult(401, "Unauthorized");
//            }
//            using (var session = NHibernateHelper.CreateSession())
//            {
//                var employee = session.Get<Employee>(id);
//                if (employee == null)
//                {
//                    return Json(new { success = false, message = "Employee not found" }, JsonRequestBehavior.AllowGet);
//                }
//                return Json(new
//                {
//                    success = true,
//                    employee = new
//                    {
//                        employee.Id,
//                        employee.FirstName,
//                        employee.LastName,
//                        employee.Email,
//                        employee.Position,
//                        employee.Phone
//                    }
//                }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        [HttpPost]
//        public ActionResult Edit(Employee employee)
//        {
//            if (Session["UserId"] == null)
//            {
//                return new HttpStatusCodeResult(401, "Unauthorized");
//            }

//            using (var session = NHibernateHelper.CreateSession())
//            {
//                using (var transaction = session.BeginTransaction())
//                {
//                    try
//                    {
//                        var existingEmployee = session.Get<Employee>(employee.Id);
//                        if (existingEmployee == null)
//                        {
//                            return Json(new { success = false, message = "Employee not found" });
//                        }

//                        existingEmployee.FirstName = employee.FirstName;
//                        existingEmployee.LastName = employee.LastName;
//                        existingEmployee.Email = employee.Email;
//                        existingEmployee.Position = employee.Position;
//                        existingEmployee.Phone = employee.Phone;

//                        session.Update(existingEmployee);
//                        transaction.Commit();

//                        return Json(new { success = true, message = "Employee edited successfully" });
//                    }
//                    catch (Exception ex)
//                    {
//                        transaction.Rollback();
//                        return new HttpStatusCodeResult(500, "Error editing employee: " + ex.Message);
//                    }
//                }
//            }
//        }

//        [HttpPost]
//        //soft delete
//        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
//        {
//            using (var session = NHibernateHelper.CreateSession())
//            {
//                using (var transaction = session.BeginTransaction())
//                {
//                    //fetch the employee
//                    var employee = session.Get<Employee>(id);
//                    if (employee != null)
//                    {
//                        employee.IsActive = isActive;
//                        session.Update(employee);
//                        transaction.Commit();
//                        return Json(new { success = true });
//                    }
//                    return Json(new { success = false, message = "Employee not found." });
//                }
//            }
//        }
//    }
//}


using CorporateBankingApplication.Models;
using CorporateBankingApplication.Services;
using CorporateBankingApplication.DTOs;
using System;
using System.Linq;
using System.Web.Mvc;

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
           // employeeDto.Client = client;
            _clientService.UpdateEmployee(employeeDto, client);

            return Json(new { success = true, message = "Employee updated successfully" });
        }

        [HttpPost]
        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            _clientService.UpdateEmployeeStatus(id, isActive);
            return Json(new { success = true });
        }
    }
}
