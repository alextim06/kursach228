using System;
using System.Drawing;
using System.Windows.Forms;

namespace kursach
{
    public class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private DatabaseAdapter db;

        public LoginForm()
        {
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            BuildUI();
        }

        private void BuildUI()
        {
            // FORM
            Text = "Авторизация";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 500);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // LEFT PANEL
            Panel left = new Panel();
            left.Dock = DockStyle.Left;
            left.Width = 350;
            left.BackColor = Color.FromArgb(0, 120, 215);
            Controls.Add(left);

            // LOGO
            Label logo = new Label();
            logo.Text = "ДПО КПК";
            logo.ForeColor = Color.White;
            logo.Font = new Font("Segoe UI", 30, FontStyle.Bold);
            logo.AutoSize = true;
            logo.Location = new Point(55, 150);
            left.Controls.Add(logo);

            // DESCRIPTION
            Label desc = new Label();
            desc.Text = "Информационная система\r\nдля образовательного центра";
            desc.ForeColor = Color.White;
            desc.Font = new Font("Segoe UI", 13);
            desc.AutoSize = true;
            desc.Location = new Point(58, 240);
            left.Controls.Add(desc);

            // RIGHT PANEL
            Panel right = new Panel();
            right.Dock = DockStyle.Fill;
            right.BackColor = Color.White;
            Controls.Add(right);

            // Вычисляем центр правой панели
            int formWidth = 900;
            int leftPanelWidth = 350;
            int rightPanelWidth = formWidth - leftPanelWidth; // 550 пикселей

            // Ширина элементов управления
            int controlWidth = 300;

            // Позиция X для центрирования элементов
            int centerX = leftPanelWidth + (rightPanelWidth - controlWidth) / 2;
            // centerX = 350 + (550 - 300) / 2 = 350 + 125 = 475

            // TITLE
            Label title = new Label();
            title.Text = "Авторизация";
            title.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            title.AutoSize = true;
            // Для текста считаем его ширину
            using (Graphics g = title.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(title.Text, title.Font);
                title.Location = new Point(
                    leftPanelWidth + (rightPanelWidth - (int)textSize.Width) / 2,
                    90
                );
            }
            right.Controls.Add(title);

            // SUBTITLE
            Label subtitle = new Label();
            subtitle.Text = "Введите логин и пароль";
            subtitle.Font = new Font("Segoe UI", 10);
            subtitle.ForeColor = Color.Gray;
            subtitle.AutoSize = true;
            using (Graphics g = subtitle.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(subtitle.Text, subtitle.Font);
                subtitle.Location = new Point(
                    leftPanelWidth + (rightPanelWidth - (int)textSize.Width) / 2,
                    145
                );
            }
            right.Controls.Add(subtitle);

            // LOGIN LABEL
            Label loginLabel = new Label();
            loginLabel.Text = "Логин";
            loginLabel.Font = new Font("Segoe UI", 9);
            loginLabel.AutoSize = true;
            loginLabel.Location = new Point(centerX, 210);
            right.Controls.Add(loginLabel);

            // LOGIN TEXTBOX
            txtLogin = new TextBox();
            txtLogin.Size = new Size(controlWidth, 35);
            txtLogin.Location = new Point(centerX, 235);
            txtLogin.Font = new Font("Segoe UI", 11);
            right.Controls.Add(txtLogin);

            // PASSWORD LABEL
            Label passLabel = new Label();
            passLabel.Text = "Пароль";
            passLabel.Font = new Font("Segoe UI", 9);
            passLabel.AutoSize = true;
            passLabel.Location = new Point(centerX, 295);
            right.Controls.Add(passLabel);

            // PASSWORD TEXTBOX
            txtPassword = new TextBox();
            txtPassword.Size = new Size(controlWidth, 35);
            txtPassword.Location = new Point(centerX, 320);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.UseSystemPasswordChar = true;
            right.Controls.Add(txtPassword);

            // BUTTON
            Button btnLogin = new Button();
            btnLogin.Text = "Войти";
            btnLogin.Size = new Size(controlWidth, 45);
            btnLogin.Location = new Point(centerX, 395);
            btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += LoginClick;
            right.Controls.Add(btnLogin);
        }


        private void LoginClick(
    object sender,
    EventArgs e
)
        {
            string login = txtLogin.Text.Trim();

            string password = txtPassword.Text.Trim();

            User user = db.AuthenticateUser(
                login,
                password
            );

            if (user == null)
            {
                MessageBox.Show(
                    "Неверный логин или пароль",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            MessageBox.Show(
                $"Добро пожаловать, {user.full_name}",
                "Успех",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // ADMIN

            if (user.role == "admin")
            {
                new AdminMainForm().Show();

                this.Hide();
            }

            // STUDENT

            else if (user.role == "student")
            {
                new StudentMainForm(user).Show();

                this.Hide();
            }

            // TEACHER

            else if (user.role == "teacher")
            {
                new TeacherMainForm(user).Show();

                this.Hide();
            }
        }
    }
}