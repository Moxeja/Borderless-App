using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private struct WindowStats
        {
            public string processName;      // Process name
            public long windowStyles;       // Window style
            public RECT windowRect;         // Window rectangle
        }


        // Pinvoke Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(IntPtr alwaysZero, string lpWindowName);

        // Window style constants
        private const int GWL_STYLE     = -16;
        private const int GWL_EXSTYLE   = -20;

        private const int HWND_TOP          = 0;
        private const int HWND_NOTOPMOST    = -2;

        private const int  SW_RESTORE       = 9;
        private const uint SWP_SHOWWINDOW   = 0x0040;
        private const uint SWP_FRAMECHANGED = 0x0020;

        private const long WS_BORDER        = 0x00800000L;
        private const long WS_CAPTION       = 0x00C00000L;
        private const long WS_VISIBLE       = 0x10000000L;
        private const long WS_MINIMIZE      = 0x20000000L;
        private const long WS_POPUP         = 0x80000000L;
        private const long WS_CLIPSIBLINGS  = 0x04000000L;
        private const long WS_EX_TOPMOST    = 0x00000008L;

        // Constants
        private readonly char[] splitter = { ' ', '[' };

        // Global Variables
        private List<WindowStats> windowStats = new List<WindowStats>();
        private readonly List<string> igrnoreList = new List<string>()
        { "ApplicationFrameHost", "SystemSettings", "WinStore.App" };

        public frmMain()
        {
            InitializeComponent();

            try
            {
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
            } catch { }
        }

        private void RefreshProcessList()
        {
            lstProcesses.Items.Clear();

            Process[] processes = Process.GetProcesses();
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            foreach (Process process in processes)
            {
                try
                {
                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero && process.MainWindowTitle != "" && process.ProcessName != currentProcessName
                        && !igrnoreList.Contains(process.ProcessName))
                    {
                        lstProcesses.Items.Add($"{process.ProcessName} [{process.MainWindowTitle}]");
                    }
                } catch { continue; }
            }

            lstProcesses.Sorted = true;
        }

        private void SetWindowBorderless(IntPtr hWnd, int width, int height, int posX, int posY)
        {
            SetWindowLongPtr(hWnd, GWL_STYLE, WS_VISIBLE | WS_POPUP | WS_CLIPSIBLINGS);
            SetWindowPos(hWnd, HWND_TOP, posX, posY, width, height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
        }

        private void SetWindowPrevious(IntPtr hWnd, RECT previousState, long flags)
        {
            int posX = previousState.Left;
            int posY = previousState.Top;
            int width = previousState.Right - previousState.Left;
            int height = previousState.Bottom - previousState.Top;

            SetWindowLongPtr(hWnd, GWL_STYLE, flags);
            SetWindowPos(hWnd, HWND_NOTOPMOST, posX, posY, width, height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
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

        private bool IsBorderless(IntPtr hWnd)
        {
            long wndStyle = GetWindowLongPtr(hWnd, GWL_STYLE).ToInt64();
            if (wndStyle == 0)
                return false;

            if ((wndStyle & WS_POPUP) == 0)
                return false;
            if ((wndStyle & WS_CAPTION) != 0)
                return false;
            if ((wndStyle & WS_BORDER) != 0)
                return false;

            return true;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            windowStats.Clear();
            RefreshProcessList();
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            // Check the user has selected something from the process list
            if (lstProcesses.SelectedItem == null)
            {
                MessageBox.Show("Select a process to patch!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Check the process exists
            Process[] process = Process.GetProcessesByName(lstProcesses.SelectedItem.ToString().Split(splitter)[0]);
            if (process.Length == 0)
            {
                MessageBox.Show("No process found, please refresh process list!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Check there is at least one process with a valid window name
            int processWithWindowIndex = -1;
            for (int i = 0; i < process.Length; i++)
            {
                if (!String.IsNullOrEmpty(process[i].MainWindowTitle) && processWithWindowIndex == -1)
                {
                    processWithWindowIndex = i;
                }
            }

            if (processWithWindowIndex == -1)
            {
                MessageBox.Show("Could not find a process with a valid window name!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Check it is possible to obtain window handle
            process[processWithWindowIndex].Refresh();
            IntPtr windowHandle = FindWindow(IntPtr.Zero, process[processWithWindowIndex].MainWindowTitle);
            if (windowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Could not obtain window handle!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Cannot make borderless fullscreen if window is fullscreen
            if (IsFullscreen(windowHandle))
            {
                MessageBox.Show("Cannot patch fullscreen processes!", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // If window is already borderless fullscreen, attempt to restore it to regular windowed mode
            if (IsBorderless(windowHandle))
            {
                for (int i = 0; i < windowStats.Count; i++)
                {
                    if (process[processWithWindowIndex].ProcessName.Equals(windowStats[i].processName))
                    {
                        SetWindowPrevious(windowHandle, windowStats[i].windowRect, windowStats[i].windowStyles);
                        windowStats.RemoveAt(i);
                        return;
                    }
                }

                MessageBox.Show("Window is already borderless and cannot find previous window state to restore it.", 
                    "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Restore window if minimised
            if (IsMinimized(windowHandle))
            {
                ShowWindow(windowHandle, SW_RESTORE);
            }

            // Store window information if close on patch is NOT checked
            if (!chkClose.Checked)
            {
                bool result = GetWindowRect(windowHandle, out RECT lpRect);
                if (!result)
                {
                    MessageBox.Show("There was a problem retrieving the window dimensions.", "Borderless App", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Add window information to list
                windowStats.Add(new WindowStats() { processName = process[processWithWindowIndex].ProcessName,
                    windowStyles = GetWindowLongPtr(windowHandle, GWL_STYLE).ToInt64(), windowRect = lpRect });
            }

            // Set window to borderless fullscreen and focus it
            SetWindowBorderless(windowHandle, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, 0, 0);
            SetForegroundWindow(windowHandle);

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
