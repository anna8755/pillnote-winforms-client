using pill_note_client.Handlers;
using pill_note_client.Views;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace pill_note_client
{
    //до рефакторинга
    internal static class Program
    {
        static Semaphore semaphore = new Semaphore(0, 1);

        static async void Start()
        {
            var data0 = await ApiHandler.TryGetUserAndCheckAuthStatus();

            if (!ApiHandler.IsValidUserApiResponce(data0))
            {
                Application.Run(new LoginForm());
            }
            else
            {
                Application.Run(new UserForm(data0));
                semaphore.Release();
                return;
            }

            var data1 = await ApiHandler.TryGetUserAndCheckAuthStatus();
            if (ApiHandler.IsValidUserApiResponce(data1))
            {
                Application.Run(new UserForm(data1));
            }

            semaphore.Release();
        }

        [STAThread]
        static void Main()
        {
            Variables.InitializeVariables();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Start();

            semaphore.WaitOne();
        }

    }
}
