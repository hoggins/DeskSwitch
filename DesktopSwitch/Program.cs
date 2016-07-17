using System;
using System.Diagnostics;
using System.Windows.Forms;
using DeploymentUtil.Utils;
using InteractiveConsole;

namespace DesktopSwitch
{
  class AppController
  {
    public static WindowManager WindowManager;

    public static AppContext Context;
    public static KeyboardManager KeyboardManager;
    public static ScreenshotManager ScreenshotManager;

    [STAThread]
    static void Main(string[] args)
    {
      Initialize();
    }

    private static void Initialize()
    {
      ConsoleUi.InitConsole(); // allocate console before any windown created
      
      ScreenshotManager = new ScreenshotManager();
      using (WindowManager = new WindowManager())
      using (Context = new AppContext())
      {

        AddCommands();
        AddHotkeys(); // creates a native window
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
      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D3, () => WindowManager.SwitchToDesctop(2));
      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.D4, () => WindowManager.SwitchToDesctop(3));
      
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D1, () => WindowManager.MoveWindow(0));
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D2, () => WindowManager.MoveWindow(1));
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D3, () => WindowManager.MoveWindow(2));
      KeyboardManager.AddHotkey(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D4, () => WindowManager.MoveWindow(3));
      
      KeyboardManager.AddHotkey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D1, () => WindowManager.MoveWindow(0, true));
      KeyboardManager.AddHotkey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D2, () => WindowManager.MoveWindow(1, true));
      KeyboardManager.AddHotkey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D3, () => WindowManager.MoveWindow(2, true));
      KeyboardManager.AddHotkey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D4, () => WindowManager.MoveWindow(3, true));

      KeyboardManager.AddHotkey(ModifierKeys.Alt, Keys.Oemtilde, ConsoleUi.ToggleConsole);

      KeyboardManager.AddHotkey(ModifierKeys.None, Keys.LaunchMail, () => ProcessManager.StartApp("calc.exe")); // my keybord doesn't have Calc button :(
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

      ConsoleUi.AddCommand("scr", "make a screenshot")
        .Method(cx =>
        {
          ScreenshotManager.StartCaptureMode();
        });
    }
  }
}
