using System.Windows;
using CirclePrefect;

namespace Carbuncle.v4;

public static class Extension
{
	public static Vector2 GetV2(this System.Windows.Point point)
	{
		return new Vector2((float)point.X, (float)point.Y);
	}
}
