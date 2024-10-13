using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using iText.Layout;
using iText.Layout.Element;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApplication.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;
        private readonly IClientRepository _clientRepository;
        public AdminService(IAdminRepository adminRepository, IEmailService emailService, IClientRepository clientRepository)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
            _clientRepository = clientRepository;
        }
        
        public List<ClientDTO> ViewAllClients()
        {
            var clients = _adminRepository.GetAllClients();
            var clientDtos = clients.Select(client => new ClientDTO
            {
                Id = client.Id,
                UserName = client.UserName,
                Email = client.Email,
                CompanyName = client.CompanyName,
                ContactInformation = client.ContactInformation,
                Location = client.Location,
                Balance = client.Balance,
                OnboardingStatus = client.OnBoardingStatus,
                IsActive = client.IsActive,
                AccountNumber = client.AccountNumber,
                IFSC = client.IFSC
            }).ToList();
            return clientDtos;
        }

        public void EditClient(ClientDTO clientDTO, Guid id)
        {
            var client = new Client
            {
                Id = id,
                UserName = clientDTO.UserName,
                Email = clientDTO.Email,
                CompanyName = clientDTO.CompanyName,
                ContactInformation = clientDTO.ContactInformation,
                Location = clientDTO.Location
            };
            _adminRepository.UpdateClientDetails(client);
        }

        public bool ToggleClientActiveStatus(Guid id, bool isActive, out string message)
        {
            var client = _adminRepository.GetClientById(id);
            if (client == null)
            {
                message = "Client not found";
                return false;
            }
            if (client.IsActive != isActive)
            {
                client.IsActive = isActive;
            }
            else
            {
                client.IsActive = !isActive;
            }
            _adminRepository.UpdateClientBeneficiaryStatus(client);
            message = "Client status updated successfully";
            return true;
        }

        public void RemoveClient(Guid id)
        {
            _adminRepository.DeleteClientDetails(id);
        }




        //*******************************VERIFY**************************
        public List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper)
        {
            var clients = _adminRepository.GetPendingClients();
            var clientDtos = clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                CompanyName = c.CompanyName,
                ContactInformation = c.ContactInformation,
                Location = c.Location,
                DocumentPaths = c.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return clientDtos;
        }


        public bool UpdateClientOnboardingStatus(Guid id, string status, string rejectionReason = "")
        {
            var client = _adminRepository.GetClientById(id);
            if (client == null)
            {
                // Client not found
                return false;
            }

            // Update onboarding status based on the status string
            if (status == "APPROVED")
            {
                client.OnBoardingStatus = CorporateStatus.APPROVED;
                _emailService.SendClientOnboardingStatusEmail(client.Email, "Client Approved!!",
                    $"Dear {client.UserName}, Your client account has been approved as all submitted details and documents meet our onboarding requirements. Now you can access all our services.");
            }
            else if (status == "REJECTED")
            {
                client.OnBoardingStatus = CorporateStatus.REJECTED;
                _emailService.SendClientOnboardingStatusEmail(client.Email, "Client Rejected!!",
                    $"Dear {client.UserName}, Your client account has been rejected due to the following reason:\n\n{rejectionReason}\n\n. Please contact support for further clarification.");
            }

            _adminRepository.UpdateClient(client);
            return true;
        }

        //*******************Salary disbursement***************

        public IEnumerable<EmployeeSalaryDisbursementDTO> GetPendingSalaryDisbursements()
        {
            return _adminRepository.GetSalaryDisbursementsByStatus(CorporateStatus.PENDING);
        }

        public bool ApproveSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false)
        {
            //return _adminRepository.ApproveSalaryDisbursement(salaryDisbursementId);
            var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(salaryDisbursementId);
            if (salaryDisbursement == null)
            {
                return false;
            }
            var client = _clientRepository.GetClientById(salaryDisbursement.Employee.Client.Id);

            var approved = _adminRepository.ApproveSalaryDisbursement(salaryDisbursementId);

            if (approved)
            {
                if (isBatch)
                {
                    var employeeSalaries = new List<EmployeeDTO>
                    {
                        new EmployeeDTO
                        {
                            FirstName = salaryDisbursement.Employee.FirstName,
                            LastName = salaryDisbursement.Employee.LastName,
                            Salary = salaryDisbursement.Employee.Salary
                        }
                    };
                    _emailService.SendBatchSalaryDisbursementApprovalEmail(client.Email, employeeSalaries, salaryDisbursement.DisbursementDate.ToString("MMMM yyyy"));

                }
                else
                {
                    // Single employee logic
                    _emailService.SendSalaryDisbursementApprovalEmail(client.Email, new EmployeeDTO
                    {
                        FirstName = salaryDisbursement.Employee.FirstName,
                        LastName = salaryDisbursement.Employee.LastName,
                        Salary = salaryDisbursement.Employee.Salary
                    }, salaryDisbursement.Employee.Salary, salaryDisbursement.DisbursementDate.ToString("MMMM yyyy"));
                }

                var pdf = GeneratePayslipPdf(salaryDisbursement.Employee, salaryDisbursement);
                _emailService.SendPayslipToEmployee(salaryDisbursement.Employee.Email, pdf);
                return true;
            }
            return false;
        }

        //generating payslip 
        public byte[] GeneratePayslipPdf(Employee employee, SalaryDisbursement salaryDisbursement)
        {
            using (var stream = new MemoryStream()) //FOR GENERATING PAYSLIP I HAVE USED ITEXT7
            {
                using (var writer = new PdfWriter(stream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new iText.Layout.Document(pdf);
                        // Add payslip details
                        document.Add(new Paragraph($"Payslip for: {employee.FirstName} {employee.LastName}"));
                        document.Add(new Paragraph($"Employee ID: {employee.Id}"));
                        document.Add(new Paragraph($"Designation: {employee.Position}"));
                        // document.Add(new Paragraph($"Date of Joining: {employee.JoiningDate.ToString("dd MMMM yyyy")}"));
                        document.Add(new Paragraph($"Salary Amount: {employee.Salary:C}"));
                        document.Add(new Paragraph($"Date of Approval: {salaryDisbursement.DisbursementDate.ToString("dd MMMM yyyy")}"));
                        document.Add(new Paragraph($"Amount in Words: {ConvertAmountToWords(employee.Salary)}"));

                        // Close the document
                        document.Close();
                    }
                }
                return stream.ToArray();
            }

        }

        public string ConvertAmountToWords(double amount)
        {
            return new ConvertAmountToWords().Convert(amount);
        }



        public bool RejectSalaryDisbursement(Guid salaryDisbursementId, string reason, bool isBatch = false)
        {
            // return  _adminRepository.RejectSalaryDisbursement(salaryDisbursementId); 
            var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(salaryDisbursementId);
            if (salaryDisbursement == null)
            {
                return false;
            }
            var client = _clientRepository.GetClientById(salaryDisbursement.Employee.Client.Id);

            // Reject the salary disbursement
            var rejected = _adminRepository.RejectSalaryDisbursement(salaryDisbursementId);
            if (rejected)
            {
                if (isBatch)
                {
                    string subject = "Salary Disbursement Request Rejected";
                    string body = $"Dear {client.CompanyName}, your salary disbursement request has been rejected.";
                    if (!string.IsNullOrEmpty(reason))
                    {
                        body += $"\n\nReason for rejection: {reason}";
                    }
                    // Send batch rejection email
                    _emailService.SendClientOnboardingStatusEmail(client.Email, subject, body);
                }
                else
                {
                    // Send single rejection email
                    _emailService.SendClientOnboardingStatusEmail(client.Email, "Salary Disbursement Request Rejected", $"Dear {client.CompanyName}, your salary disbursement request has been rejected.");
                }
            }
            return rejected;


        }


        /**************************VERIFY OUTBOUND BENEFICIARIES*****************************/
        public List<BeneficiaryDTO> GetBeneficiariesForVerification(UrlHelper urlHelper)
        {
            var beneficiaries = _adminRepository.GetPendingBeneficiaries();
            var beneficiariesDto = beneficiaries.Select(b => new BeneficiaryDTO
            {
                Id = b.Id,
                BeneficiaryName = b.BeneficiaryName,
                AccountNumber = b.AccountNumber,
                BankIFSC = b.BankIFSC,
                ClientName = b.Client.UserName,
                BeneficiaryType = b.BeneficiaryType.ToString().ToUpper(),
                DocumentUrls = b.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return beneficiariesDto;
        }
       

        public bool UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status, string rejectionReason = "")
        {
            var beneficiary = _adminRepository.GetBeneficiaryById(id);
            if (beneficiary.Client == null)
            {
                // Client not found
                return false;
            }
            // Update onboarding status based on the status string
            if (status == "APPROVED")
            {
                beneficiary.BeneficiaryStatus = CorporateStatus.APPROVED;
                _emailService.SendClientOnboardingStatusEmail(beneficiary.Client.Email, "Beneficiary Approved!!", $"Dear {beneficiary.Client.UserName}, Your beneficiary {beneficiary.BeneficiaryName} has been approved after verification of the details and documents submitted by you.");
            }
            else if (status == "REJECTED")
            {
                beneficiary.BeneficiaryStatus = CorporateStatus.REJECTED;
                _emailService.SendClientOnboardingStatusEmail(beneficiary.Client.Email, "Beneficiary Rejected!!", $"Dear {beneficiary.Client.UserName}, Your beneficiary {beneficiary.BeneficiaryName} has been rejected due to the following reason:\n\n{rejectionReason}\n\n.");
            }
            _adminRepository.UpdateBeneficiary(beneficiary);
            return true;
        }

        /**************************PROFILE*****************************/

        public AdminDTO GetAdminById(Guid id)
        {
            var admin = _adminRepository.GetAdminById(id);
            var adminDto = new AdminDTO()
            {
                UserName = admin.UserName,
                Email = admin.Email,
                BankName = admin.BankName
            };
            return adminDto;
        }

        /* Payment verification */

        public IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CorporateStatus status)
        {
            return _adminRepository.GetPendingPaymentsByStatus(status);
        }

        public void UpdatePaymentStatuses(List<Guid> paymentIds, CorporateStatus status, string reason = "")
        {
            foreach (var paymentId in paymentIds)
            {
                _adminRepository.UpdatePaymentStatus(paymentId, status);

                //fetching the payment details
                var payment = _adminRepository.GetPaymentById(paymentId);

                if (payment != null)
                {
                    var client = _clientRepository.GetClientById(payment.ClientId);
                    if (client != null)
                    {
                        string subject;
                        string body;

                        if (status == CorporateStatus.APPROVED)
                        {
                            subject = "Payment Approved";
                            body = $"Dear {client.UserName}, your payment of {payment.Amount:C} has been approved.";
                        }
                        else
                        {
                            subject = "Payment Rejected";
                            body = $"Dear {client.UserName}, your payment of {payment.Amount:C} has been rejected. Reason: {reason}";
                        }
                        _emailService.SendClientOnboardingStatusEmail(client.Email, subject, body);

                    }
                }

            }
        }

        /******************************REPORTS*****************************/

        public List<EmployeeSalaryDisbursementDTO> GetAllSalaryDisbursements(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return _adminRepository.GetAllSalaryDisbursements(companyName, startDate, endDate);
        }

        public List<PaymentDTO> GetPayments(string companyName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return _adminRepository.GetPayments(companyName, startDate, endDate);
        }

        public void AddReportInfo(Guid id)
        {
            _adminRepository.AddReportInfo(id);
        }

        public void AddPaymentReportInfo(Guid id)
        {
            _adminRepository.AddPaymentReportInfo(id);
        }


    }
};



