using SocketIOClient;
using System;
using System.Drawing;
using Tulpep.NotificationWindow;

namespace pill_note_client.Handlers
{
    public static class Variables
    {
        public static int PORT = 5000;
        public static string BASE_URL;
        public const string DATA_FILE = "userData.json";
        public static SocketIO client;
        public static Color BodyColor = Color.FromArgb(255, 192, 203);
        public static Color FontColor = Color.FromArgb(79, 61, 64);

        public static void InitializeVariables(string url = "", int port = 5000)
        {
            PORT = port;
            if (url == "")
                BASE_URL = $"http://localhost:{port}/api/";
            else BASE_URL = url;

            //client = new SocketIO($"http://localhost:{port}/");
        }
        public static PopupNotifier GetPopupNotifier(string title, string message, Color bodyColor, Color fontColor)
        {
            PopupNotifier popup = new PopupNotifier();
            popup.BodyColor = bodyColor;
            popup.ContentColor = fontColor;
            popup.TitleColor = fontColor;

            popup.TitleText = title;
            popup.ContentText = message;

            return popup;
        }
        public static PopupNotifier GetPopupNotifier(string id,string title, string message, Color bodyColor, Color fontColor, Action<string> popupHandler) 
        {
            PopupNotifier popup = new PopupNotifier();
            popup.BodyColor = bodyColor;
            popup.ContentColor = fontColor;
            popup.TitleColor = fontColor;

            popup.TitleText = title;
            popup.ContentText = message;

            popup.Click += (sender, e) => popupHandler?.Invoke(id);

            return popup;
        }

        public static PopupNotifier GetPopupNotifier(string title, string message, Color bodyColor, Color fontColor, Action popupHandler)
        {
            PopupNotifier popup = new PopupNotifier();
            popup.BodyColor = bodyColor;
            popup.ContentColor = fontColor;
            popup.TitleColor = fontColor;

            popup.TitleText = title;
            popup.ContentText = message;

            popup.Click += (sender, e) => popupHandler?.Invoke();

            return popup;
        }
    }
}