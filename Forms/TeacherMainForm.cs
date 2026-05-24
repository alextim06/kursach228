using System;
using System.Drawing;
using System.Windows.Forms;

namespace kursach
{
    public class TeacherMainForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private Panel headerPanel;
        private Label welcomeLabel;
        private Panel buttonsPanel;

        public TeacherMainForm(User user)
        {
            currentUser = user;
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            this.Shown += TeacherMainForm_Shown; // Добавляем событие после загрузки формы
        }

        private void InitializeComponent()
        {
            Text = "Преподаватель - ДПО Portal";
            Width = 900;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 245, 245);
            MinimumSize = new Size(800, 500);

            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20, 10, 20, 10);

            // Welcome Label
            welcomeLabel = new Label();
            welcomeLabel.Text = $"Здравствуйте, {currentUser.full_name}!";
            welcomeLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.AutoSize = true;
            welcomeLabel.Location = new Point(20, 35);
            headerPanel.Controls.Add(welcomeLabel);

            // Buttons Panel
            buttonsPanel = new Panel();
            buttonsPanel.Dock = DockStyle.Fill;
            buttonsPanel.BackColor = Color.White;

            Controls.Add(buttonsPanel);
            Controls.Add(headerPanel);
        }

        private void TeacherMainForm_Shown(object sender, EventArgs e)
        {
            CreateMenuButtons();
        }

        private void CreateMenuButtons()
        {
            // Очищаем панель перед добавлением
            buttonsPanel.Controls.Clear();

            int buttonWidth = 210;
            int buttonHeight = 100;
            int spacing = 30;

            // Вычисляем центр панели
            int centerX = buttonsPanel.ClientSize.Width / 2;
            int centerY = buttonsPanel.ClientSize.Height / 2;

            // Вычисляем начальную позицию для 2x2 сетки
            int startX = centerX - buttonWidth - spacing / 2;
            int startY = centerY - buttonHeight - spacing / 2;

            // Button 1: Расписание занятий (верхний левый)
            Button scheduleBtn = CreateMenuButton(
                "📅 Расписание занятий",
                "Просмотр расписания",
                buttonWidth, buttonHeight
            );
            scheduleBtn.Location = new Point(startX, startY);
            scheduleBtn.Click += (s, e) => OpenScheduleWindow();

            // Button 2: Журнал посещаемости (верхний правый)
            Button journalBtn = CreateMenuButton(
                "📝 Журнал посещаемости",
                "Ведение журнала",
                buttonWidth, buttonHeight
            );
            journalBtn.Location = new Point(startX + buttonWidth + spacing, startY);
            journalBtn.Click += (s, e) => OpenJournalWindow();

            // Button 3: Мои заявки (нижний левый)
            Button courseRequestsBtn = CreateMenuButton(
                "📋 Мои заявки",
                "Просмотр и редактирование",
                buttonWidth, buttonHeight
            );
            courseRequestsBtn.Location = new Point(startX, startY + buttonHeight + spacing);
            courseRequestsBtn.Click += (s, e) => OpenCourseRequestsWindow();

            // Button 4: Добавить курс (нижний правый)
            Button addCourseBtn = CreateMenuButton(
                "➕ Добавить курс",
                "Создать новую заявку",
                buttonWidth, buttonHeight
            );
            addCourseBtn.Location = new Point(startX + buttonWidth + spacing, startY + buttonHeight + spacing);
            addCourseBtn.Click += (s, e) => OpenAddCourseWindow();

            buttonsPanel.Controls.Add(scheduleBtn);
            buttonsPanel.Controls.Add(journalBtn);
            buttonsPanel.Controls.Add(courseRequestsBtn);
            buttonsPanel.Controls.Add(addCourseBtn);
        }

        private Button CreateMenuButton(string title, string subtitle, int width, int height)
        {
            Button btn = new Button();
            btn.Size = new Size(width, height);
            btn.BackColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btn.Cursor = Cursors.Hand;

            // Создаем метки внутри кнопки
            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(0, 120, 215);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(15, 25);
            titleLabel.BackColor = Color.Transparent;

            Label subtitleLabel = new Label();
            subtitleLabel.Text = subtitle;
            subtitleLabel.Font = new Font("Segoe UI", 9);
            subtitleLabel.ForeColor = Color.Gray;
            subtitleLabel.AutoSize = true;
            subtitleLabel.Location = new Point(15, 55);
            subtitleLabel.BackColor = Color.Transparent;

            btn.Controls.Add(titleLabel);
            btn.Controls.Add(subtitleLabel);

            return btn;
        }

        private void OpenScheduleWindow()
        {
            TeacherScheduleForm scheduleForm = new TeacherScheduleForm(currentUser);
            scheduleForm.ShowDialog();
        }

        private void OpenJournalWindow()
        {
            TeacherJournalForm journalForm = new TeacherJournalForm(currentUser);
            journalForm.ShowDialog();
        }

        private void OpenCourseRequestsWindow()
        {
            TeacherCourseRequestsForm requestsForm = new TeacherCourseRequestsForm(currentUser);
            requestsForm.ShowDialog();
        }

        private void OpenAddCourseWindow()
        {
            AddCourseRequestForm addCourseForm = new AddCourseRequestForm(currentUser);
            addCourseForm.ShowDialog();
        }
    }
}