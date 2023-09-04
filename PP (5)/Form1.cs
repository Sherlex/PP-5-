using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;

namespace PP__5_
{
    public partial class Form1 : Form
    {
        SQLiteConnection sqlconnection;
        SQLiteCommand command;
        string selected = "";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            command = new SQLiteCommand();
            command.Connection = sqlconnection;

            DataTable Materials = new DataTable();
            command = new SQLiteCommand("SELECT * FROM [Materials]", sqlconnection);
            SQLiteDataAdapter materialDataAdapter = new SQLiteDataAdapter(command);
            materialDataAdapter.Fill(Materials);
            material_comboBox.DataSource = Materials;
            material_comboBox.DisplayMember = "type_material";
            sqlconnection.Close();

        }

        private void material_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected = material_comboBox.Text;
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            command = new SQLiteCommand("SELECT Parameters_value.parameter_value FROM Materials INNER JOIN([Parameters] INNER JOIN Parameters_value ON Parameters.id_parameter = Parameters_value.id_parameter) ON Materials.id_type_material = Parameters_value.id_type_material WHERE (((Materials.type_material)=@par));", sqlconnection);
            command.Parameters.AddWithValue("par", selected);
           
            string[] arr = new string[8];
            SQLiteDataAdapter parametersDataAdapter = new SQLiteDataAdapter(command);
            SQLiteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read()&& i < 8)
            {
                arr[i] = reader[0].ToString();
                i++;
            }
            ro_textBox.Text = arr[0];
            c_textBox.Text = arr[1];
            T0_textBox.Text = arr[2];
            mu0_textBox.Text = arr[3];
            b_textBox.Text = arr[4];
            Tr_textBox.Text = arr[5];
            n_textBox.Text = arr[6];
            au_textBox.Text = arr[7];
            sqlconnection.Close();
        }

        int clicked = -1;

        private void calc_button_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            temperature_chart.Series[0].Points.Clear();

            viscosity_chart.Series[0].Points.Clear();

            temperature_chart.ChartAreas[0].AxisX.Title = "Координата по длине канала, м";
            temperature_chart.ChartAreas[0].AxisY.Title = "Температура материала, °С";
            viscosity_chart.ChartAreas[0].AxisX.Title = "Координата по длине канала, м";
            viscosity_chart.ChartAreas[0].AxisY.Title = "Вязкость материала, Па•с";

            double w, h, l, p, c, To, Vu, Tu, uo, b, Tr, n, au, step, N, temp1, temp2;
            double Product_temp = 0;
            double Viscosity = 0;

            w = Convert.ToDouble(W_textBox.Text);
            h = Convert.ToDouble(H_textBox.Text);
            l = Convert.ToDouble(L_textBox.Text);
            p = Convert.ToDouble(ro_textBox.Text);
            c = Convert.ToDouble(c_textBox.Text);
            To = Convert.ToDouble(T0_textBox.Text);
            Vu = Convert.ToDouble(Vu_textBox.Text);
            Tu = Convert.ToDouble(Tu_textBox.Text);
            uo = Convert.ToDouble(mu0_textBox.Text);
            b = Convert.ToDouble(b_textBox.Text);
            Tr = Convert.ToDouble(Tr_textBox.Text);
            n = Convert.ToDouble(n_textBox.Text);
            au = Convert.ToDouble(au_textBox.Text);
            step = Convert.ToDouble(step_textBox.Text);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            double F = 0.125 * ((h / w) * (h / w)) - 0.625 * (h / w) + 1;
            double QCH = ((h * w * Vu) / 2) * F;
            double y = Vu / h;
            double qy = h * w * uo * System.Math.Pow(y, (n + 1));
            double qa = w * au * ((1 / b) - Tu + Tr);
            int count = 0;
            int min_product = 100000;
            int min_visc = 10000;
            double temp = 0;
            N = l / step;

            for (double i = 0; i <= l; i = i + step)
            {
                temp1 = ((b * qy + w * au) / (b * qa)) * (1 - Math.Exp(-(i * b * qa) / 
                    (p * c * QCH))) + (Math.Exp(b * (To - Tr - (i * qa) / (p * c * QCH))));
                Product_temp = Tr + (1 / b) * (Math.Log(temp1));
                temp2 = uo * Math.Exp(-b * (Product_temp - Tr));
                Viscosity = temp2 * System.Math.Pow(y, (n - 1));

                Product_temp = Math.Round(Product_temp, 2);
                Viscosity = Math.Round(Viscosity, 2);
                if (Product_temp < min_product)
                    min_product = Convert.ToInt32(Product_temp);
                if (Viscosity < min_visc)
                    min_visc = Convert.ToInt32(Viscosity);
                temperature_chart.ChartAreas[0].AxisX.Minimum = 0;
                temperature_chart.ChartAreas[0].AxisX.Maximum = l;
                temperature_chart.ChartAreas[0].AxisY.Minimum = min_product;


                viscosity_chart.ChartAreas[0].AxisX.Minimum = 0;
                viscosity_chart.ChartAreas[0].AxisY.Minimum = min_visc;
                viscosity_chart.ChartAreas[0].AxisX.Maximum = l;


                temperature_chart.Series[0].Points.AddXY(i, Product_temp);
                viscosity_chart.Series[0].Points.AddXY(i, Viscosity);

                dataGridView1.Rows.Add();
                temp = Math.Round(i, 4);
                dataGridView1.Rows[count].Cells[0].Value = (temp).ToString();
                dataGridView1.Rows[count].Cells[1].Value = (Product_temp).ToString();
                dataGridView1.Rows[count].Cells[2].Value = (Viscosity).ToString();

                count++;
                

            }
            watch.Stop();
            label23.Text = "Время выполнения: " + (watch.ElapsedMilliseconds / 1000.0).ToString() + " с";
            double Productivity = Math.Round((p * QCH*3600), 1);
            product_temp_textBox.Text = Product_temp.ToString();
            product_visc_textBox.Text = Viscosity.ToString();
            productivity_textBox.Text = Productivity.ToString();
            clicked = 1;
           
        }

        private void AdminEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorizationForm auth = new AuthorizationForm();
            auth.Show();
        }

        private void Any_main_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
            {
                return;
            }

            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
            if (e.KeyChar == ',')
            {
                if ((sender as TextBox).Text.IndexOf(',') > -1)
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

        private void Any_main_textBox_TextChanged(object sender, EventArgs e)
        {
            if ((W_textBox.Text != "") && (H_textBox.Text != "") && (L_textBox.Text != "") &&
                      (ro_textBox.Text != "") && (c_textBox.Text != "") && (T0_textBox.Text != "") &&
                      (Vu_textBox.Text != "") && (Tu_textBox.Text != "") && (mu0_textBox.Text != "") &&
                      (b_textBox.Text != "") && (Tr_textBox.Text != "") && (n_textBox.Text != "") && (au_textBox.Text != "") && (step_textBox.Text != "") &&
                      (W_textBox.Text != ",") && (H_textBox.Text != ",") && (L_textBox.Text != ",") &&
                      (ro_textBox.Text != ",") && (c_textBox.Text != ",") && (T0_textBox.Text != ",") &&
                      (Vu_textBox.Text != ",") && (Tu_textBox.Text != ",") && (mu0_textBox.Text != ",") &&
                      (b_textBox.Text != ",") && (Tr_textBox.Text != ",") && (n_textBox.Text != ",") && (au_textBox.Text != ",") && (step_textBox.Text != ",")
                      && (W_textBox.Text != "0") && (H_textBox.Text != "0") && (L_textBox.Text != "0") &&
                      (ro_textBox.Text != "0") && (c_textBox.Text != "0") && (T0_textBox.Text != "0") &&
                      (Vu_textBox.Text != "0") && (Tu_textBox.Text != "0") && (mu0_textBox.Text != "0") &&
                      (b_textBox.Text != "0") && (Tr_textBox.Text != "0") && (n_textBox.Text != "0") && (au_textBox.Text != "0") && (step_textBox.Text != "0"))
                calc_button.Enabled = true;
            else calc_button.Enabled = false;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (clicked == -1) e.Cancel = e.TabPageIndex == 1;
            else
            {
                e.Cancel = e.TabPageIndex == 2;
                clicked = -1;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection(Connection.connectionString);
            sqlconnection.Open();
            command = new SQLiteCommand();
            command.Connection = sqlconnection;

            DataTable Materials = new DataTable();
            command = new SQLiteCommand("SELECT * FROM [Materials]", sqlconnection);
            SQLiteDataAdapter materialDataAdapter = new SQLiteDataAdapter(command);
            materialDataAdapter.Fill(Materials);
            material_comboBox.DataSource = Materials;
            material_comboBox.DisplayMember = "type_material";
            sqlconnection.Close();
        }

        private void generate_reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "xlsx files (*.xlsx)|*.xlsx|All files(*.*)|*.*";
            string file_name = string.Empty;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            Excel.Application excelapp = new Excel.Application();
            Excel.Workbook workbook = excelapp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            int flag = 0;
            if (save.ShowDialog() == DialogResult.OK)
            {
                file_name = save.FileName;
                try
                {
                    label25.Visible = true;
                    progressBar1.Visible = true;
                    flag = 1;
                    worksheet.Cells[1, 2] = "Входные параметры";
                    worksheet.Cells[2, 1] = "Тип материала";
                    worksheet.Cells[2, 6] = material_comboBox.Text;
                    worksheet.Cells[4, 1] = "Геометрические параметры канала:";
                    worksheet.Cells[5, 1] = "Ширина, м";
                    worksheet.Cells[5, 6] = W_textBox.Text;
                    worksheet.Cells[6, 1] = "Глубина, м";
                    worksheet.Cells[6, 6] = H_textBox.Text;
                    worksheet.Cells[7, 1] = "Длина, м";
                    worksheet.Cells[7, 6] = L_textBox.Text;
                    worksheet.Cells[9, 1] = "Параметры свойств материала:";
                    worksheet.Cells[10, 1] = "Плотность, кг/м^3";
                    worksheet.Cells[11, 6] = ro_textBox.Text;
                    worksheet.Cells[12, 1] = "Удельная теплоёмкость, Дж/(кг•°С)";
                    worksheet.Cells[12, 6] = c_textBox.Text;
                    worksheet.Cells[13, 1] = "Температура плавления, °С";
                    worksheet.Cells[13, 6] = T0_textBox.Text;
                    worksheet.Cells[15, 1] = "Режимные параметры процесса:";
                    worksheet.Cells[16, 1] = "Скорость крышки, м/с";
                    worksheet.Cells[16, 6] = Vu_textBox.Text;
                    worksheet.Cells[17, 1] = "Температура крышки, °С";
                    worksheet.Cells[17, 6] = Tu_textBox.Text;
                    worksheet.Cells[19, 1] = "Эмпирические коэффициенты математической модели:";
                    worksheet.Cells[20, 1] = "Коэффициент консистенции материала при температуре приведения, Па*с^n";
                    worksheet.Cells[20, 6] = mu0_textBox.Text;
                    worksheet.Cells[21, 1] = "Температурный коэффициент вязкости, 1/°С";
                    worksheet.Cells[21, 6] = b_textBox.Text;
                    worksheet.Cells[22, 1] = "Температура приведения, °С";
                    worksheet.Cells[22, 6] = Tr_textBox.Text;
                    worksheet.Cells[23, 1] = "Индекс течения материала";
                    worksheet.Cells[23, 6] = n_textBox.Text;
                    worksheet.Cells[24, 1] = "Коэффициент теплоотдачи от крышки канала к материалу,Вт / (м ^ 2 *°С)";
                    worksheet.Cells[24, 6] = au_textBox.Text;
                    worksheet.Cells[26, 1] = "Параметры метода решения уравнений модели:";
                    worksheet.Cells[27, 1] = "Шаг расчета по длине канала, м";
                    worksheet.Cells[27, 6] = step_textBox.Text;
                    worksheet.Cells[29, 1] = "Критериальные показатели процесса:";
                    worksheet.Cells[30, 1] = "Производительность, кг/ч";
                    worksheet.Cells[30, 6] = productivity_textBox.Text;
                    worksheet.Cells[31, 1] = "Температура продукта, °С";
                    worksheet.Cells[32, 6] = product_temp_textBox.Text;
                    worksheet.Cells[33, 1] = "Вязкость продукта, Па•с";
                    worksheet.Cells[33, 6] = product_visc_textBox.Text;

                    worksheet.Cells[1, 9] = "Длина канала, м";
                    worksheet.Cells[1, 10] = "Температура, °С";
                    worksheet.Cells[1, 11] = "Вязкость, Па•с";
                    
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                       
                        for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        {
                            worksheet.Cells[i + 2, j + 9] = dataGridView1.Rows[i].Cells[j].Value;
                           
                       
                        }
                        if (flag == 1 && progressBar1.Value < 100)
                        {
                            progressBar1.Value++;
                        }
                    }

                    flag = 0;
                    excelapp.AlertBeforeOverwriting = false;
                    workbook.SaveAs(file_name, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    save.Dispose();
                    excelapp.Quit();
                    MessageBox.Show(text: "Данные сохранены успешно!", caption: "Информация",
                        buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                    label25.Visible = false;
                    progressBar1.Visible = false;
                }
                catch
                {
                    MessageBox.Show(text: "При сохранении данных произошла ошибка!", caption: "Ошибка!",
                        buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    label25.Visible = false;
                    progressBar1.Visible = false;
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var source = new SQLiteConnection(Connection.connectionString))
                using (var destination = new SQLiteConnection("Data Source=BackupDb.db; Version=3;"))
                {
                    source.Open();
                    destination.Open();
                    source.BackupDatabase(destination, "main", "main", -1, null, 0);
                    MessageBox.Show(text: "Резервное копирование выполнено успешно!", caption: "Информация",
                            buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show(text: "При резервном копировании данных произошла ошибка!", caption: "Ошибка!",
                        buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }
    }
}
