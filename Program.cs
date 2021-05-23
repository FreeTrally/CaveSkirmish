using System;
using System.Windows.Forms;

namespace CaveSkirmish
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new DungeonForm();
            Application.Run(form);
        }
    }
}