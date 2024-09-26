using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CorporateBankingApplication.DTOs;
using CorporateBankingApplication.Models;

namespace CorporateBankingApplication.Services
{
    public interface IUserService
    {
        string IsLogging(UserDTO userDto);
        User GetUserByUsername(string username);

        void CreateNewClient(ClientDTO clientDto, IList<HttpPostedFileBase> uploadedFiles);

        //string DetermineDocumentType(string fileName);



    }
}
