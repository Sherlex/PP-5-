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
    public partial class AuthorizationForm : Form
    {
        SQLiteConnection sqlconnection;
        SQLiteCommand command;
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void entry_Click(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            string passwordOk = "";
            command = new SQLiteCommand("SELECT * FROM [Admin]", sqlconnection);
            SQLiteDataAdapter passwordDataAdapter = new SQLiteDataAdapter(command);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                passwordOk = reader[1].ToString();
            }
                if (login.Text == "admin" && password.Text == passwordOk)
            {
                EditForm edit = new EditForm();
                edit.Show();
                Hide();
            }
            else
            {
                login.Text = "";
                password.Text = "";

                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            sqlconnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
