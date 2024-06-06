namespace Carbuncle.Util;

internal struct Address
{
	public string Name;

	public int Port;

	public string IP;

	public Address(string name, string ip, int port)
	{
		Name = name;
		IP = ip;
		Port = port;
	}
}
