using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DpsLibs.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Timensit_API.Classes;

namespace Timensit_API.Controllers.Users
{
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private UserManager CustomUserManager { get; set; }
        public LoginController()
        {
        }
        public LoginController(IOptions<NCCConfig> config, IConfiguration configLogin)
        {
            CustomUserManager = new UserManager(config);
            _config = configLogin;
        }
        public LoginData AuthenticateUser(string username, string password, long cur_Vaitro=0)
        {
            try
            {
                var account = CustomUserManager.FindAsync(username, password, cur_Vaitro);
                if (account != null)
                {
                    account.Rules = CustomUserManager.GetRules(account.Id, account.VaiTro);
                    account.Token = GenerateJSONWebToken(account);
                    return account;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private string GenerateJSONWebToken(LoginData userInfo)
        {
            string json = JsonConvert.SerializeObject(userInfo);
            LoginData account = JsonConvert.DeserializeObject<LoginData>(json);            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            if (account.Rules != null)
            {
                foreach (var role in account.Rules)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }
            account.Rules = null;
            account.Token = "";
            claims.Add(new Claim("user", JsonConvert.SerializeObject(account)));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, account.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(10),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshJSONWebToken(ref LoginData account)
        {
            account.Rules = CustomUserManager.GetRules(account.Id, account.VaiTro);
            string Token = GenerateJSONWebToken(account);
            return Token;
        }

        public List<string> _GetAllRuleUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                token = token.Replace("Bearer ", string.Empty);

                var tokenS = handler.ReadJwtToken(token) as JwtSecurityToken;

                List<string> rules = new List<string>();

                foreach (var r in tokenS.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList())
                {
                    rules.Add(r.Value);
                }
                return rules;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public LoginData _GetInfoUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                token = token.Replace("Bearer ", string.Empty);

                var tokenS = handler.ReadJwtToken(token) as JwtSecurityToken;
                LoginData account = JsonConvert.DeserializeObject<LoginData>(tokenS.Claims.First(claim => claim.Type == "user").Value);
                account.Token = token;
                if (account.ExpDate <= DateTime.Now)
                    return null;
                return account;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string GetHeader(HttpRequest request)
        {
            try
            {
                Microsoft.Extensions.Primitives.StringValues headerValues;
                request.Headers.TryGetValue("Authorization", out headerValues);
                return headerValues.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
    }
}