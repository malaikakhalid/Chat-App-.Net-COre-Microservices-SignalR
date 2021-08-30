using MessageService.Database;
using MessageService.Hubs;
using MessageService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
       
          private IHubContext<ChatHub> _chat;

            public ChatController(
                IHubContext<ChatHub> chat)
            {
                _chat = chat;
            }


            [HttpPost("[action]/{connectionId}/{roomId}")]
            public async Task<string> JoinRoom(string connectionId, string roomId)
            {
                await _chat.Groups.AddToGroupAsync(connectionId, roomId);
                return "Room Joined ";
            }
            [HttpPost("[action]/{connectionId}/{roomId}")]
            public async Task<IActionResult> LeaveRoom(string connectionId, string roomId)
            {
                await _chat.Groups.RemoveFromGroupAsync(connectionId, roomId);
                return Ok();
            }
            [Authorize]
            [HttpPost("[action]")]

            public async Task<ActionResult> SendMessage(

                 [FromBody] Message sendmess,
                 [FromServices] AppDbContext _ctx
                )
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

                await _chat.Clients.Group(sendmess.ChatId.ToString())
                    .SendAsync("RecieveMessage", new
                    {
                        ChatId = sendmess.ChatId,
                        Text = sendmess.Text,
                        Name = User.Identity.Name,
                        Timestamp = DateTime.Now
                    });
                return Ok(Message);
            }
        }
    }