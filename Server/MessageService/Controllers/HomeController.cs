using AccountServices.Model;
using AccountServices.Database;
using MessageService.Database;
using MessageService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace MessageService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private MessageService.Database.AppDbContext _ctx;
        
        public HomeController(MessageService.Database.AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        [Route("CreateMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] Message sendmess)
        {
            var Message = new Message
            {
                ChatId = sendmess.ChatId,
                Text = sendmess.Text,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            _ctx.Messages.Add(Message);

            await _ctx.SaveChangesAsync();

            return Ok(Message);
        }


        [HttpPost]
        [Route("Room")]
        public async Task<IActionResult> CreateRoom([FromBody] Chat chats)
        {
            var chat = new Chat
            {
                Name = chats.Name,
                Type = ChatType.Room
            };

            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });
            _ctx.Chats.Add(chat);

            await _ctx.SaveChangesAsync();

            return Ok(chat);
        }


        [HttpGet("{id}")]
        //[Route("Chat")]
        public IActionResult Chat(int id)
        {
            var Chat = _ctx.Chats
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);

            return Ok(Chat);
        }


        public IEnumerable<String> Get()
        {
            return new String[] { "Value3", "Value4" };
        }

      


        [HttpGet]
        [Route("rip")]
        public void TestSharp()
        {
            IRestClient restClient = new RestClient();
            IRestRequest restRequest = new RestRequest("https://localhost:44303/api/values/get") { RequestFormat = DataFormat.Json };
            restRequest.AddHeader("Content-Type", "application/json");
            IRestResponse restResponse = restClient.Get(restRequest);
            var content = restResponse.Content;
          
            
        }
    }
}   
