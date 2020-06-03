using System;
using src.ApplicationLayer;
using System.Windows.Forms;
using src.SessionLayer;
namespace Project_Gameserver
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameGUI());
        }
    }
}
