using System;
using System.Threading;
using System.Windows.Forms;

namespace Borderless_App
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (Mutex mutex = new Mutex(true, "BorderlessApp", out bool createdNew))
			{
				if (!createdNew)
				{
					MessageBox.Show("Another instance is already running!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					Environment.Exit(0);
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new frmMain());
			}
		}
	}
}
