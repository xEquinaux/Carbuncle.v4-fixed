using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;			
using Brush = System.Windows.Media.Brush;
using Image = System.Windows.Controls.Image;

namespace Carbuncle.v4;

public partial class MainWindow : Window, IComponentConnector
{
	public static MainWindow Instance;

	internal const byte _PLAYERS = 0;

	internal const byte _SIZE = 1;

	internal const byte _NAME = 2;

	internal const byte _DIFFICULTY = 3;

	internal const byte _SEED = 4;

	internal const byte _PASS = 5;

	internal const byte _PORT = 6;

	//internal static Terminal terminal_window;

	public string Username { get; set; }

	public string UserColor { get; set; }

	public string Message { get; set; }

	public System.Windows.Controls.Image[] Badge { get; private set; }

	public MainWindow()
	{
		Instance = this;
		InitializeComponent();
	}

	private void button_host_Click(object sender, RoutedEventArgs e)
	{
		if (HostPanel.hostpanel_window == null)
		{
			HostPanel.hostpanel_window = new HostPanel();
			HostPanel.hostpanel_window.Show();
		}
	}

	private void button_terminal_Click(object sender, RoutedEventArgs e)
	{
		//if (terminal_window == null)
		//{
		//	terminal_window = new Terminal();
		//	terminal_window.Show();
		//}
	}

	public void DisplayMessage(string Username, string UserColor, string Message, bool emote = false)
	{
		Instance.Dispatcher.Invoke(delegate
		{
			Bold item = new Bold(new Run(Username))
			{
				Foreground = (Brush)new BrushConverter().ConvertFromString(UserColor)
			};
			if (Badge != null)
			{
				Image[] badge = Badge;
				foreach (Image uiElement in badge)
				{
					text_chat.Inlines.Add(uiElement);
					text_chat.Inlines.Add(" ");
				}
			}
			if (emote)
			{
				Bold item2 = new Bold(new Run(string.Format(" {0}{1}", new object[2] { Message, "\n" })))
				{
					Foreground = (Brush)new BrushConverter().ConvertFromString(UserColor)
				};
				text_chat.Inlines.Add(item);
				text_chat.Inlines.Add(item2);
			}
			else
			{
				text_chat.Inlines.Add(item);
				text_chat.Inlines.Add(string.Format("{0} {1}{2}", new object[3] { ":", Message, "\n" }));
			}
			richtextbox.ScrollToEnd();
		});
	}

	private void On_ClickSend(object sender, RoutedEventArgs e)
	{
		HostPanel.LocalUser.SendNetworkMessage(textbox_message.Text);
		textbox_message.Clear();
	}

	private void textbox_message_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (!e.Handled && e.Key == Key.Return && !e.IsRepeat && textbox_message.Text.Length != 0)
		{
			HostPanel.LocalUser.SendNetworkMessage(textbox_message.Text);
			textbox_message.Clear();
		}
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		App.Closing = true;
		HostPanel.LocalUser?.client?.Close();
		ChatUser.Interrupt = true;
		System.Windows.Application.Current.Shutdown();
	}

	private void image_gotFocus(object sender, RoutedEventArgs e)
	{
	}
}
