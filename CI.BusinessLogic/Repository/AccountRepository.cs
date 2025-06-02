using CI.BusinessLogic.Repository.Interface;
using CI.Models;
using CI.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CI.BusinessLogic.Repository
{
    public class AccountRepository: IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountRepository(UserManager<ApplicationUser> userManager, 
                                SignInManager<ApplicationUser> signInManager,
                                RoleManager<IdentityRole> roleManager,
                                IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<IdentityResult> SignUpAsync(RegistrationModel register)
        {
            var user = new ApplicationUser()
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                PhoneNumber = register.PhoneNumber,
                Email = register.Email,
                UserName = register.Email
            };
            
            IdentityResult result = await _userManager.CreateAsync(user, register.Password);
            var roleExists = await _roleManager.RoleExistsAsync("Admin");

            if (!roleExists)
            {
                // Role doesn't exist, create it
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!createRoleResult.Succeeded)
                {
                    // Handle role creation failure
                    return createRoleResult;
                }
            }
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (addToRoleResult.Succeeded)
            {
                // User added to role successfully
                return result; // or return addToRoleResult; if you prefer
            }
            else
            {
                // Failed to add user to role, handle the error
                // You might want to delete the user created in case of a role assignment failure
                // e.g., await _userManager.DeleteAsync(user);
                return addToRoleResult;
            }
            return result;
        }
            
        public async Task<string> SignInAsync(LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
            var user = await _userManager.FindByEmailAsync(login.Email);
            var role = await _userManager.GetRolesAsync(user);

            if (!result.Succeeded)
                return null;

            //var authClaims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, login.Email),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            //};
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, login.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Role, role[0]));

            // Add roles as multiple claims
            //foreach (var role in _roleManager.Roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role.Name));
            //}

            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordModel forgetPassword)
        {
            var user = await _userManager.FindByEmailAsync(forgetPassword.email);

            if (user == null) { return false; }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var param = new Dictionary<string, string?>
            //{
            //    {"token", token},
            //    {"email", forgetPassword.email},
            //};

            var callback = forgetPassword.ClientURI + "?token=" + token + "&email=" + forgetPassword.email;

            return new EmailUtility().SendEmailPasswordReset(forgetPassword.email, callback);
        }
    }
}
