using kursach;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class ProgramUserWindow : Form
{
    private User _user;

    public ProgramUserWindow(User user)
    {
        _user = user;

        Text = "Программа";
        Width = 600;
        Height = 400;

        Label lbl = new Label
        {
            Text = "Учебная программа",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.Add(lbl);
    }
}