using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace AHIsLearning.SignalR.BlazorClient.Pages
{
    public class MessagesBase : ComponentBase, IAsyncDisposable
    {
        [Inject] internal NavigationManager NavigationManager { get; set; }

        private HubConnection _hubConnection;
        protected List<string> Messages = new();
        protected string UserInput;
        protected string MessageInput;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Messages.Add(encodedMsg);
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
        }

        protected Task Send() =>
            _hubConnection.SendAsync("SendMessage", UserInput, MessageInput);

        public bool IsConnected =>
            _hubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
