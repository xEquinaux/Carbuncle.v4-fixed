using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Carbuncle.v4;

public class App : System.Windows.Application
{
	internal static bool Closing;

	public App()
	{
		base.DispatcherUnhandledException += App_DispatcherUnhandledException;
	}

	private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		if (!Closing)
		{
			System.Windows.MessageBox.Show("Some error that was unresolved, idk.");
		}
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
	public void InitializeComponent()
	{
		base.StartupUri = new Uri("../Login.xaml", UriKind.Relative);
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		app.Run();
	}
}
