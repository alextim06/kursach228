using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace kursach
{
    public class AdminCourseRequestsForm : Form
    {
        private DatabaseAdapter db;
        private DataGridView dgvRequests;
        private Button btnApprove;
        private Button btnReject;
        private Button btnRefresh;
        private Button btnClose;
        private Panel headerPanel;
        private Panel buttonPanel;

        public AdminCourseRequestsForm()
        {
            db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            InitializeComponent();
            LoadRequests();
        }

        private void InitializeComponent()
        {
            Text = "Обработка заявок на курсы - ДПО Portal";
            Width = 1000;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20);

            Label lblTitle = new Label();
            lblTitle.Text = "Заявки на добавление курсов";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);
            headerPanel.Controls.Add(lblTitle);

            // DataGridView
            dgvRequests = new DataGridView();
            dgvRequests.Dock = DockStyle.Fill;
            dgvRequests.BackgroundColor = Color.White;
            dgvRequests.BorderStyle = BorderStyle.None;
            dgvRequests.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.MultiSelect = false;
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.ReadOnly = true;
            dgvRequests.RowTemplate.Height = 35;
            dgvRequests.Font = new Font("Segoe UI", 10);

            dgvRequests.CellFormatting += DgvRequests_CellFormatting;
            dgvRequests.SelectionChanged += (s, e) => UpdateButtonsState();

            // Button Panel
            buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.White;
            buttonPanel.Padding = new Padding(20);

            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(20, 12);
            btnRefresh.BackColor = Color.FromArgb(0, 120, 215);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadRequests();
            buttonPanel.Controls.Add(btnRefresh);

            btnApprove = new Button();
            btnApprove.Text = "Одобрить";
            btnApprove.Size = new Size(100, 35);
            btnApprove.Location = new Point(140, 12);
            btnApprove.BackColor = Color.FromArgb(76, 175, 80);
            btnApprove.ForeColor = Color.White;
            btnApprove.FlatStyle = FlatStyle.Flat;
            btnApprove.Cursor = Cursors.Hand;
            btnApprove.Click += BtnApprove_Click;
            buttonPanel.Controls.Add(btnApprove);

            btnReject = new Button();
            btnReject.Text = "Отклонить";
            btnReject.Size = new Size(100, 35);
            btnReject.Location = new Point(260, 12);
            btnReject.BackColor = Color.FromArgb(220, 53, 69);
            btnReject.ForeColor = Color.White;
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.Cursor = Cursors.Hand;
            btnReject.Click += BtnReject_Click;
            buttonPanel.Controls.Add(btnReject);

            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Size = new Size(100, 35);
            btnClose.Location = new Point(880, 12);
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(btnClose);

            Controls.Add(dgvRequests);
            Controls.Add(buttonPanel);
            Controls.Add(headerPanel);
        }

        private void LoadRequests()
        {
            DataTable requests = db.GetAllCourseRequests();
            dgvRequests.DataSource = requests;

            if (dgvRequests.Columns.Count > 0)
            {
                if (dgvRequests.Columns["request_id"] != null)
                    dgvRequests.Columns["request_id"].Visible = false;
                if (dgvRequests.Columns["teacher_id"] != null)
                    dgvRequests.Columns["teacher_id"].Visible = false;

                if (dgvRequests.Columns["teacher_name"] != null)
                    dgvRequests.Columns["teacher_name"].HeaderText = "Преподаватель";

                if (dgvRequests.Columns["title"] != null)
                    dgvRequests.Columns["title"].HeaderText = "Название курса";

                if (dgvRequests.Columns["hours"] != null)
                    dgvRequests.Columns["hours"].HeaderText = "Часов";

                if (dgvRequests.Columns["format"] != null)
                    dgvRequests.Columns["format"].HeaderText = "Формат";

                if (dgvRequests.Columns["schedule_file"] != null)
                    dgvRequests.Columns["schedule_file"].HeaderText = "Файл расписания";

                if (dgvRequests.Columns["status"] != null)
                    dgvRequests.Columns["status"].HeaderText = "Статус";
            }

            UpdateButtonsState();
        }

        private void DgvRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvRequests.Columns["status"] != null && e.ColumnIndex == dgvRequests.Columns["status"].Index)
            {
                string status = e.Value?.ToString();
                if (status == "pending")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 248, 225);
                    e.CellStyle.ForeColor = Color.FromArgb(255, 140, 0);
                    e.Value = "На рассмотрении";
                }
                else if (status == "approved")
                {
                    e.CellStyle.BackColor = Color.FromArgb(225, 255, 225);
                    e.CellStyle.ForeColor = Color.Green;
                    e.Value = "Одобрена";
                }
                else if (status == "rejected")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 225, 225);
                    e.CellStyle.ForeColor = Color.Red;
                    e.Value = "Отклонена";
                }
            }
        }

        private void UpdateButtonsState()
        {
            bool hasSelection = dgvRequests.CurrentRow != null;
            if (hasSelection && dgvRequests.CurrentRow.Cells["status"] != null)
            {
                string status = dgvRequests.CurrentRow.Cells["status"]?.Value?.ToString();
                bool isPending = status == "pending";
                btnApprove.Enabled = isPending;
                btnReject.Enabled = isPending;
            }
            else
            {
                btnApprove.Enabled = false;
                btnReject.Enabled = false;
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null) return;

            int requestId = Convert.ToInt32(dgvRequests.CurrentRow.Cells["request_id"].Value);
            string courseTitle = dgvRequests.CurrentRow.Cells["title"].Value.ToString();
            int teacherId = Convert.ToInt32(dgvRequests.CurrentRow.Cells["teacher_id"].Value);
            int hours = Convert.ToInt32(dgvRequests.CurrentRow.Cells["hours"].Value);
            string format = dgvRequests.CurrentRow.Cells["format"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"Одобрить заявку на курс:\n\n\"{courseTitle}\"?\n\nКурс будет добавлен в систему.",
                "Подтверждение одобрения",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                bool success = db.ApproveCourseRequest(requestId, teacherId, courseTitle, hours, format);

                if (success)
                {
                    MessageBox.Show(
                        $"Курс \"{courseTitle}\" успешно добавлен!",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadRequests();
                }
                else
                {
                    MessageBox.Show(
                        "Ошибка при одобрении заявки",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null) return;

            int requestId = Convert.ToInt32(dgvRequests.CurrentRow.Cells["request_id"].Value);
            string courseTitle = dgvRequests.CurrentRow.Cells["title"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"Отклонить заявку на курс:\n\n\"{courseTitle}\"?",
                "Подтверждение отклонения",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                bool success = db.RejectCourseRequest(requestId);

                if (success)
                {
                    MessageBox.Show(
                        $"Заявка на курс \"{courseTitle}\" отклонена.",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadRequests();
                }
                else
                {
                    MessageBox.Show(
                        "Ошибка при отклонении заявки",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
}