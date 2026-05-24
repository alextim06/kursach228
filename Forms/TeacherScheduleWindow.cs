using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace kursach
{
    public class TeacherScheduleForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private WebBrowser webBrowser;
        private Button btnOpenExcel;
        private Button btnRefresh;
        private Label lblInfo;
        private string scheduleFilePath;

        public TeacherScheduleForm(User user)
        {
            currentUser = user;
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            LoadSchedule();
        }

        private void InitializeComponent()
        {
            Text = "Расписание занятий - ДПО Portal";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20);

            Label titleLabel = new Label();
            titleLabel.Text = "Расписание занятий";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 25);
            headerPanel.Controls.Add(titleLabel);

            // Info Panel
            Panel infoPanel = new Panel();
            infoPanel.Dock = DockStyle.Top;
            infoPanel.Height = 40;
            infoPanel.BackColor = Color.FromArgb(245, 245, 245);
            infoPanel.Padding = new Padding(20);

            lblInfo = new Label();
            lblInfo.Text = "Расписание загружено. Для просмотра используйте кнопки ниже.";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.Gray;
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(20, 10);
            infoPanel.Controls.Add(lblInfo);

            // Button Panel
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.White;
            buttonPanel.Padding = new Padding(20);

            btnOpenExcel = new Button();
            btnOpenExcel.Text = "📊 Открыть в Excel";
            btnOpenExcel.Size = new Size(150, 35);
            btnOpenExcel.Location = new Point(20, 12);
            btnOpenExcel.BackColor = Color.FromArgb(33, 150, 243);
            btnOpenExcel.ForeColor = Color.White;
            btnOpenExcel.FlatStyle = FlatStyle.Flat;
            btnOpenExcel.Cursor = Cursors.Hand;
            btnOpenExcel.Click += BtnOpenExcel_Click;
            buttonPanel.Controls.Add(btnOpenExcel);

            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Обновить";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(190, 12);
            btnRefresh.BackColor = Color.FromArgb(0, 120, 215);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadSchedule();
            buttonPanel.Controls.Add(btnRefresh);

            Button btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Size = new Size(100, 35);
            btnClose.Location = new Point(780, 12);
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(btnClose);

            // WebBrowser for viewing Excel
            webBrowser = new WebBrowser();
            webBrowser.Dock = DockStyle.Fill;

            Controls.Add(webBrowser);
            Controls.Add(buttonPanel);
            Controls.Add(infoPanel);
            Controls.Add(headerPanel);
        }

        private void LoadSchedule()
        {
            try
            {
                // Получаем преподавателя
                var teacher = db.GetTeacherByUserId(currentUser.user_id);
                if (teacher == null)
                {
                    MessageBox.Show("Преподаватель не найден");
                    return;
                }

                // Генерируем Excel файл с расписанием
                scheduleFilePath = db.GenerateTeacherSchedule(teacher.teacher_id);

                if (!string.IsNullOrEmpty(scheduleFilePath) && File.Exists(scheduleFilePath))
                {
                    // Отображаем в WebBrowser
                    webBrowser.Navigate(scheduleFilePath);
                    lblInfo.Text = "Расписание успешно загружено. Для скачивания нажмите 'Открыть в Excel'.";
                }
                else
                {
                    lblInfo.Text = "Расписание не найдено. Обратитесь к администратору.";
                    webBrowser.DocumentText = "<html><body><h2>Расписание отсутствует</h2><p>Нет данных о расписании занятий.</p></body></html>";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}");
            }
        }

        private void BtnOpenExcel_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(scheduleFilePath) && File.Exists(scheduleFilePath))
            {
                try
                {
                    Process.Start(scheduleFilePath);
                }
                catch
                {
                    MessageBox.Show("Не удалось открыть файл. Убедитесь, что установлен Excel.");
                }
            }
            else
            {
                MessageBox.Show("Файл расписания не найден.");
            }
        }
    }
}