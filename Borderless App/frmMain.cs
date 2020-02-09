using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Borderless_App
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            try
            {
                // Set window icon to app icon
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
            }
            catch { }
        }

        private void RefreshProcessList()
        {
            // Populate UI list with window titles
            Dictionary<string, IntPtr> titles = Win32.GetWindowTitles(chkChildWin.Checked);
            if (titles.Count != 0)
            {
                lstProcesses.DataSource = new BindingSource(titles, null);
                lstProcesses.DisplayMember = "Key";
                lstProcesses.ValueMember = "Value";
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void BtnRefreshList_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void BtnPatch_Click(object sender, EventArgs e)
        {
            // Check the user has selected something from the process list
            if (lstProcesses.SelectedItem == null)
            {
                MessageBox.Show("Select a process to patch!", "Borderless App",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Retrieve selected data
            KeyValuePair<string, IntPtr> selected = (KeyValuePair<string, IntPtr>)lstProcesses.SelectedItem;

            // Cannot make borderless fullscreen if window is fullscreen
            if (Win32.IsFullscreen(selected.Value))
            {
                MessageBox.Show("Cannot patch fullscreen processes!", "Borderless App",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Restore window if minimised
            if (Win32.IsMinimized(selected.Value))
            {
                Win32.ShowSelectedWindow(selected.Value);
            }

            // Set window to borderless fullscreen and focus it
            if (Win32.CheckValidHandle(selected, chkChildWin.Checked))
            {
                Win32.SetWindowBorderless(selected.Value, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, 0, 0);
                Win32.SetForegroundWindow(selected.Value);
            }
            else
            {
                MessageBox.Show("Invalid handle!\nPlease try again.",
                    "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                RefreshProcessList();
                return;
            }

            // Close application if set
            if (chkClose.Checked)
            {
                Application.Exit();
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }
    }
}
