using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Carbuncle.v4;

internal class Command
{
	public class Handlers
	{
		public class Keyboard
		{
			private static int WM_KEYDOWN = 256;

			private static int WM_KEYUP = 257;

			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool PostMessage(IntPtr hWnd, int Msg, Keys wParam, int lParam);

			[DllImport("user32.dll", SetLastError = true)]
			private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

			public static IntPtr FindWindow(string windowName)
			{
				Process[] processes = Process.GetProcesses();
				foreach (Process process in processes)
				{
					if (process.MainWindowHandle != IntPtr.Zero && process.MainWindowTitle.ToLower().Contains(windowName.ToLower()))
					{
						return process.MainWindowHandle;
					}
				}
				return IntPtr.Zero;
			}

			public static IntPtr FindWindow(IntPtr parent, string childClassName)
			{
				return FindWindowEx(parent, IntPtr.Zero, childClassName, string.Empty);
			}

			public static bool ForwardSlash(IntPtr hWnd)
			{
				PostMessage(hWnd, WM_KEYDOWN, Keys.Shift, 0);
				PostMessage(hWnd, WM_KEYDOWN, Keys.OemQuestion, 0);
				return false;
			}

			public static bool SendKey(IntPtr hWnd, Keys key)
			{
				return PostMessage(hWnd, WM_KEYDOWN, key, 0);
			}

			public static void SendKeys(IntPtr hWnd, Keys[] keys)
			{
				for (int i = 0; i < keys.Length; i++)
				{
					PostMessage(hWnd, WM_KEYDOWN, keys[i], 0);
				}
			}
		}

		public class Cursor
		{
			internal enum VirtualKeyStates
			{
				VK_LBUTTON = 1,
				VK_RBUTTON,
				VK_CANCEL,
				VK_MBUTTON
			}

			private const uint MOUSEEVENTF_ABSOLUTE = 32768u;

			private const uint MOUSEEVENTF_LEFTDOWN = 2u;

			private const uint MOUSEEVENTF_LEFTUP = 4u;

			private const uint MOUSEEVENTF_MIDDLEDOWN = 32u;

			private const uint MOUSEEVENTF_MIDDLEUP = 64u;

			private const uint MOUSEEVENTF_MOVE = 1u;

			private const uint MOUSEEVENTF_RIGHTDOWN = 8u;

			private const uint MOUSEEVENTF_RIGHTUP = 16u;

			private const uint MOUSEEVENTF_XDOWN = 128u;

			private const uint MOUSEEVENTF_XUP = 256u;

			private const uint MOUSEEVENTF_WHEEL = 2048u;

			private const uint MOUSEEVENTF_HWHEEL = 4096u;

			private const int KEY_PRESSED = 32768;

			[DllImport("user32.dll")]
			private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, uint dwExtraInfo);

			[DllImport("user32.dll")]
			internal static extern short GetKeyState(VirtualKeyStates nVirtKey);

			public static bool IsLeftPressed()
			{
				return Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LBUTTON) & 0x8000);
			}

			public static bool IsRightPressed()
			{
				return Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RBUTTON) & 0x8000);
			}

			private static void ZeroMouse(Point origin)
			{
				System.Windows.Forms.Cursor.Position = new Point(Math.Max(0, origin.X + 7), Math.Max(0, origin.Y));
			}

			public static void ClickDrag(Point origin, int x, int y, bool zero = true)
			{
				mouse_event(2u, 0, 0, 0u, 0u);
				RelativeMoveMouse(origin, x, y, zero);
			}

			public static void LeftMouseUp()
			{
				mouse_event(4u, 0, 0, 0u, 0u);
			}

			public static void LeftMouseDoubleClick()
			{
				mouse_event(2u, 0, 0, 0u, 0u);
				Thread.Sleep(100);
				mouse_event(4u, 0, 0, 0u, 0u);
				mouse_event(2u, 0, 0, 0u, 0u);
				Thread.Sleep(100);
				mouse_event(4u, 0, 0, 0u, 0u);
			}

			public static void RelativeMoveMouse(Point origin, int x, int y, bool zero = true)
			{
				if (zero)
				{
					ZeroMouse(origin);
				}
				Thread.Sleep(100);
				Point position = System.Windows.Forms.Cursor.Position;
				System.Windows.Forms.Cursor.Position = new Point(position.X + x, position.Y + y);
			}

			public static void LeftMouse()
			{
				mouse_event(2u, 0, 0, 0u, 0u);
				Thread.Sleep(100);
				mouse_event(4u, 0, 0, 0u, 0u);
			}

			public static void RightMouse()
			{
				mouse_event(8u, 0, 0, 0u, 0u);
				Thread.Sleep(100);
				mouse_event(16u, 0, 0, 0u, 0u);
			}

			public static void CenterMoveMouse(Point origin, int width, int height)
			{
				RelativeMoveMouse(origin, width / 2, height / 2);
			}
		}

		public class Pointers
		{
			public static string windowName = "";

			public static IntPtr hWnd = IntPtr.Zero;

			[DllImport("user32.dll")]
			public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

			[DllImport("user32.dll")]
			private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

			public static void SelectWindow(string program)
			{
				windowName = program;
				hWnd = Keyboard.FindWindow(windowName);
			}

			[DllImport("user32.dll")]
			public static extern bool GetWindowRect(IntPtr hwnd, out Shapes.Rect rectangle);

			[DllImport("user32.dll")]
			internal static extern IntPtr GetDC(IntPtr hWnd);

			[DllImport("user32.dll")]
			internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
		}
	}

	public class Shapes
	{
		public struct Rect
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public static Rect Empty => new Rect(0, 0, 0, 0);

			public Rect(int x, int y, int width, int height)
			{
				Left = x;
				Top = y;
				Right = width;
				Bottom = height;
			}

			public override string ToString()
			{
				return $" x: {Left}, y: {Top}, width: {Right - Left}, height: {Bottom - Top} ";
			}

			public static Rectangle ConvertTo(Rect input)
			{
				return new Rectangle(input.Left, input.Top, input.Right - input.Left, input.Top - input.Bottom);
			}

			public static bool operator ==(Rect a, Rect b)
			{
				if (a.Left == b.Left && a.Top == b.Top && a.Right == b.Right)
				{
					return a.Bottom == b.Bottom;
				}
				return false;
			}

			public static bool operator !=(Rect a, Rect b)
			{
				if (a.Left == b.Left && a.Top == b.Top && a.Right == b.Right)
				{
					return a.Bottom != b.Bottom;
				}
				return true;
			}

			public override bool Equals(object obj)
			{
				throw new NotImplementedException();
			}

			public override int GetHashCode()
			{
				throw new NotImplementedException();
			}
		}
	}
}
