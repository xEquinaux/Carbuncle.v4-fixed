using System.Collections.Generic;
using System.Linq;

namespace Carbuncle.v4;

public class Lobby
{
	public static IList<Lobby> lobby = new List<Lobby>();

	public string IP;

	public string name;

	public override string ToString()
	{
		return $"{name} {IP}";
	}

	public static Lobby GetLobby(string name, string ip)
	{
		return lobby.FirstOrDefault((Lobby t) => t.ToString() == $"{name} {ip}");
	}

	public static Lobby GetLobby(string value)
	{
		return lobby.FirstOrDefault((Lobby t) => t.ToString() == value);
	}

	public static Lobby GetLobbyByName(string name)
	{
		return lobby.FirstOrDefault((Lobby t) => t.name == name);
	}
}
