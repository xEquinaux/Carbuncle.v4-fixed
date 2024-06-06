using System;
using System.IO;

namespace Carbuncle.v4;

public class Logger
{
	public enum Code
	{
		Warning,
		Caution,
		Error,
		Info
	}

	public static string Ext = ".log";

	public static void WriteLine(string text, bool silent = false, string fileNameNoExt = "ServerLog")
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(fileNameNoExt + Ext, append: true);
			streamWriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + " | " + DateTime.Now.ToShortDateString() + "] " + text);
		}
		catch (Exception ex)
		{
			if (!silent)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		finally
		{
			if (!silent)
			{
				Console.WriteLine(text);
			}
		}
	}

	public static void WriteLine(string text, Code code, bool silent = false, string fileNameNoExt = "ServerLog")
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(fileNameNoExt + Ext, append: true);
			streamWriter.WriteLine("[" + DateTime.Now.ToLongTimeString() + " | " + DateTime.Now.ToShortDateString() + "] [" + Enum.GetName(typeof(Code), code) + "] " + text);
		}
		catch (Exception ex)
		{
			if (!silent)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		finally
		{
			if (!silent)
			{
				Console.WriteLine("[" + Enum.GetName(typeof(Code), code) + "] " + text);
			}
		}
	}

	public static void Write(string text, bool silent = false, string fileNameNoExt = "ServerLog")
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(fileNameNoExt + Ext, append: true);
			streamWriter.Write(text);
		}
		catch (Exception ex)
		{
			if (!silent)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		finally
		{
			if (!silent)
			{
				Console.Write(text);
			}
		}
	}
}
