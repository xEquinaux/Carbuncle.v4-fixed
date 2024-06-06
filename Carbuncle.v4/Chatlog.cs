using System.Collections.Generic;

namespace Carbuncle.v4;

public class Chatlog
{
	public static Chatlog Instance;

	public List<string> Log = new List<string>();

	public Chatlog()
	{
		Instance = this;
	}

	public static void NewMessage(string message)
	{
		Instance.Log.Add(message);
	}
}
