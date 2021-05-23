using System;
using System.Windows.Forms;
using System.Linq;

namespace CaveSkirmish
{
    public class DungeonForm : Form
    {
        private readonly MapDirector mapDirector;
        private readonly ScaledViewPanel scaledViewPanel;
        private readonly Timer timer;
        private bool gameIsEnded;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;          
            Text = "Cave Skirmish";
        }

        public DungeonForm()
        {
            var level = Maps.MapsToList().OrderBy(x => Guid.NewGuid()).First();
            mapDirector = new MapDirector(level);
            scaledViewPanel = new ScaledViewPanel(mapDirector) { Dock = DockStyle.Fill };
            Controls.Add(scaledViewPanel);

            timer = new Timer { Interval = 50 };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (mapDirector.IsLost)
            {
                if (gameIsEnded)
                    return;
                gameIsEnded = true;
                DefeatMessageBox();
            }
            else if (mapDirector.IsWon)
            {
                if (gameIsEnded)
                    return;
                gameIsEnded = true;
                VictoryMessageBox();
            }
            else
            {
                scaledViewPanel.Invalidate();
                mapDirector.Update();
            }               
        }

        private void VictoryMessageBox()
        {
            var message = "Congratulations! You won";
            var caption = "Кака пися";
            var buttons = MessageBoxButtons.OK;

            var result = MessageBox.Show(this, message, caption, buttons);

            if (result == DialogResult.OK)
                Close();
        }

        private void DefeatMessageBox()
        {
            var message = "Utterly defeated (dead)";
            var caption = "Кака пися";
            var buttons = MessageBoxButtons.OK;

            var result = MessageBox.Show(this, message, caption, buttons);

            if (result == DialogResult.OK)
                Close();
        }
    }
}