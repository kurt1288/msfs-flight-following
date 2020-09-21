using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MSFSFlightFollowing.Models
{
   public class WebSocketConnector : Hub
   {
      public static int userCount = 0;

      public override Task OnConnectedAsync()
      {
         userCount++;
         return base.OnConnectedAsync();
      }

      public override Task OnDisconnectedAsync(Exception exception)
      {
         userCount--;
         return base.OnDisconnectedAsync(exception);
      }

      public async Task SendData(string data)
      {
         await Clients.All.SendAsync("ReceiveData", data);
      }
   }
}