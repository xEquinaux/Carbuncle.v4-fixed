using System.IO;
using System.Net.Sockets;

namespace Carbuncle.v4;

public class NetHandler
{
	public static void NetRecieve(byte message)
	{
	}

	public static void NetSend(byte message, NetworkStream stream, string text = "", int num = 0, params byte[] buffer)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		switch (message)
		{
		case 5:
			binaryWriter.Write(message);
			binaryWriter.Write(num);
			break;
		case 7:
			binaryWriter.Write(text);
			break;
		case 1:
		case 4:
			binaryWriter.Write(buffer);
			break;
		case 2:
		case 3:
		case 10:
			binaryWriter.Write(buffer);
			break;
		case 6:
			binaryWriter.Write(message);
			break;
		case 8:
		case 9:
			break;
		}
	}
}
