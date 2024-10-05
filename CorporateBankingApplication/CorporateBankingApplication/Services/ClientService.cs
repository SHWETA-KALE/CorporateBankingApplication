using CorporateBankingApplication.Data;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


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
                Salary = employeeDto.Salary,
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
                existingEmployee.Salary = employeeDto.Salary;
                existingEmployee.Client = client;
                _clientRepository.UpdateEmployee(existingEmployee);
            }
        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            _clientRepository.UpdateEmployeeStatus(id, isActive);
        }

        /************************************Re-editing of details on rejection***************************************/
        public void EditClientRegistrationDetail(Client client, IList<HttpPostedFileBase> uploadedFiles)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/ClientRegistration/") + client.UserName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Document update process
            string[] documentTypes = { "Company Id Proof", "Address Proof" };
            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                var file = uploadedFiles[i];
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(filePath);
                    string relativeFilePath = $"~/Content/Documents/ClientRegistration/{client.UserName}/{fileName}";

                    // Check if the client already has this document type, if so, update it
                    var existingDocument = client.Documents.FirstOrDefault(d => d.DocumentType == documentTypes[i]);
                    if (existingDocument != null)
                    {
                        // Update existing document
                        existingDocument.FilePath = relativeFilePath;
                        existingDocument.UploadDate = DateTime.Now;
                    }
                    else
                    {
                        // Add new document if not present
                        var document = new Document
                        {
                            DocumentType = documentTypes[i],
                            FilePath = relativeFilePath,
                            UploadDate = DateTime.Now,
                            Client = client
                        };
                        client.Documents.Add(document);
                    }
                }
            }

            _clientRepository.UpdateClientRegistrationDetails(client);
        }

        // **************SALARY DISBURSEMNETS*****************

        public bool DisburseSalary(List<Guid> employeeIds, bool isBatch, out List<Guid> skippedEmployees)
        {
            skippedEmployees = new List<Guid>();

            var employees = _clientRepository.GetEmployeesByIds(employeeIds);

            if (employees == null || !employees.Any())
            {
                return false;
            }

            foreach (var employee in employees)
            {
                // Check if a salary has already been disbursed to this employee on this date
                var existingDisbursement = _clientRepository.GetSalaryDisbursementForEmployee(employee.Id, DateTime.Now);
                if (existingDisbursement != null)
                {
                    skippedEmployees.Add(employee.Id);
                    continue;
                }

                var salaryDisbursement = new SalaryDisbursement
                {
                    Employee = employee,
                    DisbursementDate = DateTime.Now,
                    IsBatch = isBatch,
                    SalaryStatus = CorporateStatus.PENDING
                    
                };
                

                _clientRepository.AddSalaryDisbursement(salaryDisbursement);
            }

            return true;
        }

        /*beneficiaries*/
        public List<BeneficiaryDTO> GetAllOutboundBeneficiaries(Guid clientId, UrlHelper urlHelper)
        {
            var beneficiaries = _clientRepository.GetAllOutboundBeneficiaries(clientId);
            var beneficiariesDto = beneficiaries.Select(b => new BeneficiaryDTO
            {
                Id = b.Id,
                BeneficiaryName = b.BeneficiaryName,
                AccountNumber = b.AccountNumber,
                BankIFSC = b.BankIFSC,
                BeneficiaryStatus = b.BeneficiaryStatus.ToString().ToUpper(),
                BeneficiaryType = b.BeneficiaryType.ToString().ToUpper(),
                IsActive = b.IsActive,
                DocumentUrls = b.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return beneficiariesDto;
        }

        //new addition
        public List<ClientDTO> GetAllInboundBeneficiaries(Guid clientId)
        {
            var beneficiaries = _clientRepository.GetAllInboundBeneficiaries(clientId);
            var beneficiariesDto = beneficiaries.Select(b => new ClientDTO
            {
                Id = b.Id,
                CompanyName = b.UserName,
                AccountNumber = b.AccountNumber,
                IFSC = b.IFSC,
                BeneficiaryStatus = "APPROVED",
                IsActive = b.IsActive,
            }).ToList();
            return beneficiariesDto;
        }

        public void AddInboundBeneficiary(Guid clientId,Guid id)
        {
            var client = _clientRepository.GetClientById(clientId);
            var beneficiary = _clientRepository.GetClientById(id);
            var inboundBeneficiary = new Beneficiary()
            {
                //BeneficiaryName = client.CompanyName,
                BeneficiaryName = beneficiary.UserName,
                AccountNumber = beneficiary.AccountNumber,
                BankIFSC = beneficiary.IFSC,
                BeneficiaryStatus = CorporateStatus.APPROVED,
                BeneficiaryType = BeneficiaryType.INBOUND,
                IsActive = beneficiary.IsActive,
                Client = client
            };
            _clientRepository.AddNewBeneficiary(inboundBeneficiary);
        }

        public void UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            _clientRepository.UpdateBeneficiaryStatus(id, isActive);
        }

        public void AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles)
        {
            var beneficiary = new Beneficiary()
            {
                Id = Guid.NewGuid(),
                BeneficiaryName = beneficiaryDTO.BeneficiaryName,
                AccountNumber = beneficiaryDTO.AccountNumber,
                BankIFSC = beneficiaryDTO.BankIFSC,
                BeneficiaryType = BeneficiaryType.OUTBOUND,
                BeneficiaryStatus = CorporateStatus.PENDING,
                IsActive = true,
                Client = client
            };
            string[] documentTypes = { "Beneficiary Id Proof", "Beneficiary Address Proof" };
            string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/Beneficiary/") + beneficiary.BeneficiaryName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            for (int i = 0; i < uploadedFiles.Count; i++)
            {

                var file = uploadedFiles[i];
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(filePath);
                    // Save relative file path (relative to the Content folder)
                    string relativeFilePath = $"~/Content/Documents/Beneficiary/{beneficiary.BeneficiaryName}/{fileName}";
                    var document = new Document
                    {
                        DocumentType = documentTypes[i], // Get document type based on index
                        FilePath = relativeFilePath,
                        UploadDate = DateTime.Now,
                        Beneficiary = beneficiary
                    };
                    beneficiary.Documents.Add(document);
                }
            }
            _clientRepository.AddNewBeneficiary(beneficiary);
        }
        public Beneficiary GetBeneficiaryById(Guid id)
        {
            return _clientRepository.GetBeneficiaryById(id);
        }

        public void UpdateBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles)
        {
            var existingBeneficiary = _clientRepository.GetBeneficiaryById(beneficiaryDTO.Id);

            string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/Beneficiary/") + existingBeneficiary.BeneficiaryName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Document update process
            string[] documentTypes = { "Beneficiary Id Proof", "Beneficiary Address Proof" };
            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                var file = uploadedFiles[i];
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    file.SaveAs(filePath);
                    string relativeFilePath = $"~/Content/Documents/Beneficiary/{existingBeneficiary.BeneficiaryName}/{fileName}";

                    // Check if the client already has this document type, if so, update it
                    var existingDocument = existingBeneficiary.Documents.FirstOrDefault(d => d.DocumentType == documentTypes[i]);
                    if (existingDocument != null)
                    {
                        // Update existing document
                        existingDocument.FilePath = relativeFilePath;
                        existingDocument.UploadDate = DateTime.Now;
                    }
                    else
                    {
                        // Add new document if not present
                        var document = new Document
                        {
                            DocumentType = documentTypes[i],
                            FilePath = relativeFilePath,
                            UploadDate = DateTime.Now,
                            Beneficiary = existingBeneficiary
                        };
                        existingBeneficiary.Documents.Add(document);
                    }
                }
            }

            if (existingBeneficiary != null)
            {
                // Map the fields from the DTO to the existing Employee model
                existingBeneficiary.BeneficiaryName = beneficiaryDTO.BeneficiaryName;
                existingBeneficiary.AccountNumber = beneficiaryDTO.AccountNumber;
                existingBeneficiary.BankIFSC = beneficiaryDTO.BankIFSC;
                existingBeneficiary.Client = client;
                existingBeneficiary.BeneficiaryStatus = CorporateStatus.PENDING;
                //existingBeneficiary.Documents = 
                _clientRepository.UpdateBeneficiary(existingBeneficiary);
            }
        }

        /**************************AddBalance***************************/
        public void AddBalance(Guid id, double amount)
        {
            
            _clientRepository.AddBalance(id, amount);
        }
        public bool CheckPassword(Guid id, string password)
        {
            var client = GetClientById(id);
            if(client !=null && PasswordHelper.VerifyPassword(password, client.Password))
            {
                return true;
            }
            return false;
        }
        public void SaveNewPassword(Guid clientId, string newPassword)
        {
            _clientRepository.SaveNewPassword(clientId, newPassword);
        }
        /*******************************PAYMENTS*********************************/
        public List<BeneficiaryDTO> GetBeneficiaryList(Guid id)
        {
            var beneficiaries = _clientRepository.GetBeneficiaryList(id);

            // Map each Beneficiary to a BeneficiaryDTO
            var beneficiaryDTOs = beneficiaries.Select(b => new BeneficiaryDTO
            {
                Id = b.Id,
                BeneficiaryName = b.BeneficiaryName,
                AccountNumber = b.AccountNumber,
                BankIFSC = b.BankIFSC,
                IsActive = b.IsActive,
                BeneficiaryType = b.BeneficiaryType.ToString().ToUpper(),
                BeneficiaryStatus = b.BeneficiaryStatus.ToString().ToUpper()
            }).ToList();
            return beneficiaryDTOs;
        }

        /********************************REPORTS*******************************/

        public List<EmployeeSalaryDisbursementDTO> GetAllSalaryDisbursements(Guid id)
        {
            return _clientRepository.GetAllSalaryDisbursements(id);
        }

        public List<PaymentDTO> GetPayments(Guid id)
        {
            return _clientRepository.GetPayments(id);
        }

        public void AddReportInfo(Guid id)
        {
            _clientRepository.AddReportInfo(id);
        }

        public void AddPaymentReportInfo(Guid id)
        {
            _clientRepository.AddPaymentReportInfo(id);
        }

    }
}