using Microsoft.AspNetCore.SignalR;

namespace AIMS.Server.Hubs
{
    public class PolicyHub : Hub
    {
        public async Task SendPolicyUpdate(string message)
        {
            await Clients.All.SendAsync("ReceivePolicy", message);
        }
    }

}
