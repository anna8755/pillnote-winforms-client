using Newtonsoft.Json.Linq;
using pill_note_client.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace pill_note_client.Views.EditForms
{
    public partial class AddNoteForm : Form
    {
        JToken reminder = null;
        public AddNoteForm()
        {
            InitializeComponent();
        }
        public AddNoteForm(JToken reminder)
        {
            InitializeComponent();
            this.reminder = reminder;

            medicineTb.Text = reminder["medicine"].ToString();
            notesTb.Text = reminder["notes"].ToString();
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var date = dateTimePicker1.Value;
                var time = new DateTime(date.Year, date.Month, date.Day, int.Parse(comboBox1.Text), int.Parse(comboBox2.Text), 0);

                if (time <= DateTime.Now)
                    throw new Exception("Data validation error!");

                JObject res;

                if (reminder == null)
                    res = await RemindersHandler.AddReminder(medicineTb.Text, time, notesTb.Text);
                else
                    res = await RemindersHandler.ChangeReminder(reminder["id"].ToString(), medicineTb.Text, time, notesTb.Text);

                if ((int)res["StatusCode"] == 200)
                {
                    var popup = Variables.GetPopupNotifier("You have successfully added a reminder!", $"It will work at {time}",
                        Variables.BodyColor, Variables.FontColor);

                    popup.Popup();

                    Close();
                }
                else
                {
                    MessageBox.Show("Failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}