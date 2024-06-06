using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Carbuncle.v4;

public class LocalServer
{
	public static LocalServer Instance;

	public static IList<LocalServer> Servers = new List<LocalServer>();

	public IPAddress IP;

	public int Port { get; private set; }

	public string Name { get; private set; }

	public string Address
	{
		get
		{
			return IP.ToString();
		}
		set
		{
			IPAddress.TryParse(value, out IP);
		}
	}

	public int Index { get; set; }

	public LocalServer()
	{
		Instance = this;
	}

	public static Process[] GetRunningServers()
	{
		return Process.GetProcessesByName("TerrariaServer");
	}

	public static string GetServerTitle(Process process)
	{
		return process.MainWindowTitle;
	}
}
