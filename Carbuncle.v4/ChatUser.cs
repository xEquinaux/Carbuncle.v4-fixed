using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Carbuncle.v4;

public class ChatUser
{
	public TcpClient client;

	private NetworkStream stream;

	public StreamReader read;

	public StreamWriter write;

	public Thread userThread;

	private const byte NAME = 1;

	private const byte COLOR = 2;

	private const byte FIRST = 3;

	public static bool Interrupt;

	private const string CMD_Symbol = "/";

	public string Name { get; set; }

	public string Color { get; set; }

	public string uID => "#" + (Name + Color).GetHashCode().ToString().Substring(0, 4);

	public string IP { get; private set; }

	public void SendNetworkMessage(string message)
	{
		if (Regex.IsMatch(message, "[^\\w\\.!@#?&*;%$' ]", RegexOptions.None, TimeSpan.FromSeconds(0.25)))
		{
			return;
		}
		try
		{
			byte[] array = null;
			string s = $"MESSAGE {Name} {Color} {message}";
			array = Encoding.ASCII.GetBytes(s);
			stream.Write(array, 0, array.Length);
		}
		catch
		{
		}
	}

	public void SendTerminalMessage(string message, bool disable_Exit = true)
	{
		if (Regex.IsMatch(message, "[^\\w\\.!@#?&*;%$' ]", RegexOptions.None, TimeSpan.FromSeconds(0.25)) || (disable_Exit && message.ToLower().Trim(' ') == "exit"))
		{
			return;
		}
		try
		{
			byte[] array = null;
			string s = "TERMINAL " + message;
			array = Encoding.ASCII.GetBytes(s);
			stream.Write(array, 0, array.Length);
		}
		catch
		{
		}
	}

	public Thread NetworkLoop()
	{
		return new Thread((ThreadStart)delegate
		{
			while (!Interrupt)
			{
				try
				{
					while (true)
					{
						NetworkStream networkStream = stream;
						if (networkStream == null || networkStream.DataAvailable)
						{
							break;
						}
						Thread.Sleep(50);
					}
				}
				catch (Exception)
				{
					break;
				}
				string buffer = NetHelper.StringFromStream(stream);
				string text = null;
				if (buffer.StartsWith("MESSAGE"))
				{
					text = NetHelper.ExtractMessage(buffer, ' ', out var split);
					MainWindow.Instance.DisplayMessage(split[1], split[2], text);
				}
				if (buffer.StartsWith("EMOTE"))
				{
					text = NetHelper.ExtractMessage(buffer, ' ', out var split2);
					MainWindow.Instance.DisplayMessage(split2[1], split2[2], text, emote: true);
				}
				if (buffer.Contains("CLEAR"))
				{
					//MainWindow.Instance.listbox_servers.Dispatcher.Invoke(delegate
					//{
					//	MainWindow.Instance.listbox_servers.Items.Clear();
					//	Server.serverList.Clear();
					//});
				}
				if (buffer.Contains("JOIN"))
				{
					System.Windows.Controls.ListBox list = MainWindow.Instance.list_users;
					string[] array = buffer.Split(';');
					for (int i = 0; i < Math.Max(array.Length - 1, 1); i++)
					{
						if (array[i].Contains(" "))
						{
							string name = array[i].Split(' ')[1];
							if (!list.Items.Contains(name))
							{
								list.Dispatcher.Invoke(delegate
								{
									if (!Regex.IsMatch(name, "[^\\w]"))
									{
										list.Items.Add(name);
									}
								});
							}
						}
					}
				}
				if (buffer.Contains("LEAVE"))
				{
					System.Windows.Controls.ListBox list = MainWindow.Instance.list_users;
					string[] array2 = buffer.Split(';');
					for (int j = 0; j < Math.Max(array2.Length - 1, 1); j++)
					{
						if (array2[j].Contains("LEAVE"))
						{
							string name = array2[j].Split(' ')[1];
							if (list.Items.Contains(name))
							{
								list.Dispatcher.Invoke(delegate
								{
									list.Items.Remove(name);
								});
							}
						}
					}
				}
				if (buffer.Contains("LIST"))
				{
					//MainWindow.Instance.listbox_servers.Dispatcher.Invoke(delegate
					//{
					//	if (buffer.Contains(";"))
					//	{
					//		string[] array3 = buffer.Split(';');
					//		for (int k = 0; k < array3.Length; k++)
					//		{
					//			if (array3[k].Contains(" ") && array3[k].Contains(","))
					//			{
					//				Server.AddListing(Server.NewListing(array3[k].Split(',')));
					//			}
					//		}
					//	}
					//});
				}
				if (buffer.Contains("MOD"))
				{
					MainWindow.Instance.Dispatcher.Invoke(delegate
					{
						MainWindow instance2 = MainWindow.Instance;
						instance2.list_users.Items.Clear();
						//instance2.listbox_servers.Items.Clear();
						System.Windows.MessageBox.Show("The session has ended due to being booted from the lobby by a moderator.", "Booted", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
						instance2.Close();
					});
				}
				if (buffer.Contains("QUIT"))
				{
					MainWindow.Instance.Dispatcher.Invoke(delegate
					{
						MainWindow instance = MainWindow.Instance;
						instance.list_users.Items.Clear();
						//instance.listbox_servers.Items.Clear();
						System.Windows.MessageBox.Show("The session has ended due to the connection not responding to the server.", "Connection Error", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
						instance.Close();
					});
				}
			}
		});
	}

	public bool RemoteRegister(string code, int port = 1010)
	{
		try
		{
			client = new TcpClient();
			client.Connect(IPAddress.Parse(MainUI.Current.IP), 1010);
		}
		catch (Exception ex)
		{
			System.Windows.MessageBox.Show("Remote server is currently down. " + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		write = new StreamWriter(client.GetStream());
		read = new StreamReader(client.GetStream());
		stream = client.GetStream();
		byte[] bytes = Encoding.ASCII.GetBytes($"LOGIN {Name} {code} {Color} {uID}");
		stream.Write(bytes, 0, bytes.Length);
		int num = -1;
		if (client.Connected)
		{
			return true;
		}
		return false;
	}
}
