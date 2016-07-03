using System;
using System.Windows.Forms;
using DeploymentUtil.Utils;
using InteractiveConsole;

namespace DesktopSwitch
{
  class AppController
  {
    public readonly static WindowManager WindowManager = new WindowManager();

    public static AppContext AppContext;
    public static KeyboardManager KeyboardManager;

    [STAThread]
    static void Main(string[] args)
    {
      ConsoleUi.InitConsole(); // allocate console before any windown created
      AddCommands();
      
      AddHotkeys(); // creates a native window

      AppContext = new AppContext();

      AddAppMenu();
      
      Application.Run(AppContext);

      AppContext.Dispose();

//      Application.EnableVisualStyles();
//      Application.SetCompatibleTextRenderingDefault(false);
//      Application.Run(new Form1());
    }

    private static void AddAppMenu()
    {
      AppContext.AddMenuItem("Desctops/One", () =>
      {
        WindowManager.SwitchToDesctop(0);
        AppContext.SetIcon(Resources.TrayIconNumber1);
      });
      AppContext.AddMenuItem("Desctops/Two", () =>
      {
        WindowManager.SwitchToDesctop(1);
        AppContext.SetIcon(Resources.TrayIconNumber2);
      });

      AppContext.AddMenuItem("Exit", Application.Exit);
    }

    private static void AddHotkeys()
    {
      KeyboardManager = new KeyboardManager(); // creates a native window
      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D1, () =>
      {
        WindowManager.SwitchToDesctop(0);
        AppContext.SetIcon(Resources.TrayIconNumber1);
      });
      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D2, () =>
      {
        WindowManager.SwitchToDesctop(1);
        AppContext.SetIcon(Resources.TrayIconNumber2);
      });

      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.Oemtilde, ConsoleUi.ToggleConsole);
    }


    private static void AddCommands()
    {
      ConsoleUi.AddCommand("wnd_list", "list all windows")
        .Method(cx =>
        {
          ConsoleUi.WriteLine("windows:");
          WindowManager.GetActiveWindows();
        });

      ConsoleUi.AddCommand("desk_switch", "switch desctop")
        .Param("id", "zerobased desktop id")
        .Method(cx =>
        {
          WindowManager.SwitchToDesctop(cx.Required<int>("id"));
        });
    }
  }
}
