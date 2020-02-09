using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Borderless_App
{
	public partial class frmMain : Form
	{
		// User-Defined Types
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		// Pinvoke Imports
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, Int64 dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		private static extern IntPtr SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern int SetForegroundWindow(IntPtr hwnd);

		// Window style constants
		private const int GWL_STYLE = -16;
		private const int GWL_EXSTYLE = -20;

		private const int HWND_TOP = 0;

		private const int SW_RESTORE = 9;
		private const uint SWP_SHOWWINDOW = 0x0040;
		private const uint SWP_FRAMECHANGED = 0x0020;

		private const long WS_BORDER = 0x00800000L;
		private const long WS_CAPTION = 0x00C00000L;
		private const long WS_VISIBLE = 0x10000000L;
		private const long WS_MINIMIZE = 0x20000000L;
		private const long WS_POPUP = 0x80000000L;
		private const long WS_CLIPSIBLINGS = 0x04000000L;
		private const long WS_EX_TOPMOST = 0x00000008L;

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

		private void SetWindowBorderless(IntPtr hWnd, int width, int height, int posX, int posY)
		{
			SetWindowLongPtr(hWnd, GWL_STYLE, WS_VISIBLE | WS_POPUP | WS_CLIPSIBLINGS);
			SetWindowPos(hWnd, HWND_TOP, posX, posY, width, height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
		}

		private bool IsMinimized(IntPtr hWnd)
		{
			long wndStyle = GetWindowLongPtr(hWnd, GWL_STYLE).ToInt64();
			if (wndStyle == 0)
				return false;

			return (wndStyle & WS_MINIMIZE) != 0;
		}

		private bool IsFullscreen(IntPtr hWnd)
		{
			long wndStyle = GetWindowLongPtr(hWnd, GWL_STYLE).ToInt64();
			long wndExStyle = GetWindowLongPtr(hWnd, GWL_EXSTYLE).ToInt64();
			if (wndStyle == 0 || wndExStyle == 0)
				return false;

			if ((wndExStyle & WS_EX_TOPMOST) == 0)
				return false;
			if ((wndStyle & WS_POPUP) != 0)
				return false;
			if ((wndStyle & WS_CAPTION) != 0)
				return false;
			if ((wndStyle & WS_BORDER) != 0)
				return false;

			return true;
		}

		private void FrmMain_Load(object sender, EventArgs e)
		{
			RefreshProcessList();
		}

		private void BtnRefreshList_Click(object sender, EventArgs e)
		{
			RefreshProcessList();
		}

		private bool CheckValidHandle(KeyValuePair<string, IntPtr> data)
		{
			Dictionary<string, IntPtr> titles = Win32.GetWindowTitles(chkChildWin.Checked);
			
			// Check if window still exists
			if (!titles.ContainsKey(data.Key))
				return false;

			// Check if handle has been reused
			if (titles[data.Key] != data.Value)
				return false;

			return true;
		}

		private void BtnPatch_Click(object sender, EventArgs e)
		{
			// Check the user has selected something from the process list
			if (lstProcesses.SelectedItem == null)
			{
				MessageBox.Show("Select a process to patch!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			// Retrieve selected data
			KeyValuePair<string, IntPtr> selected = (KeyValuePair<string, IntPtr>)lstProcesses.SelectedItem;

			// Cannot make borderless fullscreen if window is fullscreen
			if (IsFullscreen(selected.Value))
			{
				MessageBox.Show("Cannot patch fullscreen processes!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			// Restore window if minimised
			if (IsMinimized(selected.Value))
			{
				ShowWindow(selected.Value, SW_RESTORE);
			}

			// Set window to borderless fullscreen and focus it
			if (CheckValidHandle(selected))
			{
				SetWindowBorderless(selected.Value, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, 0, 0);
				SetForegroundWindow(selected.Value);
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
