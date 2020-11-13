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
    public class AccessTokenController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AccessTokenController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string token)
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var JSON = System.IO.File.ReadAllText(contentRootPath + "/admin.secret.json");
            dynamic adminSecret = JsonConvert.DeserializeObject(JSON);
            string adminUsername = adminSecret["ADMIN_USERNAME"].ToString();

            Token user = Base64DecodeObject(token);
            TimeSpan ts = user.expiryTime - DateTime.Now;
            if (user.username == adminUsername && ts.TotalSeconds < 600)
            {
                string res = Base64EncodeObject(new Token
                {
                    username = adminUsername,
                    expiryTime = DateTime.Now.AddSeconds(600)
                });
                return Ok(res);
            }
            return BadRequest();
        }

        public static string Base64EncodeObject(Token obj)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static Token Base64DecodeObject(string base64String)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            return JsonConvert.DeserializeObject<Token>(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
        }

    }
}