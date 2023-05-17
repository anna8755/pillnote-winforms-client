using Newtonsoft.Json.Linq;
using pill_note_client.Handlers;
using pill_note_client.Views.EditForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pill_note_client.Views
{
    public partial class HistoryForm : Form
    {
        List<JToken> RemindersData;
        public HistoryForm()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ForeColor = Color.Black;
            dataGridView1.Font = new Font("Modern No. 20", 18f, FontStyle.Bold);
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.ForeColor = Color.Black;
            dataGridView2.Font = new Font("Modern No. 20", 18f, FontStyle.Bold);
        }
        async Task LoadLogs()
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                    return;
                var reminder = RemindersData[dataGridView1.CurrentCell.RowIndex];
                var logs = await LogHandler.GetLogsByReminderId(reminder["id"].ToString());

                reminder["logs"] = (JArray)logs["array"];

                dataGridView2.DataSource = reminder["logs"].ToList().Select(s => new
                {
                    Notes = s["notes"],
                    Time = s["time"]
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            await LoadLogs();
        }
        private async Task LoadRemindersData()
        {
            var data = await RemindersHandler.GetReminders(2);
            if ((int)data["StatusCode"] != 200)
            {
                if (MessageBox.Show("It looks like you don't have any notes. Add new?", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Close();
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

        private async void HistoryForm_Load(object sender, EventArgs e)
        {
            await LoadRemindersData();
        }
        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var reminder = RemindersData[dataGridView1.CurrentCell.RowIndex];
                var logId = reminder["logs"][dataGridView2.CurrentCell.RowIndex]["id"].ToString();
                var res = await LogHandler.DeleteLogById(logId);

                await LoadLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                    return;
                var reminderId = RemindersData[dataGridView1.CurrentCell.RowIndex]["id"].ToString();

                var form = new AddLogForm(reminderId);
                Hide();
                form.ShowDialog();
                Show();

                await LoadLogs();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}