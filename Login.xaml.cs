using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Carbuncle.Util;

namespace Carbuncle.v4;

public partial class MainUI : Window, IComponentConnector
{
	internal static MainUI Instance;

	internal static Address Current;

	public static ChatUser LocalUser => HostPanel.LocalUser;

	public MainUI()
	{
		if (!File.Exists("ip_server.ini"))
		{
			using (File.CreateText("ip_server.ini"))
			{
			}
		}
		Instance = this;
		InitializeComponent();
		button_grab_Click(null, null);
		Current.IP = textbox_ipstart.Text;
	}

	private void box_colors_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (box_colors.SelectedIndex != -1)
		{
			label_usercolor.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(box_colors.SelectedItem.ToString().Substring(box_colors.SelectedItem.ToString().LastIndexOf(':') + 2));
		}
	}

	private void listbox_ipdirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		textbox_ipstart.IsEnabled = ((ListBoxItem)listbox_ipdirectory.SelectedItem).Content.ToString() == "Manual IP Entry";
		if (!textbox_ipstart.IsEnabled)
		{
			Lobby lobbyByName = Lobby.GetLobbyByName(((ListBoxItem)listbox_ipdirectory.SelectedItem).Content.ToString());
			textbox_ipstart.Text = lobbyByName.name;
			Current.Name = lobbyByName.name;
			Current.IP = lobbyByName.IP;
		}
		else
		{
			Current.IP = textbox_ipstart.Text;
		}
	}

	private void On_ClickLogin(object sender, KeyboardEventArgs e)
	{
		string text = textbox_username.Text;
		string password = textbox_password.Password;
		if (!Regex.IsMatch(text, "[^\\w]", RegexOptions.None, TimeSpan.FromSeconds(1.5)) && !Regex.IsMatch(password, "[^\\w]", RegexOptions.None, TimeSpan.FromSeconds(1.5)) && text.Length > 3 && password.Length >= 4 && box_colors.SelectedIndex != -1)
		{
			HostPanel.LocalUser = new ChatUser
			{
				Color = box_colors.SelectedItem.ToString().Substring(box_colors.SelectedItem.ToString().LastIndexOf(':') + 2),
				Name = text
			};
			if (LocalUser.RemoteRegister(password))
			{
				(LocalUser.userThread = LocalUser.NetworkLoop()).Start();
				MainWindow.Instance = new MainWindow();
				MainWindow.Instance.list_users.Items.Add(LocalUser.Name);
				Close();
				MainWindow.Instance.Show();
			}
		}
	}

	private void button_grab_Click(object sender, RoutedEventArgs e)
	{
		string text = "";
		using (StreamReader streamReader = new StreamReader("ip_server.ini"))
		{
			text = streamReader.ReadLine();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			text = text.TrimEnd('\n', '\r');
		}
		if (!IPAddress.TryParse(text, out var address))
		{
			return;
		}
		try
		{
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(address, 1030);
			NetworkStream stream = tcpClient.GetStream();
			byte[] bytes = Encoding.ASCII.GetBytes("GRAB");
			if (!tcpClient.Connected)
			{
				return;
			}
			stream.Write(bytes, 0, bytes.Length);
			int num = -1;
			bool flag = true;
			while (!stream.DataAvailable)
			{
				Thread.Sleep(1000);
				if (++num == 10)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			label_status.Content = "Status: Online";
			Lobby.lobby.Clear();
			string[] array = StringFromStream(stream).Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains(':'))
				{
					string[] array2 = array[i].Split(':');
					Lobby.lobby.Add(new Lobby
					{
						name = array2[0],
						IP = array2[1]
					});
					string text2 = array2[0];
					ListBoxItem listBoxItem = new ListBoxItem();
					listBoxItem.Content = text2;
					if (!listbox_ipdirectory.Items.Contains(text2))
					{
						listbox_ipdirectory.Items.Add(listBoxItem);
					}
				}
			}
		}
		catch (Exception ex)
		{
			System.Windows.MessageBox.Show("IP server is down or there is a connectivity problem. " + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}

	public string StringFromStream(NetworkStream stream)
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
			return Encoding.ASCII.GetString(array, 0, num);
		}
	}

	public Window ShowLogin()
	{
		MainUI mainUI = (Instance = new MainUI());
		mainUI.ShowDialog();
		return mainUI;
	}

	private void textbox_ipstart_TextChanged(object sender, TextChangedEventArgs e)
	{
		Current.IP = textbox_ipstart.Text;
	}
}
