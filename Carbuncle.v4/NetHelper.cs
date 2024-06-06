using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Carbuncle.v4;

internal sealed class NetHelper
{
	public static string StringFromStream(NetworkStream stream)
	{
		string text = "";
		byte[] array = new byte[256];
		using (new MemoryStream())
		{
			int num = 0;
			while (stream.DataAvailable)
			{
				num += stream.Read(array, 0, array.Length);
			}
			return Encoding.ASCII.GetString(array, 0, Math.Min(num, 256));
		}
	}

	public static string ExtractMessage(string buffer, char separator, out string[] split)
	{
		int num = 0;
		string[] array = buffer.Split(separator);
		num = array[0].Length + array[1].Length + array[2].Length + 3;
		split = array;
		return buffer.Substring(num);
	}

	public static string ReadWithPeek(StreamReader read)
	{
		string text = "";
		while (read.Peek() != -1)
		{
			text += char.ConvertFromUtf32(read.Read());
		}
		return text;
	}

	public static MemoryStream GetMemoryStream(string path)
	{
		byte[] array = new byte[1024];
		MemoryStream memoryStream = new MemoryStream();
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		while (fileStream.Position < fileStream.Length)
		{
			int count = fileStream.Read(array, 0, array.Length);
			memoryStream.Write(array, 0, count);
		}
		return memoryStream;
	}

	public static byte[] GetBuffer(char separator, params object[] data)
	{
		string text = "";
		for (int i = 0; i < data.Length; i++)
		{
			text = text + data[i].ToString() + separator;
		}
		text = text.TrimEnd(separator);
		return Encoding.ASCII.GetBytes(text);
	}

	public static bool TimeOut(NetworkStream stream, TimeSpan time, int max)
	{
		int num = 0;
		while (!stream.DataAvailable && num++ < max)
		{
			Thread.Sleep(time);
		}
		return num < max;
	}
}
