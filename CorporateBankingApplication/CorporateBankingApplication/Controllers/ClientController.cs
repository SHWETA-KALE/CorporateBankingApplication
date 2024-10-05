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
using System.Reflection;
using System.Web.Routing;
using iTextSharp.text.pdf;

namespace CorporateBankingApplication.Controllers
{
    [Authorize(Roles = "Client")]
    [HandleError]
    [RoutePrefix("client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult ClientDashboard()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                throw new InvalidOperationException("Client not found");
            }
            return View(client);
           

        }

        [Route("employee")]
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            //for checking onboarding status
            ViewBag.Client = client;
            if (client == null)
            {
                throw new InvalidOperationException("Client not found");
            }
            return View();
        }


        //public ActionResult GetAllEmployees()
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        return RedirectToAction("Login", "User");
        //    }

        //    Guid clientId = (Guid)Session["UserId"];
        //    var employees = _clientService.GetAllEmployees(clientId);
        //    if (employees == null || !employees.Any())
        //    {
        //        throw new InvalidOperationException("No employees found for this client");
        //    }


        //    var employeeDtos = employees.Select(e => new EmployeeDTO
        //    {
        //        Id = e.Id,
        //        FirstName = e.FirstName,
        //        LastName = e.LastName,
        //        Email = e.Email,
        //        Position = e.Position,
        //        Phone = e.Phone,
        //        Salary = e.Salary,
        //        IsActive = e.IsActive
        //    }).ToList();

        //    return Json(employeeDtos, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetAllEmployees(string firstName, string lastName)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var employees = _clientService.GetAllEmployees(clientId);

            // Apply search filter if firstName or lastName is provided
            if (!string.IsNullOrEmpty(firstName))
            {
                employees = employees.Where(e => e.FirstName.IndexOf(firstName, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                employees = employees.Where(e => e.LastName.IndexOf(lastName, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (employees == null || !employees.Any())
            {
                throw new InvalidOperationException("No employees found for this client");
            }

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
            
            if (!ModelState.IsValid)
            {
                // Collect errors into a dictionary to return as JSON
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors });
            }
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
            if (!ModelState.IsValid)
            {
                // Collect errors into a dictionary to return as JSON
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors });
            }
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

        [Route("clientregistration")]
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
                Id = client.Id,
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

        [Route("clientregistration")]
        [HttpPost]
        public ActionResult EditClientRegistrationDetails(ClientDTO clientDTO)
        {
          
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }


            Guid clientId = (Guid)Session["UserId"];
           // clientDTO.Id = clientId;
           
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

            //for doc validation
            if (companyIdProof == null || companyIdProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document1", "Company Id Proof field is required.");
            }

            if (addressProof == null || addressProof.ContentLength == 0)
            {
                ModelState.AddModelError("Document2", "Address Proof field is required.");
            }
            if (!ModelState.IsValid)
            {
                // Collect errors into a dictionary to return as JSON
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return View(clientDTO);
            }

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
        public ActionResult DisburseSalary(List<Guid> employeeIds, bool isBatch, double amount)
        {
            //
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client.Balance < amount)
            {
                return Json(new { success = false, message = "You don't have enough balance. Visit your profile to update your balance." });
            }
            //

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

        /*************************** MANAGE BENEFICIARIES *****************************/

        [Route("listofbeneficiaries")]
        public ActionResult ViewAllOutboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            //for checking onboarding status
            ViewBag.Client = client;
            return View();
        }

        public ActionResult GetAllOutboundBeneficiaries(string searchTerm)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var urlHelper = new UrlHelper(Request.RequestContext);

            // Get all beneficiaries
            var beneficiaries = _clientService.GetAllOutboundBeneficiaries(clientId, urlHelper);

            // Apply search filter if searchTerm is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                beneficiaries = beneficiaries
                                .Where(b => b.BeneficiaryName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                                .ToList();
            }

            return Json(beneficiaries, JsonRequestBehavior.AllowGet);
        }


        [Route("ShowingBeneficiary")]
        public ActionResult ViewAllInboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            //for checking onboarding status
            ViewBag.Client = client;
            return View();
        }

        public ActionResult GetAllInboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var beneficiaries = _clientService.GetAllInboundBeneficiaries(clientId);
            return Json(beneficiaries, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddInboundBeneficiary(List<Guid> beneficiaryIds)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            if (beneficiaryIds == null || !beneficiaryIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No beneficiaries selected for addition."
                });
            }
            bool success = true;
            foreach (var id in beneficiaryIds)
            {
                //_clientService.AddInboundBeneficiary(id);   
                _clientService.AddInboundBeneficiary(clientId, id);
                success = true;
                //break;
            }
            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Inbound Beneficiary/s added successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add Inbound Beneficiary/s."
                });
            }
        }
        public ActionResult UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            _clientService.UpdateBeneficiaryStatus(id, isActive);
            return Json(new { success = true });
        }

        //public ActionResult AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO)
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        return new HttpStatusCodeResult(401, "Unauthorized");
        //    }

        //    Guid clientId = (Guid)Session["UserId"];
        //    var client = _clientService.GetClientById(clientId);
        //    if (client == null)
        //    {
        //        return new HttpStatusCodeResult(400, "Client not found");
        //    }
        //    var uploadedFiles = new List<HttpPostedFileBase>();

        //    var idProof = Request.Files["uploadedDocs1"];
        //    var addressProof = Request.Files["uploadedDocs2"];

        //    //for doc validation
        //    if (idProof == null || idProof.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("BeneficiaryIdProof", "Id Proof field is required.");
        //    }

        //    if (addressProof == null || addressProof.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof field is required.");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        // Collect errors into a dictionary to return as JSON
        //        var errors = ModelState.ToDictionary(
        //            kvp => kvp.Key,
        //            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        //        );
        //        return Json(new { success = false, errors });
        //    }

        //    if (idProof != null && idProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(idProof);
        //    }

        //    if (addressProof != null && addressProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(addressProof);
        //    }
        //    _clientService.AddNewBeneficiary(beneficiaryDTO, client, uploadedFiles);
        //    return Json(new { success = true });
        //}

        public ActionResult AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO)
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
            var uploadedFiles = new List<HttpPostedFileBase>();

            var idProof = Request.Files["uploadedDocs1"];
            var addressProof = Request.Files["uploadedDocs2"];
            //for doc validation
            if (idProof == null || idProof.ContentLength == 0)
            {
                ModelState.AddModelError("BeneficiaryIdProof", "Id Proof field is required.");
            }

            if (addressProof == null || addressProof.ContentLength == 0)
            {
                ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof field is required.");
            }
            if (!ModelState.IsValid)
            {
                // Collect errors into a dictionary to return as JSON
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors });
            }
            if (idProof != null && idProof.ContentLength > 0)
            {
                uploadedFiles.Add(idProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
            _clientService.AddNewBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true });
        }

        public ActionResult GetBeneficiaryById(Guid id)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            var beneficiary = _clientService.GetBeneficiaryById(id);
            if (beneficiary == null)
            {
                return Json(new { success = false, message = "Beneficiary not found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                beneficiary = new
                {
                    beneficiary.Id,
                    beneficiary.BeneficiaryName,
                    beneficiary.AccountNumber,
                    beneficiary.BankIFSC
                }
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult EditBeneficiary(BeneficiaryDTO beneficiaryDTO)
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        return new HttpStatusCodeResult(401, "Unauthorized");
        //    }
        //    Guid clientId = (Guid)Session["UserId"];
        //    var client = _clientService.GetClientById(clientId);

        //    if (client == null)
        //    {
        //        return new HttpStatusCodeResult(400, "Client not found");
        //    }
        //    var existingBeneficiary = _clientService.GetBeneficiaryById(beneficiaryDTO.Id);
        //    var uploadedFiles = new List<HttpPostedFileBase>();

        //    var idProof = Request.Files["newIdProof"];
        //    var addressProof = Request.Files["newAddressProof"];

        //    //for doc validation
        //    if (idProof == null || idProof.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("BeneficiaryIdProof", "Id Proof field is required.");
        //    }

        //    if (addressProof == null || addressProof.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof field is required.");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        // Collect errors into a dictionary to return as JSON
        //        var errors = ModelState.ToDictionary(
        //            kvp => kvp.Key,
        //            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        //        );
        //        return Json(new { success = false, errors });
        //    }

        //    if (idProof != null && idProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(idProof);
        //    }

        //    if (addressProof != null && addressProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(addressProof);
        //    }
        //    _clientService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
        //    return Json(new { success = true, message = "Beneficiary updated successfully" });
        //}

        public ActionResult EditBeneficiary(BeneficiaryDTO beneficiaryDTO)
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

            var existingBeneficiary = _clientService.GetBeneficiaryById(beneficiaryDTO.Id);
            var uploadedFiles = new List<HttpPostedFileBase>();

            var idProof = Request.Files["newIdProof"];
            var addressProof = Request.Files["newAddressProof"];

            //for doc validation
            if (idProof == null || idProof.ContentLength == 0)
            {
                ModelState.AddModelError("BeneficiaryIdProof", "Id Proof field is required.");
            }

            if (addressProof == null || addressProof.ContentLength == 0)
            {
                ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof field is required.");
            }
            if (!ModelState.IsValid)
            {
                // Collect errors into a dictionary to return as JSON
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors });
            }

            if (idProof != null && idProof.ContentLength > 0)
            {
                uploadedFiles.Add(idProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
            _clientService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true, message = "Beneficiary updated successfully" });
        }
        /***************************PROFILE*****************************/

        [Route("Profile")]
        public ActionResult ViewClientProfile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            var clientDto = new ClientDTO
            {
                Id = client.Id,
                UserName = client.UserName,
                Email = client.Email,
                CompanyName = client.CompanyName,
                ContactInformation = client.ContactInformation,
                Location = client.Location,
                Balance = client.Balance,
                AccountNumber = client.AccountNumber,
                IFSC = client.IFSC,
                OnboardingStatus = client.OnBoardingStatus,
                Documents = client.Documents.Select(document => new DocumentDTO
                {
                    DocumentType = document.DocumentType,
                    FilePath = document.FilePath
                }).ToList()
            };
            return View(clientDto);
        }

        public ActionResult ChangePassword(string previousPassword, string newPassword, string confirmNewPassword)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var previousPasswordCheck = _clientService.CheckPassword(clientId, previousPassword);
            if (previousPasswordCheck)
            {
                if (newPassword != null && newPassword == confirmNewPassword)
                {
                    _clientService.SaveNewPassword(clientId, newPassword);
                    return Json(new { success = true, message = "Password updated successfully." });
                }
            }
            if (newPassword != null && newPassword != confirmNewPassword)
            {
                return Json(new { success = false, message = "Passwords don't match." });
            }
            return Json(new { success = false, message = "Error in updating password" });
        }



        /**************************AddBalance***************************/
       
        [HttpPost]
        public ActionResult AddBalance(Guid id, double amount)
        {
            var client = _clientService.GetClientById(id);
            if (client.OnBoardingStatus == CorporateStatus.PENDING || client.OnBoardingStatus == CorporateStatus.REJECTED)
            {
                return Json(new { success = false, message = "Cannot update balance as you are not approved"});
            }
            else
            {
                _clientService.AddBalance(id, amount);
                return Json(new { success = true });
            }
        }

        /*******************************PAYMENTS*********************************/

        [Route("Payment")]
        public ActionResult ViewBeneficiaryListForPayment()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            //for checking onboarding status
            ViewBag.Client = client;
            var beneficiaryList = _clientService.GetBeneficiaryList(clientId);
            if (beneficiaryList == null || !beneficiaryList.Any())
            {
                ViewBag.Message = "No beneficiaries found.";
                return View(new PaymentBeneficiaryDTO { Beneficiaries = new List<BeneficiaryDTO>() });

            }
            var model = new PaymentBeneficiaryDTO
            {
                Beneficiaries = beneficiaryList
            };

            return View(model);
        }
    
        
        [HttpGet]
        public ActionResult GetBeneficiaryListForPayment()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var beneficiaryList = _clientService.GetBeneficiaryList(clientId);

            if (beneficiaryList == null || !beneficiaryList.Any())
            {
                return Json(new { success = false, message = "No beneficiaries found" }, JsonRequestBehavior.AllowGet);
            }

            var paymentBeneficiaryDTO = new PaymentBeneficiaryDTO
            {
                Amount = 0,  // Default amount
                Beneficiaries = beneficiaryList
            };

            return Json(new { success = true, data = paymentBeneficiaryDTO }, JsonRequestBehavior.AllowGet);
        }

        /*********************************REPORTS **************************************/

        [Route("report")]
        public ActionResult ReportView()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [Route("salaryreport")]
        public ActionResult ViewSalaryDisbursements()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            ViewBag.Client = client;
            var salaryDisbursements = _clientService.GetAllSalaryDisbursements(clientId);
            return View(salaryDisbursements);
        }
        public ActionResult DownloadSalaryDisbursementsPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var list = _clientService.GetAllSalaryDisbursements(clientId);
            // Create a new PDF document
            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                // Create a table for better formatting
                var table = new PdfPTable(6); // Create a table with 5 columns
                table.AddCell("ID");
                table.AddCell("Client Company Name");
                table.AddCell("Employee Name");
                table.AddCell("Salary");
                table.AddCell("Disbursement Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var emp in list)
                {
                    table.AddCell(emp.SalaryDisbursementId.ToString());
                    table.AddCell(emp.CompanyName);
                    table.AddCell($"{emp.EmployeeFirstName} {emp.EmployeeLastName}");
                    table.AddCell(emp.Salary.ToString("C")); // Format as currency
                    table.AddCell(emp.DisbursementDate.ToShortDateString());
                    table.AddCell(emp.SalaryStatus.ToString());
                }

                // Add the table to the document
                doc.Add(table);
                doc.Close(); // Closing the document finalizes it

                // Prepare the byte array to return
                byte[] bytes = memoryStream.ToArray();

                //add in report table
                _clientService.AddReportInfo(clientId);

                return File(bytes, "application/pdf", "SalaryDisbursement.pdf");
            }
        }

        [Route("paymentreport")]
        public ActionResult ViewPayments()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            ViewBag.Client = client;
            var payments = _clientService.GetPayments(clientId);
            return View(payments);
        }

        public ActionResult DownloadPaymentPDFReport()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid userId = (Guid)Session["UserId"];
            var list = _clientService.GetPayments(userId);

            // Create a new PDF document
            using (var memoryStream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                // Create a table for better formatting
                var table = new PdfPTable(7); // Create a table with 5 columns
                table.AddCell("ID");
                table.AddCell("Client Company Name");
                table.AddCell("Account Number");
                table.AddCell("Beneficiary Name");
                table.AddCell("Amount");
                table.AddCell("Payment Request Date");
                table.AddCell("Status");

                // Add data to the table
                foreach (var pay in list)
                {
                    table.AddCell(pay.PaymentId.ToString());
                    table.AddCell(pay.CompanyName);
                    table.AddCell(pay.AccountNumber);
                    table.AddCell(pay.BeneficiaryName); // Format as currency
                    table.AddCell(pay.Amount.ToString()); // Format as currency
                    table.AddCell(pay.PaymentRequestDate.ToShortDateString());
                    table.AddCell(pay.PaymentStatus.ToString());
                }

                // Add the table to the document
                doc.Add(table);
                doc.Close(); // Closing the document finalizes it

                // Prepare the byte array to return
                byte[] bytes = memoryStream.ToArray();

                //add in report table
                _clientService.AddPaymentReportInfo(userId);

                return File(bytes, "application/pdf", "Payment.pdf");
            }
        }
    }
}
