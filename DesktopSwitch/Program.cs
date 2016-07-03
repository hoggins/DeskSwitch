using System;
using System.Windows.Forms;
using DeploymentUtil.Utils;
using InteractiveConsole;

namespace DesktopSwitch
{
  class AppController
  {
    public readonly static WindowManager WindowManager = new WindowManager();

    public static AppContext Context;
    public static KeyboardManager KeyboardManager;

    [STAThread]
    static void Main(string[] args)
    {
      ConsoleUi.InitConsole(); // allocate console before any windown created
      AddCommands();
      
      AddHotkeys(); // creates a native window

      using (Context = new AppContext())
      {
        AddAppMenu();
        Application.Run(Context);
      }

//      Application.EnableVisualStyles();
//      Application.SetCompatibleTextRenderingDefault(false);
//      Application.Run(new Form1());
    }

    private static void AddAppMenu()
    {
      Context.AddMenuItem("Desctops/One", () => WindowManager.SwitchToDesctop(0));
      Context.AddMenuItem("Desctops/Two", () => WindowManager.SwitchToDesctop(1));
      Context.AddMenuItem("Exit", Application.Exit);
    }

    private static void AddHotkeys()
    {
      KeyboardManager = new KeyboardManager(); // creates a native window

      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D1, () => WindowManager.SwitchToDesctop(0));
      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D2, () => WindowManager.SwitchToDesctop(1));
      
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D1, () => { WindowManager.MoveWindow(0); });
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D2, () => { WindowManager.MoveWindow(1); });

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
