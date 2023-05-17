using Microsoft.VisualBasic;
using pill_note_client.Handlers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace pill_note_client.Views
{
    public partial class RegistrationForm : Form
    {


        private Color faultColor = Color.FromArgb(255, 128, 128);
        private Color successColor = Color.FromArgb(176, 194, 191);
        private string photoLink = "";

        public RegistrationForm()
        {
            InitializeComponent();
        }

        #region animation
        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.Font = new Font(label1.Font, FontStyle.Underline | FontStyle.Bold);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.Font = new Font(label1.Font, FontStyle.Bold);

        }
        #endregion

        private void label4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ValidateData()
        {
            textBox1.BackColor = successColor;
            textBox2.BackColor = successColor;
            passTb.BackColor = successColor;
            confPassTb.BackColor = successColor;

            bool isValid = true;

            var message = "";

            if (passTb.Text != confPassTb.Text ||
                !DataValidator.ValidatePassword(passTb.Text) || !DataValidator.ValidatePassword(confPassTb.Text))
            {
                isValid = false;
                passTb.BackColor = faultColor;
                confPassTb.BackColor = faultColor;

                message += "Перевірте правильність введених паролів!\n";
            }
            if (!DataValidator.ValidateEmail(textBox1.Text))
            {
                isValid = false;
                textBox1.BackColor = faultColor;

                message += "Перевірте правильність введеної пошти!\n";
            }
            if (textBox2.Text.Length < 5)
            {
                isValid = false;
                textBox2.BackColor = faultColor;

                message += "Ви не повністю ввели своє ім'я!\n";
            }

            if (!isValid) throw new Exception("Перевірте введені дані:\n\n" + message);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateData();

                var regResponce = await ApiHandler.RegistrateAsync(textBox2.Text, textBox1.Text, passTb.Text, photoLink);
                if (ApiHandler.IsValidUserApiResponce(regResponce))
                {
                    MessageBox.Show("Check the email you provided during registration, " +
                        "if you do not activate the account, the service will not be available to you.");
                    Close();
                }
                else
                {
                    MessageBox.Show(regResponce.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                photoLink = Interaction.InputBox("Enter a link to a photo in this field:");
                if (string.IsNullOrEmpty(photoLink))
                    return;
                userPb.Load(photoLink);
            }
            catch (Exception ex)
            {
                photoLink = "";
                userPb.Image = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                photoLink = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passTb.UseSystemPasswordChar = !((CheckBox)sender).Checked;
            confPassTb.UseSystemPasswordChar = !((CheckBox)sender).Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var strongPass = DataValidator.GeneratePassword();
            passTb.Text = strongPass;
            confPassTb.Text = strongPass;
            checkBox1.Checked = true;
        }
    }
}