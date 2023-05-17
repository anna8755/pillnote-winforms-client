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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace pill_note_client.Views.EditForms
{
    public partial class AddLogForm : Form
    {
        bool isEditing = false;
        string reminderId;
        public AddLogForm(string reminderId, bool isEditing = false)
        {
            InitializeComponent();
            this.reminderId = reminderId;
            this.isEditing = isEditing;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isEditing)
                {
                    var addLogResult = await LogHandler.AddLog(reminderId, notesTb.Text);
                    if ((int)addLogResult["StatusCode"] == 200)
                    {
                        var popup = Variables.GetPopupNotifier("Well done!!!)", $"You have successfully added a log!",
                            Variables.BodyColor, Variables.FontColor);

                        popup.Popup();

                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}