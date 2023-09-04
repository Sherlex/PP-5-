using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PP__5_
{
    public partial class NewPasswordForm : Form
    {
        SQLiteConnection sqlconnection;
        SQLiteCommand command;
        public NewPasswordForm()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string verpassword = "";
            if (password.Text != verifyPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Text = "";
                verifyPassword.Text = "";
            }
            if (((password.Text).Length >= 20) || ((verifyPassword.Text).Length >= 20))
            {
                MessageBox.Show("Пароль не может быть длинее 20 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                password.Text = "";
                verifyPassword.Text = "";
            }
            else
            {
                try
                {
                    verpassword = verifyPassword.Text;
                    sqlconnection = new SQLiteConnection(Connection.connectionString);
                    sqlconnection.Open();
                    command = new SQLiteCommand("UPDATE [Admin] SET [Password] = @par WHERE Id = 1", sqlconnection);
                    command.Parameters.AddWithValue("par", verpassword);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Пароль успешно изменен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sqlconnection.Close();
                    Hide();
                }
                catch
                {
                MessageBox.Show("Пароль некорректен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
