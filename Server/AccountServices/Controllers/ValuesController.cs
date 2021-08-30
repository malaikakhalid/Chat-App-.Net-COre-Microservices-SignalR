using AccountServices.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountServices.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private AppDbContext _ctx;


        public ValuesController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [Authorize]
        [HttpGet]
        [Route("Find")]
        public IActionResult Find()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var users =
                _ctx.Users.
                Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();
            return Ok(users);
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<String> Get()
        {
            return new String[] { "Value3", "Value4" };
        }
    }
}
