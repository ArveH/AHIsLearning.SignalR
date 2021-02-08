using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AHIsLearning.SignalR.BlazorClient.Hubs;

namespace AHIsLearning.SignalR.BlazorClient.Pages
{
    public class StreamingBase : ComponentBase
    {
        [Inject] internal NavigationManager NavigationManager { get; set; }

        private HubConnection _hubConnection;
        protected int CurrentNum;
        private CancellationTokenSource cts;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/streaminghub"))
                .Build();

            _hubConnection.Closed += async (ex) =>
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("restart");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            cts = new CancellationTokenSource();
            await _hubConnection.StartAsync(cts.Token);
            await Task.Delay(2000, cts.Token);
        }

        protected async Task Start()
        {
            var channel = await _hubConnection.StreamAsChannelAsync<SomeData>("GetSomeDataWithChannelReader", 100, 1000, cts.Token);
            while (await channel.WaitToReadAsync())
            {
                while (channel.TryRead(out SomeData data))
                {
                    CurrentNum = data.Value;
                    StateHasChanged();
                    Debug.WriteLine($"received {data}");
                }
            }
        }

        public bool IsConnected =>
            _hubConnection.State == HubConnectionState.Connected;
    }
}
