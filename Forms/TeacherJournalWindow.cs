using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace kursach
{
    public class TeacherJournalForm : Form
    {
        private DatabaseAdapter db;
        private User currentUser;
        private DataGridView dgvAttendance;
        private ComboBox cbCourses;
        private ComboBox cbSchedule;
        private Button btnSave;
        private Button btnRefresh;
        private Button btnClose;
        private Teacher currentTeacher;
        private Panel headerPanel;
        private Panel filterPanel;
        private Panel buttonPanel;
        private Label lblCourse;
        private Label lblLesson;

        public TeacherJournalForm(User user)
        {
            currentUser = user;
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
            this.Text = "Журнал посещаемости - ДПО Portal";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215);
            headerPanel.Padding = new Padding(20);

            Label titleLabel = new Label();
            titleLabel.Text = "Журнал посещаемости";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 25);
            headerPanel.Controls.Add(titleLabel);

            // Filter Panel
            filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 80;
            filterPanel.BackColor = Color.FromArgb(245, 245, 245);
            filterPanel.Padding = new Padding(20);

            lblCourse = new Label();
            lblCourse.Text = "Курс:";
            lblCourse.Font = new Font("Segoe UI", 10);
            lblCourse.Location = new Point(20, 30);
            lblCourse.Size = new Size(50, 25);
            filterPanel.Controls.Add(lblCourse);

            cbCourses = new ComboBox();
            cbCourses.Size = new Size(250, 30);
            cbCourses.Location = new Point(80, 27);
            cbCourses.Font = new Font("Segoe UI", 10);
            cbCourses.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCourses.SelectedIndexChanged += CbCourses_SelectedIndexChanged;
            filterPanel.Controls.Add(cbCourses);

            lblLesson = new Label();
            lblLesson.Text = "Занятие:";
            lblLesson.Font = new Font("Segoe UI", 10);
            lblLesson.Location = new Point(380, 30);
            lblLesson.Size = new Size(60, 25);
            filterPanel.Controls.Add(lblLesson);

            cbSchedule = new ComboBox();
            cbSchedule.Size = new Size(250, 30);
            cbSchedule.Location = new Point(450, 27);
            cbSchedule.Font = new Font("Segoe UI", 10);
            cbSchedule.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSchedule.SelectedIndexChanged += CbSchedule_SelectedIndexChanged;
            filterPanel.Controls.Add(cbSchedule);

            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Location = new Point(750, 27);
            btnRefresh.BackColor = Color.FromArgb(0, 120, 215);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadAttendance();
            filterPanel.Controls.Add(btnRefresh);

            // DataGridView
            dgvAttendance = new DataGridView();
            dgvAttendance.Dock = DockStyle.Fill;
            dgvAttendance.BackgroundColor = Color.White;
            dgvAttendance.BorderStyle = BorderStyle.None;
            dgvAttendance.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAttendance.RowHeadersVisible = true;
            dgvAttendance.RowHeadersWidth = 40;
            dgvAttendance.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvAttendance.AllowUserToAddRows = false;
            dgvAttendance.AllowUserToDeleteRows = false;
            dgvAttendance.RowTemplate.Height = 35;
            dgvAttendance.Font = new Font("Segoe UI", 10);

            // Button Panel
            buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.White;
            buttonPanel.Padding = new Padding(20);

            btnSave = new Button();
            btnSave.Text = "Сохранить посещаемость";
            btnSave.Size = new Size(180, 40);
            btnSave.Location = new Point(20, 10);
            btnSave.BackColor = Color.FromArgb(76, 175, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;
            buttonPanel.Controls.Add(btnSave);

            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Size = new Size(100, 40);
            btnClose.Location = new Point(880, 10);
            btnClose.BackColor = Color.FromArgb(108, 117, 125);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(btnClose);

            // Add controls to form
            this.Controls.Add(dgvAttendance);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(filterPanel);
            this.Controls.Add(headerPanel);
        }

        private void LoadCourses()
        {
            currentTeacher = db.GetTeacherByUserId(currentUser.user_id);
            if (currentTeacher == null)
            {
                MessageBox.Show("Преподаватель не найден");
                return;
            }

            var courses = db.GetCoursesByTeacher(currentTeacher.teacher_id);
            cbCourses.DataSource = courses;
            cbCourses.DisplayMember = "title";
            cbCourses.ValueMember = "course_id";

            if (courses.Count == 0)
            {
                cbCourses.Enabled = false;
                cbSchedule.Enabled = false;
                btnSave.Enabled = false;
                MessageBox.Show("У вас нет назначенных курсов");
            }
        }

        private void CbCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCourses.SelectedValue != null && cbCourses.SelectedValue is int)
            {
                int courseId = (int)cbCourses.SelectedValue;
                var schedule = db.GetScheduleByCourse(courseId);
                cbSchedule.DataSource = schedule;
                cbSchedule.DisplayMember = "topic";
                cbSchedule.ValueMember = "schedule_id";

                if (schedule.Count == 0)
                {
                    cbSchedule.Enabled = false;
                    MessageBox.Show("Для этого курса нет расписания занятий");
                }
                else
                {
                    cbSchedule.Enabled = true;
                }
            }
        }

        private void CbSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAttendance();
        }

        private void LoadAttendance()
        {
            if (cbSchedule.SelectedValue == null || !(cbSchedule.SelectedValue is int))
                return;

            int scheduleId = (int)cbSchedule.SelectedValue;
            DataTable attendance = db.GetStudentsForAttendance(scheduleId);

            // Добавляем колонку CheckBox если ее нет
            if (!attendance.Columns.Contains("attended"))
            {
                attendance.Columns.Add("attended", typeof(bool));
            }

            // Заполняем CheckBox на основе status
            foreach (DataRow row in attendance.Rows)
            {
                string status = row["status"] != DBNull.Value ? row["status"].ToString() : "";
                row["attended"] = (status == "present");
            }

            dgvAttendance.DataSource = attendance;

            // Настройка колонок
            if (dgvAttendance.Columns.Count > 0)
            {
                if (dgvAttendance.Columns["student_id"] != null)
                    dgvAttendance.Columns["student_id"].Visible = false;
                if (dgvAttendance.Columns["attendance_id"] != null)
                    dgvAttendance.Columns["attendance_id"].Visible = false;

                if (dgvAttendance.Columns["student_name"] != null)
                    dgvAttendance.Columns["student_name"].HeaderText = "Студент";

                if (dgvAttendance.Columns["status"] != null)
                    dgvAttendance.Columns["status"].Visible = false;

                if (dgvAttendance.Columns["attended"] != null)
                {
                    dgvAttendance.Columns["attended"].HeaderText = "Присутствовал";
                    dgvAttendance.Columns["attended"].Width = 100;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSchedule.SelectedValue == null || !(cbSchedule.SelectedValue is int))
                {
                    MessageBox.Show("Выберите занятие");
                    return;
                }

                int scheduleId = (int)cbSchedule.SelectedValue;

                // Сохраняем посещаемость для каждого студента
                foreach (DataGridViewRow row in dgvAttendance.Rows)
                {
                    if (row.IsNewRow) continue;

                    int studentId = Convert.ToInt32(row.Cells["student_id"].Value);
                    bool attended = Convert.ToBoolean(row.Cells["attended"].Value);
                    string status = attended ? "present" : "absent";

                    db.SaveAttendance(studentId, scheduleId, status);
                }

                MessageBox.Show(
                    "Посещаемость успешно сохранена!",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadAttendance(); // Обновляем отображение
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}