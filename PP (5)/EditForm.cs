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
    public partial class EditForm : Form
    {
        SQLiteConnection sqlconnection;
        SQLiteCommand command;
        string selected = "";
        string[] arr = new string[8];

        public EditForm()
        {
            InitializeComponent();
        }

        private void ChangePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPasswordForm newPassword = new NewPasswordForm();
            newPassword.Show();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            try
            {
                sqlconnection = new SQLiteConnection(Connection.connectionString);
                sqlconnection.Open();
                arr[0] = ro_edittextBox.Text;
                arr[1] = c_edittextBox.Text;
                arr[2] = T0_edittextBox.Text;
                arr[3] = mu0_edittextBox.Text;
                arr[4] = b_edittextBox.Text;
                arr[5] = Tr_edittextBox.Text;
                arr[6] = n_edittextBox.Text;
                arr[7] = au_edittextBox.Text;
                selected = material_editcomboBox.Text;
                int id = 0;
                command = new SQLiteCommand("SELECT id_type_material FROM [Materials] WHERE (((Materials.type_material)=@par))", sqlconnection);
                command.Parameters.AddWithValue("par", selected);
                SQLiteDataAdapter materialsDataAdapter = new SQLiteDataAdapter(command);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    id = Convert.ToInt32(reader[0].ToString());
                }
                for (int i = 0; i < 8; i++)
                {
                    command = new SQLiteCommand("UPDATE Parameters_value SET parameter_value = @par WHERE (((Parameters_value.[id_parameter]) = @count) AND((Parameters_value.[id_type_material]) = @idtype))", sqlconnection);
                    command.Parameters.AddWithValue("par", arr[i]);
                    command.Parameters.AddWithValue("count", i + 1);
                    command.Parameters.AddWithValue("idtype", id);
                    command.ExecuteNonQuery();
                }
                sqlconnection.Close();
                MessageBox.Show("Данные успешно обновлены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("При обновлении данных произошла ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
      
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            command = new SQLiteCommand();
            command.Connection = sqlconnection;

            DataTable Materials = new DataTable();
            command = new SQLiteCommand("SELECT * FROM [Materials]", sqlconnection);
            SQLiteDataAdapter materialDataAdapter = new SQLiteDataAdapter(command);
            materialDataAdapter.Fill(Materials);
            material_editcomboBox.DataSource = Materials;
            material_editcomboBox.DisplayMember = "type_material";
            sqlconnection.Close();
        }

        private void material_editcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            selected = material_editcomboBox.Text;
            command = new SQLiteCommand("SELECT Parameters_value.parameter_value FROM Materials INNER JOIN([Parameters] INNER JOIN Parameters_value ON Parameters.id_parameter = Parameters_value.id_parameter) ON Materials.id_type_material = Parameters_value.id_type_material WHERE (((Materials.type_material)=@par));", sqlconnection);
            command.Parameters.AddWithValue("par", selected);

            
            SQLiteDataAdapter parametersDataAdapter = new SQLiteDataAdapter(command);
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read() && i < 8)
            {
                arr[i] = reader[0].ToString();
                i++;
            }
            ro_edittextBox.Text = arr[0];
            c_edittextBox.Text = arr[1];
            T0_edittextBox.Text = arr[2];
            mu0_edittextBox.Text = arr[3];
            b_edittextBox.Text = arr[4];
            Tr_edittextBox.Text = arr[5];
            n_edittextBox.Text = arr[6];
            au_edittextBox.Text = arr[7];
            sqlconnection.Close();
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            try
            {
                sqlconnection = new SQLiteConnection(Connection.connectionString);
                sqlconnection.Open();
                int id = 0;
                command = new SQLiteCommand("SELECT id_type_material FROM [Materials] WHERE (((Materials.type_material)=@par))", sqlconnection);
                command.Parameters.AddWithValue("par", selected);
                SQLiteDataAdapter materialsDataAdapter = new SQLiteDataAdapter(command);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader[0].ToString());
                }
                for (int i = 0; i < 8; i++)
                {
                    command = new SQLiteCommand("DELETE FROM Parameters_value WHERE (((Parameters_value.[id_parameter]) = @count) AND((Parameters_value.[id_type_material]) = @idtype))", sqlconnection);
                    command.Parameters.AddWithValue("count", i + 1);
                    command.Parameters.AddWithValue("idtype", id);
                    command.ExecuteNonQuery();
                }
                command = new SQLiteCommand("DELETE FROM Materials WHERE ((Materials.id_type_material) = @idtype)", sqlconnection);
                command.Parameters.AddWithValue("idtype", id);
                command.ExecuteNonQuery();
                command = new SQLiteCommand();
                command.Connection = sqlconnection;

                DataTable Materials = new DataTable();
                command = new SQLiteCommand("SELECT * FROM [Materials]", sqlconnection);
                SQLiteDataAdapter materialDataAdapter = new SQLiteDataAdapter(command);
                materialDataAdapter.Fill(Materials);
                material_editcomboBox.DataSource = Materials;
                material_editcomboBox.DisplayMember = "type_material";
                sqlconnection.Close();
                MessageBox.Show("Данные успешно удалены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("При удалении данных произошла ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            try
            {
                sqlconnection = new SQLiteConnection(Connection.connectionString);
                sqlconnection.Open();
                string[] addarr = new string[9];
                addarr[0] = ro_addtextBox.Text;
                addarr[1] = c_addtextBox.Text;
                addarr[2] = T0_addtextBox.Text;
                addarr[3] = mu0_addtextBox.Text;
                addarr[4] = b_addtextBox.Text;
                addarr[5] = Tr_addtextBox.Text;
                addarr[6] = n_addtextBox.Text;
                addarr[7] = au_addtextBox.Text;
                addarr[8] = material_addtextBox.Text;
                int id = 0;
                string new_material;
                command = new SQLiteCommand("INSERT INTO Materials (type_material) VALUES (@typematerial)", sqlconnection);
                command.Parameters.AddWithValue("typematerial", addarr[8]);
                command.ExecuteNonQuery();
                new_material = material_addtextBox.Text;

                command = new SQLiteCommand("SELECT id_type_material FROM [Materials] WHERE (((Materials.type_material)=@par))", sqlconnection);
                command.Parameters.AddWithValue("par", new_material);
                SQLiteDataAdapter materialsDataAdapter = new SQLiteDataAdapter(command);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader[0].ToString());
                }
                for (int i = 0; i < 8; i++)
                {
                    command = new SQLiteCommand("INSERT INTO [Parameters_value] (parameter_value, id_type_material, id_parameter)VALUES(@par, @idtype, @count)", sqlconnection);
                    command.Parameters.AddWithValue("par", addarr[i]);
                    command.Parameters.AddWithValue("count", i + 1);
                    command.Parameters.AddWithValue("idtype", id);
                    command.ExecuteNonQuery();
                }
                ro_addtextBox.Clear();
                c_addtextBox.Clear();
                T0_addtextBox.Clear();
                mu0_addtextBox.Clear();
                b_addtextBox.Clear();
                Tr_addtextBox.Clear();
                n_addtextBox.Clear();
                au_addtextBox.Clear();
                material_addtextBox.Clear();

                command = new SQLiteCommand();
                command.Connection = sqlconnection;

                DataTable Materials = new DataTable();
                command = new SQLiteCommand("SELECT * FROM [Materials]", sqlconnection);
                SQLiteDataAdapter materialDataAdapter = new SQLiteDataAdapter(command);
                materialDataAdapter.Fill(Materials);
                material_editcomboBox.DataSource = Materials;
                material_editcomboBox.DisplayMember = "type_material";
                sqlconnection.Close();
                MessageBox.Show("Данные успешно добавлены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("При добавлении данных произошла ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Any_addtextBox_TextChanged(object sender, EventArgs e)
        {
            if ((ro_addtextBox.Text != "") && (c_addtextBox.Text != "") && (T0_addtextBox.Text != "") &&
                (mu0_addtextBox.Text != "") && (b_addtextBox.Text != "") && (Tr_addtextBox.Text != "") && (n_addtextBox.Text != "") && 
                (au_addtextBox.Text != "") && (ro_addtextBox.Text != ",") && (c_addtextBox.Text != ",") && (T0_addtextBox.Text != ",") &&
                (mu0_addtextBox.Text != ",") && (b_addtextBox.Text != ",") && (Tr_addtextBox.Text != ",") && (n_addtextBox.Text != ",") &&
                (au_addtextBox.Text != ",") && (ro_addtextBox.Text != "0") && (c_addtextBox.Text != "0") && (T0_addtextBox.Text != "0") &&
                (mu0_addtextBox.Text != "0") && (b_addtextBox.Text != "0") && (Tr_addtextBox.Text != "0") && (n_addtextBox.Text != "0") &&
                (au_addtextBox.Text != "0") && (material_addtextBox.Text != ""))
                add_button.Enabled = true;
            else add_button.Enabled = false;
        }

        private void Any_edittextBox_TextChanged(object sender, EventArgs e)
        {
            if ((ro_edittextBox.Text != "") && (c_edittextBox.Text != "") && (T0_edittextBox.Text != "") &&
                (mu0_edittextBox.Text != "") && (b_edittextBox.Text != "") && (Tr_edittextBox.Text != "") && (n_edittextBox.Text != "") &&
                (au_edittextBox.Text != "") && (ro_edittextBox.Text != ",") && (c_edittextBox.Text != ",") && (T0_edittextBox.Text != ",") &&
                (mu0_edittextBox.Text != ",") && (b_edittextBox.Text != ",") && (Tr_edittextBox.Text != ",") && (n_edittextBox.Text != ",") &&
                (au_edittextBox.Text != ",") && (ro_edittextBox.Text != "0") && (c_edittextBox.Text != "0") && (T0_edittextBox.Text != "0") &&
                (mu0_edittextBox.Text != "0") && (b_edittextBox.Text != "0") && (Tr_edittextBox.Text != "0") && (n_edittextBox.Text != "0") &&
                (au_edittextBox.Text != "0"))
                save_button.Enabled = true;
            else
                save_button.Enabled = false;
        }

        private void Any_edittextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
            {
                return;
            }

            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }
            if (e.KeyChar == '.')
            {
                if ((sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                return;
            }

            if (Char.IsControl(e.KeyChar))
            {
                return;
            }
            e.Handled = true;
        }

        private void Any_addtextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
            {
                return;
            }

            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }
            if (e.KeyChar == '.')
            {
                if ((sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                return;
            }

            if (Char.IsControl(e.KeyChar))
            {
                return;
            }
            e.Handled = true;
        }
    }
}
