using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kursach
{
    public class StudentMainForm : Form
    {
        private FlowLayoutPanel flowPanel;
        private DatabaseAdapter db;
        private User currentUser;
        private Panel headerPanel;
        private Label welcomeLabel;
        private Label titleLabel;

        public StudentMainForm(User user)
        {
            currentUser = user;  // user содержит user_id, login, role, full_name, email
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            LoadCourses();
        }

        private void InitializeComponent()
        {
            Text = "Студент - ДПО КПК";
            Width = 1200;
            Height = 700;
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
            welcomeLabel.Location = new Point(20, 20);

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "Доступные образовательные программы";
            titleLabel.Font = new Font("Segoe UI", 12);
            titleLabel.ForeColor = Color.FromArgb(230, 230, 230);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 55);

            headerPanel.Controls.Add(welcomeLabel);
            headerPanel.Controls.Add(titleLabel);

            // Flow Layout Panel for cards
            flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(20);
            flowPanel.WrapContents = true;
            flowPanel.FlowDirection = FlowDirection.TopDown;


            // В headerPanel, рядом с другими элементами
            Button myApplicationsBtn = new Button();
            myApplicationsBtn.Text = "Мои заявки";
            myApplicationsBtn.Size = new Size(120, 35);
            myApplicationsBtn.Location = new Point(1000, 20);  // Настройте позицию
            myApplicationsBtn.BackColor = Color.White;
            myApplicationsBtn.ForeColor = Color.FromArgb(0, 120, 215);
            myApplicationsBtn.FlatStyle = FlatStyle.Flat;
            myApplicationsBtn.Cursor = Cursors.Hand;
            myApplicationsBtn.Click += (s, e) =>
            {
                MyApplicationsForm myAppsForm = new MyApplicationsForm(currentUser);
                myAppsForm.ShowDialog();
            };
            headerPanel.Controls.Add(myApplicationsBtn);

            Controls.Add(flowPanel);
            Controls.Add(headerPanel);
        }

        private void LoadCourses()
        {
            flowPanel.Controls.Clear();

            var courses = db.GetAvailablePrograms();

            if (courses == null || courses.Count == 0)
            {
                Label noCoursesLabel = new Label();
                noCoursesLabel.Text = "На данный момент нет доступных программ";
                noCoursesLabel.Font = new Font("Segoe UI", 14);
                noCoursesLabel.ForeColor = Color.Gray;
                noCoursesLabel.AutoSize = true;
                noCoursesLabel.Location = new Point(20, 20);
                flowPanel.Controls.Add(noCoursesLabel);
                return;
            }

            foreach (var course in courses)
            {
                var card = CreateCourseCard(course);
                flowPanel.Controls.Add(card);
            }
        }

        private Panel CreateCourseCard(Course course)
        {
            Panel card = new Panel();
            card.Width = 350;
            card.Height = 250;
            card.BackColor = Color.White;
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;
            card.Tag = course;

            card.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(
                    e.Graphics,
                    card.ClientRectangle,
                    Color.LightGray,
                    ButtonBorderStyle.Solid
                );
            };

            // Заголовок
            Label titleLabel = new Label();
            titleLabel.Text = course.title;
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(0, 120, 215);
            titleLabel.Location = new Point(15, 15);
            titleLabel.Size = new Size(320, 35);
            titleLabel.AutoEllipsis = true;

            // Часы
            Label hoursLabel = new Label();
            hoursLabel.Text = $"⏱ Длительность: {course.hours} часов";
            hoursLabel.Font = new Font("Segoe UI", 10);
            hoursLabel.ForeColor = Color.DarkGray;
            hoursLabel.Location = new Point(15, 55);
            hoursLabel.AutoSize = true;

            // Стоимость
            Label priceLabel = new Label();
            string priceText = course.price > 0 ? $"{course.price:N0} ₽" : "Бесплатно";
            priceLabel.Text = $"💰 {priceText}";
            priceLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            priceLabel.ForeColor = course.price > 0 ? Color.FromArgb(255, 140, 0) : Color.Green;
            priceLabel.Location = new Point(15, 80);
            priceLabel.AutoSize = true;

            // Статус
            Label statusLabel = new Label();
            statusLabel.Text = $"📋 Статус: {GetStatusText(course.status)}";
            statusLabel.Font = new Font("Segoe UI", 10);
            statusLabel.ForeColor = Color.DarkGray;
            statusLabel.Location = new Point(15, 105);
            statusLabel.AutoSize = true;

            // Описание
            Label descLabel = new Label();
            descLabel.Text = course.description ?? "Описание отсутствует";
            descLabel.Font = new Font("Segoe UI", 9);
            descLabel.ForeColor = Color.Gray;
            descLabel.Location = new Point(15, 135);
            descLabel.Size = new Size(320, 40);
            descLabel.AutoEllipsis = true;

            // Кнопки
            Button detailsBtn = new Button();
            detailsBtn.Text = "Подробнее";
            detailsBtn.Size = new Size(100, 30);
            detailsBtn.Location = new Point(15, 190);
            detailsBtn.BackColor = Color.FromArgb(240, 240, 240);
            detailsBtn.FlatStyle = FlatStyle.Flat;
            detailsBtn.FlatAppearance.BorderSize = 1;
            detailsBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            detailsBtn.ForeColor = Color.FromArgb(0, 120, 215);
            detailsBtn.Cursor = Cursors.Hand;
            detailsBtn.Click += (sender, e) => ShowCourseDetails(course);

            Button applyBtn = new Button();
            applyBtn.Text = "Подать заявку";
            applyBtn.Size = new Size(120, 30);
            applyBtn.Location = new Point(215, 190);
            applyBtn.BackColor = Color.FromArgb(0, 120, 215);
            applyBtn.FlatStyle = FlatStyle.Flat;
            applyBtn.FlatAppearance.BorderSize = 0;
            applyBtn.ForeColor = Color.White;
            applyBtn.Cursor = Cursors.Hand;
            applyBtn.Click += (sender, e) => ApplyForCourse(course);

            card.Controls.Add(titleLabel);
            card.Controls.Add(hoursLabel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(statusLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(detailsBtn);
            card.Controls.Add(applyBtn);

            card.Click += (sender, e) => ShowCourseDetails(course);

            return card;
        }

        private string GetStatusText(string status)
        {
            if (status == "approved") return "Доступен";
            if (status == "pending") return "На рассмотрении";
            if (status == "rejected") return "Отклонен";
            return status ?? "Неизвестно";
        }

        private void ShowCourseDetails(Course course)
        {
            Form detailsForm = new Form();
            detailsForm.Text = course.title;
            detailsForm.Width = 500;
            detailsForm.Height = 450;
            detailsForm.StartPosition = FormStartPosition.CenterParent;
            detailsForm.BackColor = Color.White;
            detailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            detailsForm.MaximizeBox = false;
            detailsForm.MinimizeBox = false;

            // Используем Panel вместо TableLayoutPanel для простоты
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.AutoScroll = true;
            contentPanel.Padding = new Padding(20);

            int yOffset = 20;
            int labelX = 20;
            int valueX = 150;
            int lineHeight = 35;

            // Название
            Label titleLabel = new Label();
            titleLabel.Text = "Название:";
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.Location = new Point(labelX, yOffset);
            titleLabel.AutoSize = true;
            contentPanel.Controls.Add(titleLabel);

            Label titleValue = new Label();
            titleValue.Text = course.title;
            titleValue.Font = new Font("Segoe UI", 11);
            titleValue.Location = new Point(valueX, yOffset);
            titleValue.AutoSize = true;
            contentPanel.Controls.Add(titleValue);

            yOffset += lineHeight;

            // Длительность
            Label durationLabel = new Label();
            durationLabel.Text = "Длительность:";
            durationLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            durationLabel.Location = new Point(labelX, yOffset);
            durationLabel.AutoSize = true;
            contentPanel.Controls.Add(durationLabel);

            Label durationValue = new Label();
            durationValue.Text = $"{course.hours} часов";
            durationValue.Font = new Font("Segoe UI", 11);
            durationValue.Location = new Point(valueX, yOffset);
            durationValue.AutoSize = true;
            contentPanel.Controls.Add(durationValue);

            yOffset += lineHeight;

            // Стоимость
            Label priceLabel = new Label();
            priceLabel.Text = "Стоимость:";
            priceLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            priceLabel.Location = new Point(labelX, yOffset);
            priceLabel.AutoSize = true;
            contentPanel.Controls.Add(priceLabel);

            Label priceValue = new Label();
            string priceText = course.price > 0 ? $"{course.price:N0} ₽" : "Бесплатно";
            priceValue.Text = priceText;
            priceValue.Font = new Font("Segoe UI", 11);
            priceValue.ForeColor = course.price > 0 ? Color.FromArgb(255, 140, 0) : Color.Green;
            priceValue.Location = new Point(valueX, yOffset);
            priceValue.AutoSize = true;
            contentPanel.Controls.Add(priceValue);

            yOffset += lineHeight;

            // Статус
            Label statusLabel = new Label();
            statusLabel.Text = "Статус:";
            statusLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            statusLabel.Location = new Point(labelX, yOffset);
            statusLabel.AutoSize = true;
            contentPanel.Controls.Add(statusLabel);

            Label statusValue = new Label();
            statusValue.Text = GetStatusText(course.status);
            statusValue.Font = new Font("Segoe UI", 11);
            statusValue.ForeColor = course.status == "approved" ? Color.Green : Color.Orange;
            statusValue.Location = new Point(valueX, yOffset);
            statusValue.AutoSize = true;
            contentPanel.Controls.Add(statusValue);

            yOffset += lineHeight;

            // Описание
            Label descLabel = new Label();
            descLabel.Text = "Описание:";
            descLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            descLabel.Location = new Point(labelX, yOffset);
            descLabel.AutoSize = true;
            contentPanel.Controls.Add(descLabel);

            Label descValue = new Label();
            descValue.Text = course.description ?? "Описание отсутствует";
            descValue.Font = new Font("Segoe UI", 11);
            descValue.Location = new Point(valueX, yOffset);
            descValue.Size = new Size(280, 60);
            descValue.AutoEllipsis = true;
            contentPanel.Controls.Add(descValue);

            yOffset += 70;

            // Кнопка подачи заявки
            Button applyButton = new Button();
            applyButton.Text = "Подать заявку на программу";
            applyButton.Size = new Size(220, 40);
            applyButton.Location = new Point(130, yOffset);
            applyButton.BackColor = Color.FromArgb(0, 120, 215);
            applyButton.ForeColor = Color.White;
            applyButton.FlatStyle = FlatStyle.Flat;
            applyButton.FlatAppearance.BorderSize = 0;
            applyButton.Cursor = Cursors.Hand;
            applyButton.Click += (sender, e) =>
            {
                ApplyForCourse(course);
                detailsForm.Close();
            };
            contentPanel.Controls.Add(applyButton);

            detailsForm.Controls.Add(contentPanel);
            detailsForm.ShowDialog(this);
        }



        private void ApplyForCourse(Course course)
        {
            try
            {
                // Получаем студента по user_id
                var student = db.GetStudentByUserId(currentUser.user_id);  // Используем user_id вместо id

                if (student == null)
                {
                    MessageBox.Show(
                        "Ошибка: профиль студента не найден. Обратитесь к администратору.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Проверяем, не подавал ли уже заявку
                if (db.HasApplication(currentUser.user_id, course.course_id))
                {
                    MessageBox.Show(
                        "Вы уже подавали заявку на эту программу",
                        "Информация",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Создаем заявку
                bool success = db.CreateApplication(student.student_id, course.course_id);

                if (success)
                {
                    MessageBox.Show(
                        $"Заявка на программу \"{course.title}\" успешно подана!\n\n" +
                        "В течении нескольких часов заявка будет обработана",
                        "Заявка подана",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Произошла ошибка при подаче заявки. Попробуйте позже.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}