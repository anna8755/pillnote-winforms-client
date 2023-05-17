using pill_note_client.Handlers;
using System;
using System.Windows.Forms;

namespace pill_note_client.Views
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            passTb.UseSystemPasswordChar = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var loginData = await ApiHandler.LoginAsync(emailTb.Text, passTb.Text);

                if (loginData["refreshToken"] == null)
                {
                    throw new Exception("Login error!");
                }
                MessageBox.Show($"Welcome, dear {loginData["user"]["fullname"]}");

                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label4_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                RegistrationForm form = new RegistrationForm();
                Hide();
                form.ShowDialog();
                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passTb.UseSystemPasswordChar = !((CheckBox)sender).Checked;
        }
    }
}