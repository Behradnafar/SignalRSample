using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSample.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRSample.Hubs
{
    public class SiteChatHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;

        public SiteChatHub(IChatRoomService chatRoomService,
                           IMessageService messageService)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
        }

        public async Task SendNewMessage(string sender, string message)
        {
            //Find chat room
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);


            //Save in Db
            MessageDto messageDto = new MessageDto()
            {
                Message = message,
                Sender = sender,
                Time = DateTime.Now,
            };
            await _messageService.SaveChatMessage(roomId, messageDto);

            //Call getNewMessage for clients and send sende,msg,DateTime
            await Clients.Groups(roomId.ToString())
                .SendAsync("getNewMessage", messageDto.Sender, messageDto.Message, messageDto.Time);

        }


        //Join support member to the room
        [Authorize]
        public async Task JoinRoom(Guid roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        //Leave support member from the room
        [Authorize]
        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }


        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                await base.OnConnectedAsync();
                return; 
            }

            //Create room for each connected user
            var roomId = await _chatRoomService.CreateChatRoom(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            //Send a message when chat box opened
            await Clients.Caller.SendAsync("getNewMessage", "From support", "How can I help you?", DateTime.Now.ToShortTimeString()); ;
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
