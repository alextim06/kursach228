using System;
using System.Drawing;
using System.Windows.Forms;

namespace kursach
{
    public class CourseTeacherWindow : Form
    {
        private User _user;
        private DatabaseAdapter db;

        private DataGridView dgv;
        private TextBox txtTitle;
        private NumericUpDown numHours;
        private NumericUpDown numPrice;
        private TextBox txtStatus;
        private TextBox txtDescription;
        private ComboBox cmbTeacher;

        private int selectedId = -1;

        public CourseTeacherWindow(User user)
        {
            _user = user;

            db = new DatabaseAdapter(
                "localhost",
                "education_db",
                "postgres",
                "1234"
            );

            InitUI();
            LoadTeachers();
            LoadData();
        }

        // ---------------- UI ----------------

        private void InitUI()
        {
            Text = "CRUD Курсы";
            Width = 1000;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 250,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);

            Controls.Add(new Label { Text = "Название", Top = 270, Left = 20 });
            txtTitle = new TextBox { Top = 290, Left = 20, Width = 200 };
            Controls.Add(txtTitle);

            Controls.Add(new Label { Text = "Часы", Top = 270, Left = 250 });
            numHours = new NumericUpDown { Top = 290, Left = 250, Width = 100 };
            Controls.Add(numHours);

            Controls.Add(new Label { Text = "Цена", Top = 270, Left = 370 });
            numPrice = new NumericUpDown { Top = 290, Left = 370, Width = 100, Maximum = 1000000 };
            Controls.Add(numPrice);

            Controls.Add(new Label { Text = "Статус", Top = 330, Left = 20 });
            txtStatus = new TextBox { Top = 350, Left = 20, Width = 200 };
            Controls.Add(txtStatus);

            Controls.Add(new Label { Text = "Описание", Top = 330, Left = 250 });
            txtDescription = new TextBox { Top = 350, Left = 250, Width = 300 };
            Controls.Add(txtDescription);

            Controls.Add(new Label { Text = "Преподаватель", Top = 390, Left = 20 });
            cmbTeacher = new ComboBox { Top = 410, Left = 20, Width = 200 };
            Controls.Add(cmbTeacher);

            Button btnAdd = new Button { Text = "Добавить", Top = 450, Left = 20 };
            btnAdd.Click += (s, e) => AddCourse();
            Controls.Add(btnAdd);

            Button btnUpdate = new Button { Text = "Изменить", Top = 450, Left = 120 };
            btnUpdate.Click += (s, e) => UpdateCourse();
            Controls.Add(btnUpdate);

            Button btnDelete = new Button { Text = "Удалить", Top = 450, Left = 220 };
            btnDelete.Click += (s, e) => DeleteCourse();
            Controls.Add(btnDelete);

            Button btnRefresh = new Button { Text = "Обновить", Top = 450, Left = 320 };
            btnRefresh.Click += (s, e) => LoadData();
            Controls.Add(btnRefresh);
        }

        // ---------------- LOAD ----------------

        private void LoadData()
        {
            dgv.DataSource = db.GetAllCourses();
        }

        private void LoadTeachers()
        {
            cmbTeacher.DataSource =
                db.QueryTable("SELECT teacher_id, full_name FROM Teachers");

            cmbTeacher.DisplayMember = "full_name";
            cmbTeacher.ValueMember = "teacher_id";
        }

        // ---------------- SELECT ----------------

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgv.Rows[e.RowIndex];

            selectedId = (int)row.Cells["course_id"].Value;

            txtTitle.Text = row.Cells["title"].Value.ToString();
            numHours.Value = Convert.ToDecimal(row.Cells["hours"].Value);
            numPrice.Value = Convert.ToDecimal(row.Cells["price"].Value);
            txtStatus.Text = row.Cells["status"].Value.ToString();
            txtDescription.Text = row.Cells["description"].Value.ToString();
        }

        // ---------------- CREATE ----------------

        private void AddCourse()
        {
            var course = new Course
            {
                title = txtTitle.Text,
                hours = (int)numHours.Value,
                price = numPrice.Value,
                teacher_id = Convert.ToInt32(cmbTeacher.SelectedValue),
                status = txtStatus.Text,
                description = txtDescription.Text
            };

            db.AddCourse(course);
            LoadData();
            ClearFields();
        }

        // ---------------- UPDATE ----------------

        private void UpdateCourse()
        {
            if (selectedId == -1) return;

            var course = new Course
            {
                course_id = selectedId,
                title = txtTitle.Text,
                hours = (int)numHours.Value,
                price = numPrice.Value,
                teacher_id = Convert.ToInt32(cmbTeacher.SelectedValue),
                status = txtStatus.Text,
                description = txtDescription.Text
            };

            db.UpdateCourse(course);
            LoadData();
            ClearFields();
        }

        // ---------------- DELETE ----------------

        private void DeleteCourse()
        {
            if (selectedId == -1) return;

            db.DeleteCourse(selectedId);
            LoadData();
            ClearFields();
        }

        // ---------------- CLEAR ----------------

        private void ClearFields()
        {
            txtTitle.Clear();
            txtStatus.Clear();
            txtDescription.Clear();
            numHours.Value = 0;
            numPrice.Value = 0;
            selectedId = -1;
        }
    }
}