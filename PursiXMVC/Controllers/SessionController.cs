using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace PursiXMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {

        [HttpGet("set/{data}")]
        public IActionResult setsession(string data)
        {
            HttpContext.Session.SetString("keyname", data);
            return Ok("session data set");
        }

        [HttpGet("get")]
        public IActionResult getsessiondata()
        {
            var sessionData = HttpContext.Session.GetString("keyname");
            return Ok(sessionData);
        }





    }
}
