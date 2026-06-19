using System;
using System.Windows.Forms;

namespace Practice.Views
{
    public partial class LoginForm : Form
    {
        public static string UserRole { get; private set; } = "User";

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == "admin" && password == "admin")
            {
                UserRole = "Admin";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (username == "user" && password == "user123")
            {
                UserRole = "User";
                this.DialogResult = DialogResult.OK; 
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
