using System.Windows.Input;

namespace Carbuncle.v4;

public sealed class KeyChar
{
	public static string Convert(Key key)
	{
		switch (key)
		{
		case Key.A:
		case Key.B:
		case Key.C:
		case Key.D:
		case Key.E:
		case Key.F:
		case Key.G:
		case Key.H:
		case Key.I:
		case Key.J:
		case Key.K:
		case Key.L:
		case Key.M:
		case Key.N:
		case Key.O:
		case Key.P:
		case Key.Q:
		case Key.R:
		case Key.S:
		case Key.T:
		case Key.U:
		case Key.V:
		case Key.W:
		case Key.X:
		case Key.Y:
		case Key.Z:
			return key.ToString();
		case Key.D0:
		case Key.D1:
		case Key.D2:
		case Key.D3:
		case Key.D4:
		case Key.D5:
		case Key.D6:
		case Key.D7:
		case Key.D8:
		case Key.D9:
			return key.ToString().Substring(1, 1);
		case Key.Oem7:
			return "\"";
		case Key.Space:
			return " ";
		case Key.OemPeriod:
			return ".";
		case Key.Oem2:
			return "/";
		case Key.Oem102:
			return "\\";
		default:
			return string.Empty;
		}
	}
}
