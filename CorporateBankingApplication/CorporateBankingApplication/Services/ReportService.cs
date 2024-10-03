using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Enum;
using CorporateBankingApplication.Models;
using CorporateBankingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public List<EmployeeSalaryDisbursementDTO> GetSalaryDisbursements(string role,Guid id)
        {
            if (role == "Admin")
            {
                return _reportRepository.GetSalaryDisbursements();

            }
            else if (role == "Client")
            {

                return _reportRepository.GetSalaryDisbursementsOfClient(id);
            }
            return null;
        }
        public void AddReportInfo(string role, Guid userId)
        {
            _reportRepository.AddReportInfo(role,userId);
            
        }

        public List<PaymentDTO> GetPayments(string role, Guid id)
        {
            if (role == "Admin")
            {
                return _reportRepository.GetPayments();

            }
            else if (role == "Client")
            {

                return _reportRepository.GetPaymentsOfClient(id);
            }
            return null;
        }
        public void AddPaymentReportInfo(string role, Guid userId)
        {
            _reportRepository.AddPaymentReportInfo(role, userId);

        }
    }
}