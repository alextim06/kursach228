using kursach;
using System.Drawing;
using System.Windows.Forms;

public class ApplicationUserWindow : Form
{
    private User _user;

    public ApplicationUserWindow(User user)
    {
        _user = user;

        Text = "Заявки";
        Width = 600;
        Height = 400;

        Label lbl = new Label
        {
            Text = "Мои заявки",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.Add(lbl);
    }
}