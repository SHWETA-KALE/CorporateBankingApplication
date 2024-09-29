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
using System.Web;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CorporateBankingApplication.Data;

namespace CorporateBankingApplication.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public ActionResult ClientDashboard()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            return View(client);
        }
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
                Salary = e.Salary,
                IsActive = e.IsActive
            }).ToList();

            return Json(employeeDtos, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add(EmployeeDTO employeedto)
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
                Salary = employeedto.Salary,
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
                    employee.Phone,
                    employee.Salary
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
            catch (InvalidOperationException ex)
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

        /************************************Re-editing of details on rejection***************************************/
        public ActionResult EditClientRegistrationDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            var clientDTO = new ClientDTO
            {
                //UserName = client.UserName,
                Email = client.Email,
                CompanyName = client.CompanyName,
                ContactInformation = client.ContactInformation,
                Location = client.Location,
                AccountNumber = client.AccountNumber,
                IFSC = client.IFSC,
                Balance = client.Balance,
                Documents = client.Documents.Select(d => new DocumentDTO
                {
                    DocumentType = d.DocumentType,
                    FilePath = d.FilePath,
                    UploadDate = d.UploadDate
                }).ToList()
            };
            return View(clientDTO);
        }
        [HttpPost]
        public ActionResult EditClientRegistrationDetails(ClientDTO clientDTO)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            client.Email = clientDTO.Email;
            client.CompanyName = clientDTO.CompanyName;
            client.ContactInformation = clientDTO.ContactInformation;
            client.Location = clientDTO.Location;
            client.AccountNumber = clientDTO.AccountNumber;
            client.IFSC = clientDTO.IFSC;
            client.Balance = clientDTO.Balance;
            client.OnBoardingStatus = CorporateStatus.PENDING;
            var uploadedFiles = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                uploadedFiles.Add(companyIdProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }

            _clientService.EditClientRegistrationDetail(client, uploadedFiles);

            return RedirectToAction("ClientDashboard");
        }

        //********************** CSV UPLOAD *********************
        [HttpPost]
        public ActionResult UploadCSV(HttpPostedFileBase file)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    Guid clientId = (Guid)Session["UserId"];
                    var client = _clientService.GetClientById(clientId);

                    if (client == null)
                    {
                        return new HttpStatusCodeResult(400, "Client not found");
                    }

                    using (var reader = new StreamReader(file.InputStream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null, // Allows missing headers like Id
                        MissingFieldFound = null,
                        Delimiter = ","
                    }))
                    {
                        var records = csv.GetRecords<EmployeeDTO>().ToList();

                        foreach (var record in records)
                        {

                            var employeeDto = new EmployeeDTO
                            {
                                FirstName = record.FirstName,
                                LastName = record.LastName,
                                Email = record.Email,
                                Position = record.Position,
                                Phone = record.Phone,
                                Salary = record.Salary,
                                IsActive = true

                            };

                            _clientService.AddEmployee(employeeDto, client);
                        }
                    }
                    return Json(new { success = true, message = "CSV uploaded and employees saved successfully." });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "No file uploaded." });
        }



        //***************************SALARY DISBURSEMENT***************************
        [HttpPost]
        public ActionResult DisburseSalary(List<Guid> employeeIds, bool isBatch)
        {

            if (employeeIds == null || !employeeIds.Any())
            {
                return Json(new { success = false, message = "No employees selected for salary disbursement." });
            }
            try
            {
                var result = _clientService.DisburseSalary(employeeIds, isBatch, out var skippedEmployees);
                if (result)
                {
                    if (isBatch)
                    {
                        // Message for batch processing
                        if (skippedEmployees.Any())
                        {
                            return Json(new { success = true, message = "Salary disbursement initiated, but some employees were skipped because they already received their salary for this month." });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Salary disbursement initiated for all employees." });
                        }
                    }
                    else
                    {
                        if (skippedEmployees.Any())
                        {
                            return Json(new { success = false, message = "Salary has already been disbursed for this month." });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Salary disbursement initiated." });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error during salary disbursement." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }


        }

       

    }
}
