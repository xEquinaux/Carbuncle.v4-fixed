using System.Windows.Threading;
using CirclePrefect.Foundation;

namespace Carbuncle.v4;

public sealed class Surface
{
	public static void HandleCommands(string command)
	{
		HostPanel.LocalUser.SendTerminalMessage(command, disable_Exit: false);
	}

	public static Game CreateGraphic(int width, int height, Dispatcher dispatcher)
	{
		Game game = new Game(dispatcher);
		game.Run(game.Width = width, game.Height = height);
		return game;
	}
}
