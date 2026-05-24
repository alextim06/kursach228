using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kursach
{
    public class TeacherCourseRequestsForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private DataGridView dgvRequests;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnAdd;
        private Teacher currentTeacher;

        public TeacherCourseRequestsForm(User user)
        {
            currentUser = user;
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
            Text = "Мои заявки на курсы - ДПО Portal";
            Width = 1000;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20);

            Label titleLabel = new Label();
            titleLabel.Text = "Мои заявки на курсы";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 25);
            headerPanel.Controls.Add(titleLabel);

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

            // Color status cells
            dgvRequests.CellFormatting += DgvRequests_CellFormatting;

            // Button Panel
            Panel buttonPanel = new Panel();
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

            btnEdit = new Button();
            btnEdit.Text = "✏ Редактировать";
            btnEdit.Size = new Size(130, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.FromArgb(255, 193, 7);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += BtnEdit_Click;
            buttonPanel.Controls.Add(btnEdit);

            btnDelete = new Button();
            btnDelete.Text = "🗑 Удалить";
            btnDelete.Size = new Size(100, 35);
            btnDelete.Location = new Point(290, 12);
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += BtnDelete_Click;
            buttonPanel.Controls.Add(btnDelete);

            btnAdd = new Button();
            btnAdd.Text = "➕ Новая заявка";
            btnAdd.Size = new Size(130, 35);
            btnAdd.Location = new Point(410, 12);
            btnAdd.BackColor = Color.FromArgb(40, 167, 69);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) =>
            {
                AddCourseRequestForm addForm = new AddCourseRequestForm(currentUser);
                addForm.ShowDialog();
                LoadRequests();
            };
            buttonPanel.Controls.Add(btnAdd);

            Button btnClose = new Button();
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
            currentTeacher = db.GetTeacherByUserId(currentUser.user_id);
            if (currentTeacher == null)
            {
                MessageBox.Show("Преподаватель не найден");
                return;
            }

            DataTable requests = db.GetCourseRequestsByTeacher(currentTeacher.teacher_id);
            dgvRequests.DataSource = requests;

            if (dgvRequests.Columns.Count > 0)
            {
                if (dgvRequests.Columns["request_id"] != null)
                    dgvRequests.Columns["request_id"].Visible = false;
                if (dgvRequests.Columns["teacher_id"] != null)
                    dgvRequests.Columns["teacher_id"].Visible = false;

                if (dgvRequests.Columns["title"] != null)
                    dgvRequests.Columns["title"].HeaderText = "Название курса";
                if (dgvRequests.Columns["hours"] != null)
                    dgvRequests.Columns["hours"].HeaderText = "Часов";
                if (dgvRequests.Columns["format"] != null)
                    dgvRequests.Columns["format"].HeaderText = "Формат";
                if (dgvRequests.Columns["status"] != null)
                    dgvRequests.Columns["status"].HeaderText = "Статус";
            }
        }

        private void DgvRequests_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvRequests.Columns["status"]?.Index)
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

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null)
            {
                MessageBox.Show("Выберите заявку для редактирования");
                return;
            }

            dynamic request = dgvRequests.CurrentRow.DataBoundItem;
            int requestId = request.request_id;
            string status = request.status;

            if (status != "pending")
            {
                MessageBox.Show("Можно редактировать только заявки в статусе 'На рассмотрении'");
                return;
            }

            EditCourseRequestForm editForm = new EditCourseRequestForm(currentUser, requestId);
            editForm.ShowDialog();
            LoadRequests();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow == null)
            {
                MessageBox.Show("Выберите заявку для удаления");
                return;
            }

            dynamic request = dgvRequests.CurrentRow.DataBoundItem;
            int requestId = request.request_id;
            string title = request.title;
            string status = request.status;

            if (status != "pending")
            {
                MessageBox.Show("Можно удалять только заявки в статусе 'На рассмотрении'");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить заявку на курс \"{title}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                bool success = db.DeleteCourseRequest(requestId);
                if (success)
                {
                    MessageBox.Show("Заявка успешно удалена");
                    LoadRequests();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении заявки");
                }
            }
        }
    }
}