using Newtonsoft.Json.Linq;
using pill_note_client.Handlers;
using pill_note_client.Views.EditForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace pill_note_client.Views
{
    public partial class UserForm : Form
    {
        JToken userData;
        List<JToken> RemindersData;
        System.Threading.Timer timer;
        public UserForm(JObject userData)
        {
            InitializeComponent();

            this.userData = userData;
            Config();
        }
        private Tuple<string, string> GetCurrentUserData()
        {
            try
            {
                return new Tuple<string, string>(userData["user"]["fullname"].ToString(), userData["user"]["email"].ToString());
            }
            catch
            {
                return new Tuple<string, string>(userData["fullname"].ToString(), userData["email"].ToString());
            }
        }

        private async Task LoadRemindersData()
        {
            var data = await RemindersHandler.GetReminders(1);
            if ((int)data["StatusCode"] != 200)
            {
                if (MessageBox.Show("It looks like you don't have any notes. Add new?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //throw new NotImplementedException();
                    AddNoteForm addNoteForm = new AddNoteForm();
                    Hide();
                    addNoteForm.ShowDialog();
                    Show();
                }
                return;
            }
            RemindersData = ((JArray)data["array"]).ToList();

            dataGridView1.DataSource = RemindersData.Select(s => new
            {
                The_name_of_the_medicine = s["medicine"].ToString(),
                When_to_take = s["time"],
                Notes = s["notes"].ToString()
            }).ToList();
        }
        private void Config()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ForeColor = Color.Black;


            notifyIcon1.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon1.ContextMenuStrip.Items.Add("Открыть", null, (sender, args) =>
            {
                Show();
                Activate();
                notifyIcon1.Visible = false;
            });
            notifyIcon1.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon1.ContextMenuStrip.Items.Add("Выход", null, (sender, args) =>
            {
                Close();
                Dispose();
            });

            var userInfo = GetCurrentUserData();

            fullnameTb.Text = userInfo.Item1;
            emailTb.Text = userInfo.Item2;
            try
            {
                if (!string.IsNullOrEmpty(userData["photoLink"].ToString()))
                {
                    userPb.Load(userData["photoLink"].ToString());
                }
            }
            catch { }

            timer = new System.Threading.Timer(async state =>
            {
                await ApiHandler.Refresh();
            }, null, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15));
        }
        private async void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Ви точно хочете вийти із облікового запису?",
                    "Log Out", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                //AccountHandler.Logout();

                var logoutResult = await ApiHandler.Logout();
                if (ApiHandler.IsValidUserApiResponce(logoutResult))
                {
                    MessageBox.Show("You have successfully logged out!");
                    userData = null;
                }
                else
                    MessageBox.Show("Something went wrong!");

                File.Delete(Variables.DATA_FILE);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region работа из Socket'ами
        private async void UserForm_Load(object sender, EventArgs e)
        {
            await SocketHandler.ConnectToServer(GetReminder, DisconnectedFromSocket);
            await LoadRemindersData();
        }
        private void AddLog(string reminderId)
        {
            var form = new AddLogForm(reminderId);
            Hide();
            form.ShowDialog();
            Show();
        }
        private void DisconnectedFromSocket()
        {
            var popup = Variables.GetPopupNotifier("Disconnected from the notification service",
                "You have disconnected from the notification service. Reconnect?",
                Variables.BodyColor, Variables.FontColor, async () =>
                {
                    if (MessageBox.Show("Do you want to try to reconnect to the server?", "Try to reconnect?"
                        , MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        await SocketHandler.TryToReconnect();
                    }
                });

            this.Invoke(new Action(() => { popup.Popup(); }));
        }
        private async void SuggestToAddLog(string reminderId)
        {
            try
            {
                var markResult = await RemindersHandler.MarkAsViewed(reminderId);
                if ((int)markResult["StatusCode"] == 200)
                {
                    if (MessageBox.Show("Okay, would you like to add some note?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        AddLog(reminderId);
                    }
                    await LoadRemindersData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void GetReminder(string data)
        {
            JToken jsonObject = JArray.Parse(data)[0];

            var popup = Variables.GetPopupNotifier(jsonObject["id"].ToString(), "Reminder to take your medicine!",
                $"Dear {GetCurrentUserData().Item1}, please take an {jsonObject["medicine"]}!\n\nDon't forget these notes: {jsonObject["notes"]}",
                Variables.BodyColor, Variables.FontColor, SuggestToAddLog);

            this.Invoke(new Action(() => { popup.Popup(); }));
        }
        #endregion
        private async void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                AddNoteForm form = new AddNoteForm();
                Hide();
                form.ShowDialog();
                Show();

                await LoadRemindersData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (userData == null)
                return;
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Activate();
            notifyIcon1.Visible = false;
        }
        private JToken GetSelectedReminder()
        {
            if (dataGridView1.Rows.Count == 0) throw new Exception("You have not selected a reminder");
            return RemindersData[dataGridView1.CurrentCell.RowIndex];
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var reminder = GetSelectedReminder();

                AddNoteForm addNoteForm = new AddNoteForm(reminder);
                Hide();
                addNoteForm.ShowDialog();
                Show();

                await LoadRemindersData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var reminder = GetSelectedReminder();

                var deleteResult = await RemindersHandler.DeleteReminder(reminder);
                if ((int)deleteResult["StatusCode"] == 200)
                {
                    await LoadRemindersData();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var reminder = GetSelectedReminder();

                SuggestToAddLog(reminder["id"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void viewHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HistoryForm historyForm = new HistoryForm();
                Hide();
                historyForm.ShowDialog();
                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}