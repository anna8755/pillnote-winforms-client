using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pill_note_client.Handlers
{
    public static class SocketHandler
    {
        public static async Task<string> ConnectToServer(Action<string> reminderHandler, Action onDisconnected)
        {
            Variables.client = new SocketIO($"http://localhost:{Variables.PORT}/", new SocketIOOptions
            {
                ExtraHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {ApiHandler.GetAuthDataFromFile()["accessToken"]}" }
                    },
                Reconnection = false
            });

            //при срабатывании этого event'а нужно вызвать переданый метод
            Variables.client.On("reminder", (msg) =>
            {
                reminderHandler?.Invoke(msg.ToString());
                //System.Windows.Forms.MessageBox.Show(msg.ToString());
            });

            Variables.client.OnDisconnected += (sender, e) => onDisconnected?.Invoke();

            await Variables.client.ConnectAsync();

            return Variables.client.Id;
        }

        public static async Task TryToReconnect()
        {
            await Variables.client.ConnectAsync();
        }
    }
}
