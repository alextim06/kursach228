using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace kursach
{
    public class EditCourseRequestForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private int requestId;
        private TextBox txtTitle;
        private NumericUpDown nudHours;
        private ComboBox cbFormat;
        private TextBox txtScheduleFile;
        private Button btnBrowse;
        private Button btnSave;
        private Button btnCancel;

        public EditCourseRequestForm(User user, int requestId)
        {
            currentUser = user;
            this.requestId = requestId;
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            LoadRequestData();
        }

        private void InitializeComponent()
        {
            Text = "Редактировать заявку на курс - ДПО Portal";
            Width = 500;
            Height = 450;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Main Panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(30);
            mainPanel.BackColor = Color.White;

            int yOffset = 20;
            int labelWidth = 120;
            int controlWidth = 300;
            int controlHeight = 30;
            int spacing = 15;

            // Title Label
            Label titleLabel = new Label();
            titleLabel.Text = "Редактирование заявки";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(0, 120, 215);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, yOffset);
            mainPanel.Controls.Add(titleLabel);
            yOffset += 50;

            // Название курса
            Label lblTitle = new Label();
            lblTitle.Text = "Название курса:*";
            lblTitle.Font = new Font("Segoe UI", 10);
            lblTitle.Location = new Point(20, yOffset);
            lblTitle.Size = new Size(labelWidth, controlHeight);
            mainPanel.Controls.Add(lblTitle);

            txtTitle = new TextBox();
            txtTitle.Size = new Size(controlWidth, controlHeight);
            txtTitle.Location = new Point(labelWidth + 30, yOffset);
            txtTitle.Font = new Font("Segoe UI", 10);
            mainPanel.Controls.Add(txtTitle);
            yOffset += controlHeight + spacing;

            // Количество часов
            Label lblHours = new Label();
            lblHours.Text = "Количество часов:*";
            lblHours.Font = new Font("Segoe UI", 10);
            lblHours.Location = new Point(20, yOffset);
            lblHours.Size = new Size(labelWidth, controlHeight);
            mainPanel.Controls.Add(lblHours);

            nudHours = new NumericUpDown();
            nudHours.Size = new Size(150, controlHeight);
            nudHours.Location = new Point(labelWidth + 30, yOffset);
            nudHours.Font = new Font("Segoe UI", 10);
            nudHours.Minimum = 16;
            nudHours.Maximum = 500;
            nudHours.Value = 72;
            mainPanel.Controls.Add(nudHours);
            yOffset += controlHeight + spacing;

            // Формат обучения
            Label lblFormat = new Label();
            lblFormat.Text = "Формат обучения:*";
            lblFormat.Font = new Font("Segoe UI", 10);
            lblFormat.Location = new Point(20, yOffset);
            lblFormat.Size = new Size(labelWidth, controlHeight);
            mainPanel.Controls.Add(lblFormat);

            cbFormat = new ComboBox();
            cbFormat.Size = new Size(controlWidth, controlHeight);
            cbFormat.Location = new Point(labelWidth + 30, yOffset);
            cbFormat.Font = new Font("Segoe UI", 10);
            cbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFormat.Items.AddRange(new string[] { "Очный", "Онлайн", "Смешанный" });
            cbFormat.SelectedIndex = 0;
            mainPanel.Controls.Add(cbFormat);
            yOffset += controlHeight + spacing;

            // Файл расписания
            Label lblSchedule = new Label();
            lblSchedule.Text = "Файл расписания:";
            lblSchedule.Font = new Font("Segoe UI", 10);
            lblSchedule.Location = new Point(20, yOffset);
            lblSchedule.Size = new Size(labelWidth, controlHeight);
            mainPanel.Controls.Add(lblSchedule);

            txtScheduleFile = new TextBox();
            txtScheduleFile.Size = new Size(controlWidth - 80, controlHeight);
            txtScheduleFile.Location = new Point(labelWidth + 30, yOffset);
            txtScheduleFile.Font = new Font("Segoe UI", 10);
            txtScheduleFile.ReadOnly = true;
            mainPanel.Controls.Add(txtScheduleFile);

            btnBrowse = new Button();
            btnBrowse.Text = "Обзор";
            btnBrowse.Size = new Size(70, controlHeight);
            btnBrowse.Location = new Point(labelWidth + controlWidth - 50, yOffset);
            btnBrowse.BackColor = Color.FromArgb(0, 120, 215);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Click += BtnBrowse_Click;
            mainPanel.Controls.Add(btnBrowse);
            yOffset += controlHeight + spacing;

            // Info Label
            Label infoLabel = new Label();
            infoLabel.Text = "* - обязательные поля";
            infoLabel.Font = new Font("Segoe UI", 8);
            infoLabel.ForeColor = Color.Gray;
            infoLabel.AutoSize = true;
            infoLabel.Location = new Point(20, yOffset);
            mainPanel.Controls.Add(infoLabel);
            yOffset += 40;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Сохранить изменения";
            btnSave.Size = new Size(150, 40);
            btnSave.Location = new Point(20, yOffset);
            btnSave.BackColor = Color.FromArgb(76, 175, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            mainPanel.Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(100, 40);
            btnCancel.Location = new Point(190, yOffset);
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();
            mainPanel.Controls.Add(btnCancel);

            Controls.Add(mainPanel);
        }

        private void LoadRequestData()
        {
            DataRow request = db.GetCourseRequestById(requestId);
            if (request == null)
            {
                MessageBox.Show("Заявка не найдена");
                this.Close();
                return;
            }

            txtTitle.Text = request["title"].ToString();
            nudHours.Value = Convert.ToInt32(request["hours"]);

            string format = request["format"].ToString();
            if (cbFormat.Items.Contains(format))
                cbFormat.SelectedItem = format;

            if (request["schedule_file"] != DBNull.Value)
            {
                txtScheduleFile.Text = request["schedule_file"].ToString();
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл расписания";
            openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtScheduleFile.Text = openFileDialog.FileName;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название курса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (nudHours.Value < 16)
            {
                MessageBox.Show("Минимальная продолжительность курса - 16 часов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string scheduleFileName = null;
            if (!string.IsNullOrEmpty(txtScheduleFile.Text))
            {
                scheduleFileName = System.IO.Path.GetFileName(txtScheduleFile.Text);
            }

            bool success = db.UpdateCourseRequest(
                requestId,
                txtTitle.Text.Trim(),
                (int)nudHours.Value,
                cbFormat.SelectedItem.ToString(),
                scheduleFileName
            );

            if (success)
            {
                MessageBox.Show(
                    "Заявка успешно обновлена!\n\n" +
                    "Ожидайте повторного рассмотрения администратором.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Ошибка при обновлении заявки. Попробуйте позже.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}