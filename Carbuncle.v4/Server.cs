using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Carbuncle.v4;

public class Server
{
	internal static IList<Server> serverList = new List<Server>();

	internal const byte _PLAYERS = 0;

	internal const byte _SIZE = 1;

	internal const byte _NAME = 2;

	internal const byte _DIFFICULTY = 3;

	internal const byte _SEED = 4;

	internal const byte _PASS = 5;

	internal const byte _HOST = 6;

	internal const byte _HEADER = 7;

	internal const byte _PORT = 8;

	internal string[] data;

	internal bool active;

	internal int index;

	internal static System.Windows.Controls.ListBox List => default;//MainWindow.Instance.listbox_servers;

	public static Server NewListing(string[] data)
	{
		Server server = new Server
		{
			active = true,
			data = data
		};
		server.data[0] = server.data[0].Replace("CLEAR", "").Replace("LIST", "").Replace(" ", "");
		if (serverList.Count((Server t) => t != null && t.data[2] == data[2] && t.data[8] == data[8]) == 0)
		{
			serverList.Add(server);
			server.index = serverList.IndexOf(server);
		}
		return server;
	}

	public static Server GetServer(string name, string port)
	{
		for (int i = 0; i < serverList.Count; i++)
		{
			if (serverList[i].data[2] == name && serverList[i].data[8] == port)
			{
				return serverList[i];
			}
		}
		return null;
	}

	public static Server GetServer(string value)
	{
		return serverList.First((Server t) => t.ToString() == value);
	}

	public override string ToString()
	{
		if (data.Length < 8)
		{
			return "";
		}
		object[] args = new string[7]
		{
			"127.0.0.1",
			data[8],
			data[2],
			data[0],
			data[1],
			data[3],
			data[7]
		};
		return string.Format("IP: {0}, Port: {1}, Name: {2}, Players: {3}, Size: {4}, Difficulty: {5}, Type: {6}", args);
	}

	public static void AddListing(Server server)
	{
		List.Dispatcher.Invoke(delegate
		{
			if (!List.Items.Contains(server.ToString()) && !string.IsNullOrWhiteSpace(server.ToString()))
			{
				List.Items.Add(server.ToString());
			}
		});
	}

	public static void RemoveListing(Server server)
	{
		serverList.Remove(server);
		List.Dispatcher.Invoke(delegate
		{
			List.Items.Remove(server.ToString());
		});
	}
}
