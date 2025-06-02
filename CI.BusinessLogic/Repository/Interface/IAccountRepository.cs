using CI.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI.BusinessLogic.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(RegistrationModel register);

        Task<string> SignInAsync(LoginModel login);

        Task<bool> ForgetPasswordAsync(ForgetPasswordModel forgetPassword);
    }
}
