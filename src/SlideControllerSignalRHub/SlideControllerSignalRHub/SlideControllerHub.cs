using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SlideControllerSignalRHub
{
    internal class SlideControllerHub : Hub
    {
        public async Task SendToOthers(string message)
        {
            await Clients.Others.SendAsync("Receive",message);
        }
    }
}