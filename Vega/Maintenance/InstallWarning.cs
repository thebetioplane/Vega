using System;
using System.IO;
using System.Windows.Forms;

namespace Vega.Maintenance
{
    class BigButton
        : Button
    {
        public BigButton(int n, EventHandler evt, string text)
        {
            this.Text = text;
            this.Left = 75;
            this.Top = (n * 45) + 100;
            this.Height = 40;
            this.Width = 250;
            this.Click += evt;
        }
    }
    public class InstallWarning : Form
    {
        public InstallWarning()
        {
            Logger.DefaultLogger.WriteLine("Install warning shown");
            this.Text = "Install Warning";
            this.Width = 400;
            this.Height = 270;
            Label l = new Label();
            l.Top = 5;
            l.Left = 5;
            l.Width = 390;
            l.Height = 95;
            l.Text = @"Vega is about to create a bunch of files. If this program is on your desktop and not in it's own folder it's probably going to clutter things up. Is this okay with you?";
            this.Controls.Add(l);
            this.Controls.Add(new BigButton(0, this.Fine, "Yes, this folder is fine, Vega.exe is the only thing here."));
            this.Controls.Add(new BigButton(1, this.NotFine, "No, exit and let me move the .exe myself."));
            this.Controls.Add(new BigButton(2, this.Lazy, "No, but I am too lazy to make a new folder and move the exe, can you do it for me?"));
        }
        private void Fine(object sender, EventArgs e)
        {
            Logger.DefaultLogger.WriteLine("Response: Fine");
            this.Close();
        }
        private void NotFine(object sender, EventArgs e)
        {
            Logger.DefaultLogger.WriteLine("Response: NotFine");
            Environment.Exit(0);
        }
        private void Lazy(object sender, EventArgs e)
        {
            Logger.DefaultLogger.WriteLine("Response: Lazy");
            if (Directory.Exists("VEGA"))
            {
                Logger.DefaultLogger.WriteLine("Directory already exists");
                MessageBox.Show("The directory already exits", "Problem",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            Directory.CreateDirectory("VEGA");
            File.Move(Application.ExecutablePath, Path.Combine("VEGA", "Vega.exe"));
            Directory.SetCurrentDirectory("VEGA");
            this.Close();
        }
    }
}