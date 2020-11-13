using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using trxinvest.Models;

namespace trxinvest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public LoginController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var JSON = System.IO.File.ReadAllText(contentRootPath + "/admin.secret.json");
            dynamic adminSecret = JsonConvert.DeserializeObject(JSON);
            string adminUsername = adminSecret["ADMIN_USERNAME"].ToString();
            string adminPassword = adminSecret["ADMIN_PASSWORD"].ToString();
            
            if (adminUsername == user.username && adminPassword == user.password)
            {
                string token = Base64EncodeObject(new Token
                {
                    username = adminUsername,
                    expiryTime = DateTime.Now.AddSeconds(600)
                });
                return Ok(token);
            }

            return BadRequest();
        }

        public static string Base64EncodeObject(Token obj)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
