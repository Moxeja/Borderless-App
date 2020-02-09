using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Borderless_App
{
	public static class Win32
	{
		private delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[Flags]
		enum SendMessageTimeoutFlags : uint
		{
			SMTO_NORMAL = 0x0,
			SMTO_BLOCK = 0x1,
			SMTO_ABORTIFHUNG = 0x2,
			SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
			SMTO_ERRORONEXIT = 0x20
		}

		[DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern uint SendMessageTimeoutText(IntPtr hWnd, int Msg, int countOfChars,
			StringBuilder text, SendMessageTimeoutFlags flags, uint uTImeoutj, out IntPtr result);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);

		private static readonly Dictionary<string, IntPtr> windowTitles = new Dictionary<string, IntPtr>();
		private const int WM_GETTEXT = 0xD;
		private const int WM_GETTEXTLENGTH = 0xE;

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
	}
}
