using System;
using System.Threading;
using System.Windows.Forms;

namespace Vega.Maintenance
{
    public class UpdaterGraphical : Form
    {
        private Label Output;
        private ProgressBar Bar;
        public UpdaterGraphical()
        {
            this.Text = "Vega Updater";
            this.Width = 400;
            this.Height = 100;
            this.Output = new Label();
            this.Output.Text = "Getting ready";
            this.Output.Left = 5;
            this.Output.Top = 15;
            this.Output.Width = 385;
            this.Controls.Add(this.Output);
            this.Bar = new ProgressBar();
            this.Bar.Left = 5;
            this.Bar.Top = 40;
            this.Bar.Value = 0;
            this.Bar.Height = 30;
            this.Bar.Width = 385;
            this.Controls.Add(this.Bar);
            Thread thread = new Thread(this.Run);
            thread.Start();
        }
        private void Run()
        {
            Updater.DefaultUpdater.PercentChanged += this.PercentChangeListener;
            Updater.DefaultUpdater.TextChanged += this.TextChangeListener;
            bool success = Updater.DefaultUpdater.Run(true);
            if (! success)
                this.Text = "Vega Updater (failed)";
            Updater.DefaultUpdater.PercentChanged -= this.PercentChangeListener;
            Updater.DefaultUpdater.TextChanged -= this.TextChangeListener;
            if (success)
                this.Close();
        }
        private void PercentChangeListener(object sender, int value)
        {
            this.Bar.Value = value;
        }

        private void TextChangeListener(object sender, string value)
        {
            this.Output.Text = value;
        }
    }
}