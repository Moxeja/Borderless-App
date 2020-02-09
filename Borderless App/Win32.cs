using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Borderless_App
{
    public static class Win32
    {
        private delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

        // Pinvoke imports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint SendMessageTimeoutText(IntPtr hWnd, int Msg, int countOfChars,
            StringBuilder text, SendMessageTimeoutFlags flags, uint uTImeoutj, out IntPtr result);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, Int64 dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        // Title storage
        private static readonly Dictionary<string, IntPtr> windowTitles = new Dictionary<string, IntPtr>();

        // Windows constants
        private const int WM_GETTEXT = 0xD;
        private const int WM_GETTEXTLENGTH = 0xE;

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

        public static Dictionary<string, IntPtr> GetWindowTitles(bool includeChildren)
        {
            Win32.windowTitles.Clear();
            EnumWindows(Win32.EnumWindowsCallback, includeChildren ? (IntPtr)1 : IntPtr.Zero);
            return Win32.windowTitles;
        }

        private static bool EnumWindowsCallback(IntPtr testWindowHandle, IntPtr includeChildren)
        {
            string title = Win32.GetWindowTitle(testWindowHandle);
            if (!String.IsNullOrEmpty(title) && IsWindowVisible(testWindowHandle) &&
                !Win32.windowTitles.ContainsKey(title) && !title.Equals("Borderless App"))
            {
                Win32.windowTitles.Add(title, testWindowHandle);
            }

            if (includeChildren.Equals(IntPtr.Zero) == false)
            {
                Win32.EnumChildWindows(testWindowHandle, Win32.EnumWindowsCallback, IntPtr.Zero);
            }
            return true;
        }

        private static string GetWindowTitle(IntPtr windowHandle)
        {
            // Get window title text length
            SendMessageTimeoutText(windowHandle, WM_GETTEXTLENGTH, 0, null,
                SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000u, out IntPtr result);

            // Create buffer and read window title into it
            StringBuilder sb = new StringBuilder(result.ToInt32() + 1);
            SendMessageTimeoutText(windowHandle, WM_GETTEXT, sb.Capacity, sb,
                SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000u, out _);

            return sb.ToString();
        }

        public static void SetWindowBorderless(IntPtr hWnd, int width, int height, int posX, int posY)
        {
            SetWindowLongPtr(hWnd, GWL_STYLE, WS_VISIBLE | WS_POPUP | WS_CLIPSIBLINGS);
            SetWindowPos(hWnd, HWND_TOP, posX, posY, width, height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
        }

        public static bool IsMinimized(IntPtr hWnd)
        {
            long wndStyle = GetWindowLongPtr(hWnd, GWL_STYLE).ToInt64();
            if (wndStyle == 0)
                return false;

            return (wndStyle & WS_MINIMIZE) != 0;
        }

        public static bool IsFullscreen(IntPtr hWnd)
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

        public static void ShowSelectedWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_RESTORE);
        }

        public static bool CheckValidHandle(KeyValuePair<string, IntPtr> data, bool childWind)
        {
            Dictionary<string, IntPtr> titles = Win32.GetWindowTitles(childWind);

            // Check if window still exists
            if (!titles.ContainsKey(data.Key))
                return false;

            // Check if handle has been reused
            if (titles[data.Key] != data.Value)
                return false;

            return true;
        }
    }
}
