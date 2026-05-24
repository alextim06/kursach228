using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kursach
{
    public class MyApplicationsForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private DataGridView dgvApplications;
        private Button btnRevoke;
        private Button btnRefresh;
        private Label lblTitle;
        private Panel headerPanel;
        private Student currentStudent;

        public MyApplicationsForm(User user)
        {
            currentUser = user;
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            LoadApplications();
        }

        private void InitializeComponent()
        {
            Text = "Мои заявки - ДПО Portal";
            Width = 900;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            MinimumSize = new Size(800, 400);

            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20, 10, 20, 10);

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Мои заявки на обучение";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 25);
            headerPanel.Controls.Add(lblTitle);

            // Panel for buttons
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.FromArgb(245, 245, 245);
            buttonPanel.Padding = new Padding(20);

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(20, 12);
            btnRefresh.BackColor = Color.FromArgb(0, 120, 215);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadApplications();
            buttonPanel.Controls.Add(btnRefresh);

            // Revoke Button
            btnRevoke = new Button();
            btnRevoke.Text = "Отозвать заявку";
            btnRevoke.Size = new Size(140, 35);
            btnRevoke.Location = new Point(160, 12);
            btnRevoke.BackColor = Color.FromArgb(220, 53, 69);
            btnRevoke.ForeColor = Color.White;
            btnRevoke.FlatStyle = FlatStyle.Flat;
            btnRevoke.FlatAppearance.BorderSize = 0;
            btnRevoke.Cursor = Cursors.Hand;
            btnRevoke.Click += BtnRevoke_Click;
            buttonPanel.Controls.Add(btnRevoke);

            // Close Button
            Button btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Size = new Size(100, 35);
            btnClose.Location = new Point(780, 12);
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(btnClose);

            // DataGridView
            dgvApplications = new DataGridView();
            dgvApplications.Dock = DockStyle.Fill;
            dgvApplications.BackgroundColor = Color.White;
            dgvApplications.BorderStyle = BorderStyle.None;
            dgvApplications.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvApplications.GridColor = Color.FromArgb(230, 230, 230);
            dgvApplications.RowHeadersVisible = false;
            dgvApplications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvApplications.MultiSelect = false;
            dgvApplications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvApplications.AllowUserToAddRows = false;
            dgvApplications.AllowUserToDeleteRows = false;
            dgvApplications.ReadOnly = true;
            dgvApplications.RowTemplate.Height = 35;
            dgvApplications.Font = new Font("Segoe UI", 10);

            // Cell formatting
            dgvApplications.CellFormatting += DgvApplications_CellFormatting;

            Controls.Add(dgvApplications);
            Controls.Add(buttonPanel);
            Controls.Add(headerPanel);
        }

        private void LoadApplications()
        {
            try
            {
                // Получаем студента
                currentStudent = db.GetStudentByUserId(currentUser.user_id);

                if (currentStudent == null)
                {
                    MessageBox.Show(
                        "Профиль студента не найден",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Получаем заявки студента
                var applications = db.GetApplicationsByStudentWithDetails(currentStudent.student_id);

                dgvApplications.DataSource = null;
                dgvApplications.DataSource = applications;

                // Настройка колонок
                if (dgvApplications.Columns.Count > 0)
                {
                    // Скрываем ненужные колонки
                    if (dgvApplications.Columns["application_id"] != null)
                        dgvApplications.Columns["application_id"].Visible = false;
                    if (dgvApplications.Columns["student_id"] != null)
                        dgvApplications.Columns["student_id"].Visible = false;
                    if (dgvApplications.Columns["course_id"] != null)
                        dgvApplications.Columns["course_id"].Visible = false;

                    // Переименовываем колонки
                    if (dgvApplications.Columns["course_title"] != null)
                        dgvApplications.Columns["course_title"].HeaderText = "Название курса";

                    if (dgvApplications.Columns["status"] != null)
                        dgvApplications.Columns["status"].HeaderText = "Статус";

                    if (dgvApplications.Columns["date_created"] != null)
                    {
                        dgvApplications.Columns["date_created"].HeaderText = "Дата подачи";
                        dgvApplications.Columns["date_created"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                    }
                }

                // Обновляем статус кнопки отзыва
                btnRevoke.Enabled = dgvApplications.Rows.Count > 0;

                if (dgvApplications.Rows.Count == 0)
                {
                    ShowEmptyMessage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при загрузке заявок: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ShowEmptyMessage()
        {
            // Создаем панель с сообщением об отсутствии заявок
            Panel emptyPanel = new Panel();
            emptyPanel.Dock = DockStyle.Fill;
            emptyPanel.BackColor = Color.White;

            Label emptyLabel = new Label();
            emptyLabel.Text = "У вас пока нет поданных заявок";
            emptyLabel.Font = new Font("Segoe UI", 14);
            emptyLabel.ForeColor = Color.Gray;
            emptyLabel.AutoSize = true;
            emptyLabel.Location = new Point(
                (this.ClientSize.Width - emptyLabel.Width) / 2,
                (this.ClientSize.Height - emptyLabel.Height) / 2
            );

            emptyPanel.Controls.Add(emptyLabel);

            // Если DataGridView пуст, показываем сообщение
            if (dgvApplications.Rows.Count == 0)
            {
                // Временно отключаем DataGridView и показываем сообщение
                // Но лучше добавить обработчик Resize для центрирования
            }
        }

        private void DgvApplications_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Окрашиваем строки в зависимости от статуса
            if (e.RowIndex >= 0 && dgvApplications.Rows[e.RowIndex].DataBoundItem != null)
            {
                var status = dgvApplications.Rows[e.RowIndex].Cells["status"]?.Value?.ToString();

                if (status == "pending")
                {
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 225);
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(255, 140, 0);
                }
                else if (status == "approved")
                {
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(225, 255, 225);
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green;
                }
                else if (status == "rejected")
                {
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 225, 225);
                    dgvApplications.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void BtnRevoke_Click(object sender, EventArgs e)
        {
            if (dgvApplications.CurrentRow == null)
            {
                MessageBox.Show(
                    "Выберите заявку для отзыва",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Получаем выбранную заявку
            var selectedApplication = dgvApplications.CurrentRow.DataBoundItem as dynamic;
            int applicationId = selectedApplication.application_id;
            string courseTitle = selectedApplication.course_title;
            string status = selectedApplication.status;

            // Проверяем статус заявки
            if (status != "pending")
            {
                MessageBox.Show(
                    $"Нельзя отозвать заявку со статусом \"{GetStatusText(status)}\"\n" +
                    "Отозвать можно только заявки в статусе 'На рассмотрении'",
                    "Невозможно отозвать",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Подтверждение отзыва
            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите отозвать заявку на курс:\n\n\"{courseTitle}\"?\n\n" +
                "Это действие нельзя отменить.",
                "Подтверждение отзыва",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Удаляем заявку
                bool success = db.DeleteApplication(applicationId);

                if (success)
                {
                    MessageBox.Show(
                        $"Заявка на курс \"{courseTitle}\" успешно отозвана",
                        "Заявка отозвана",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Обновляем список заявок
                    LoadApplications();
                }
                else
                {
                    MessageBox.Show(
                        "Ошибка при отзыве заявки. Попробуйте позже.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private string GetStatusText(string status)
        {
            if (status == "pending") return "На рассмотрении";
            if (status == "approved") return "Одобрена";
            if (status == "rejected") return "Отклонена";
            return status ?? "Неизвестно";
        }

        // Класс для отображения заявок с деталями
        public class ApplicationWithDetails
        {
            public int application_id { get; set; }
            public int student_id { get; set; }
            public int course_id { get; set; }
            public string course_title { get; set; }
            public string status { get; set; }
            public DateTime date_created { get; set; }
        }
    }
}