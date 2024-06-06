using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace Carbuncle.v4;

public partial class HostPanel : Window, IComponentConnector
{
	private int valid0;

	private int valid1;

	private int valid2;

	internal static ChatUser LocalUser;

	internal static HostPanel hostpanel_window;

	private int playerCount => ToInt(ui_textbox_maxplayers.Text);

	private string difficulty => ui_text_box_difficulty.Text;

	private string password => ui_text_passbox_serverpassword.Password;

	private string mapSelect => Path.GetFileNameWithoutExtension(WorldFile);

	public static string WorldFile => hostpanel_window.s_textbox_upload.Text;

	public static string tModFile => hostpanel_window.m_textbox_upload.Text;

	internal static string IP => MainUI.Current.IP;

	public HostPanel()
	{
		hostpanel_window = this;
		InitializeComponent();
		ui_progress_server.Maximum = 1.0;
	}

	private int ToInt(string text)
	{
		int.TryParse(text, out var result);
		return result;
	}

	private string Replace(string text)
	{
		return Regex.Replace(text, "[^\\w]", "");
	}

	private bool HasSpecialChars(string[] text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (Regex.Match(text[i], "[^\\w]").Success)
			{
				return true;
			}
		}
		return false;
	}

	private string[] DataPacket(string header)
	{
		return new string[9] { ui_textbox_maxplayers.Text, "", mapSelect, difficulty, "", password, LocalUser.Name, header, "" };
	}

	private void Host_Click(object sender, RoutedEventArgs e)
	{
		if (ui_progress_server.Value < 1.0 || HasSpecialChars(new string[3] { ui_textbox_maxplayers.Text, ui_text_box_difficulty.Text, ui_text_passbox_serverpassword.Password }) || playerCount < 8 || playerCount > 256)
		{
			return;
		}
		TcpClient tcpClient = new TcpClient();
		tcpClient.SendTimeout = (int)TimeSpan.FromSeconds(10.0).TotalMilliseconds;
		tcpClient.Connect(IPAddress.Parse(IP), 1040);
		if (!tcpClient.Connected)
		{
			return;
		}
		NetworkStream stream = tcpClient.GetStream();
		string header = ((tModFile != string.Empty) ? "tModLoader" : "tShock");
		string[] array = DataPacket(header);
		object[] data = array;
		byte[] buffer = NetHelper.GetBuffer(',', data);
		stream.Write(buffer, 0, buffer.Length);
		bool flag = tModFile != string.Empty;
		TcpClient tcpClient2 = new TcpClient();
		TcpClient tcpClient3 = new TcpClient();
		tcpClient2.SendTimeout = (int)TimeSpan.FromSeconds(10.0).TotalMilliseconds;
		tcpClient3.SendTimeout = (int)TimeSpan.FromSeconds(20.0).TotalMilliseconds;
		if (!Timeout(tcpClient2, IPAddress.Parse(IP), 1050) && (!flag || !Timeout(tcpClient3, IPAddress.Parse(IP), 1080)))
		{
			WldFileUpload(tcpClient2, header);
			if (flag)
			{
				tModFileUpload(tcpClient3);
			}
		}
	}

	private void ui_listbox_mapselect_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		valid2 = 1;
		ui_progress_server.Value = (float)(valid0 + valid1 + valid2) / 3f;
	}

	private void ui_textbox_maxplayers_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (ui_textbox_maxplayers.Text.Length > 0)
		{
			valid0 = 1;
		}
		else
		{
			valid0 = 0;
		}
		ui_progress_server.Value = (float)(valid0 + valid1 + valid2) / 3f;
	}

	private void WldFileUpload(TcpClient client, string header = "tModLoader")
	{
		int num = -1;
		while (++num < 3 && !client.Connected)
		{
			try
			{
				client.Connect(IPAddress.Parse(IP), 1050);
			}
			catch
			{
				Thread.Sleep(1000);
			}
		}
		num = -1;
		while (!client.Connected)
		{
			if (++num == 3)
			{
				return;
			}
			Thread.Sleep(1000);
		}
		NetworkStream stream = client.GetStream();
		if (WorldFile != null && WorldFile != "" && (WorldFile.EndsWith(".wld") || WorldFile.EndsWith(".zip")))
		{
			byte message = 0;
			if (header == "tShock")
			{
				message = 4;
			}
			else if (header == "tModLoader")
			{
				message = 4;
			}
			byte[] array = new byte[81920];
			using FileStream fileStream = new FileStream(WorldFile, FileMode.Open, FileAccess.Read);
			while (fileStream.Position < fileStream.Length)
			{
				int num2 = fileStream.Read(array, 0, array.Length);
				NetHandler.NetSend(message, stream, "", num2, array);
				ui_progress_server.Value = fileStream.Position / fileStream.Length;
				ui_progress_server.UpdateLayout();
			}
		}
		client.Close();
		stream.Dispose();
	}

	private void button_upload_Click(object sender, RoutedEventArgs e)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Filter = "World File Archive (*.zip)|*.zip";
		openFileDialog.DefaultExt = ".zip";
		openFileDialog.ShowDialog(this);
		openFileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + "Documents/My Games/Terraria";
		s_textbox_upload.Text = openFileDialog.FileName;
	}

	private void Button_Click_1(object sender, RoutedEventArgs e)
	{
		valid2 = 0;
		s_textbox_upload.Clear();
	}

	private void s_textbox_upload_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (s_textbox_upload.Text.Length > 0)
		{
			valid2 = 1;
		}
		ui_progress_server.Value = (float)(valid0 + valid1 + valid2) / 3f;
	}

	private void tModFileUpload(TcpClient client, string header = "tModLoader")
	{
		int num = -1;
		while (++num < 3 && !client.Connected)
		{
			try
			{
				client.Connect(IPAddress.Parse(IP), 1080);
			}
			catch
			{
				Thread.Sleep(1000);
			}
		}
		num = -1;
		while (!client.Connected)
		{
			if (++num == 3)
			{
				return;
			}
			Thread.Sleep(1000);
		}
		NetworkStream stream = client.GetStream();
		bool flag = tModFile.EndsWith(".zip");
		bool flag2 = tModFile.EndsWith(".tmod");
		if (tModFile != null && tModFile != "" && (flag || flag2))
		{
			byte message = 0;
			if (header == "tShock")
			{
				return;
			}
			if (header == "tModLoader")
			{
				message = (byte)(flag ? 10 : 2);
			}
			byte[] array = new byte[81920];
			using FileStream fileStream = new FileStream(tModFile, FileMode.Open, FileAccess.Read);
			while (fileStream.Position < fileStream.Length)
			{
				int num2 = fileStream.Read(array, 0, array.Length);
				NetHandler.NetSend(message, stream, "", num2, array);
				ui_progress_server.Value = fileStream.Position / fileStream.Length;
				ui_progress_server.UpdateLayout();
			}
		}
		client.Close();
		stream.Dispose();
	}

	private void button_upload_Click2(object sender, RoutedEventArgs e)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Filter = "Mod Files Archive (*.zip)|*.zip";
		openFileDialog.DefaultExt = ".zip";
		openFileDialog.ShowDialog(this);
		openFileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + "Documents/My Games/Terraria";
		m_textbox_upload.Text = openFileDialog.FileName;
	}

	private void Button_Click_2(object sender, RoutedEventArgs e)
	{
		m_textbox_upload.Clear();
	}

	private void Window_init(object sender, EventArgs e)
	{
	}

	private void ui_text_box_difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		valid1 = 1;
		ui_progress_server.Value = (float)(valid0 + valid1 + valid2) / 3f;
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		ui_textbox_maxplayers.Clear();
		ui_text_box_difficulty.SelectedIndex = -1;
		ui_text_passbox_serverpassword.Clear();
		valid0 = (valid1 = (valid2 = 0));
		hostpanel_window = null;
	}

	private bool Timeout(NetworkStream stream, int max = 15)
	{
		int num = 0;
		while (!stream.DataAvailable)
		{
			Thread.Sleep(1000);
			if (++num == max)
			{
				return true;
			}
		}
		return false;
	}

	private bool Timeout(TcpClient tcp, IPAddress ip, int port, int max = 15)
	{
		int num = 0;
		while (!tcp.Connected)
		{
			Thread.Sleep(1000);
			if (num++ == max)
			{
				return true;
			}
			try
			{
				tcp.Connect(ip, port);
			}
			catch
			{
			}
		}
		return false;
	}
}
