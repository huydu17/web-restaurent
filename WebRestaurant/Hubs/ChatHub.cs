using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using WebRestaurant.Models;

namespace WebRestaurant.Hubs
{
    public class ChatHub : Hub
    {
        public Task JoinRoom(string conversationId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
        public async Task UpdateConversation(Conversation conversation)
        {
            await Clients.All.SendAsync("ConversationUpdated", conversation);
        }
    }
}
