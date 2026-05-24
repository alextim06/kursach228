using System;
using System.Drawing;
using System.Windows.Forms;

namespace kursach
{
    public class AdminMainForm : Form
    {
        private DatabaseAdapter db;
        private Panel headerPanel;
        private Label welcomeLabel;
        private Button btnCourseRequests;


        public AdminMainForm()
        {
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Администратор - ДПО Portal";
            Width = 500;
            Height = 300;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20, 10, 20, 10);

            // Welcome Label
            welcomeLabel = new Label();
            welcomeLabel.Text = "Панель администратора";
            welcomeLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.AutoSize = true;
            welcomeLabel.Location = new Point(20, 25);
            headerPanel.Controls.Add(welcomeLabel);

            // Main Panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            mainPanel.Padding = new Padding(50, 50, 50, 50);

            // Button: Обработка заявок на курсы
            btnCourseRequests = new Button();
            btnCourseRequests.Text = "📋 Заявки на курсы";
            btnCourseRequests.Size = new Size(350, 80);
            btnCourseRequests.Location = new Point(25, 20);
            btnCourseRequests.BackColor = Color.FromArgb(0, 120, 215);
            btnCourseRequests.ForeColor = Color.White;
            btnCourseRequests.FlatStyle = FlatStyle.Flat;
            btnCourseRequests.FlatAppearance.BorderSize = 0;
            btnCourseRequests.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCourseRequests.Cursor = Cursors.Hand;
            btnCourseRequests.Click += (s, e) => OpenCourseRequestsWindow();
            mainPanel.Controls.Add(btnCourseRequests);

    

            Controls.Add(mainPanel);
            Controls.Add(headerPanel);
        }

        private void OpenCourseRequestsWindow()
        {
            AdminCourseRequestsForm requestsForm = new AdminCourseRequestsForm();
            requestsForm.ShowDialog();
        }
    }
}