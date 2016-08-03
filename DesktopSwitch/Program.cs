using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using InteractiveConsole;

namespace DesktopSwitch
{
  class AppController
  {
    public static SettingsManager Settings;
    public static AppContext Context;


    public static MessagePompWnd PompWnd;
    public static WindowManager WindowManager;

    public static KeyboardManager KeyboardManager;
    public static ScreenshotManager ScreenshotManager;

    [STAThread]
    static void Main(string[] args)
    {
      Initialize();
    }

    private static void Initialize()
    {
//      Application.EnableVisualStyles();
//      Application.SetCompatibleTextRenderingDefault(false);
//      Application.Run(new Form1());
//      return;

      Settings = new SettingsManager();
      Settings.Init("app.config");


      ConsoleUi.InitConsole(); // allocate console before any windown created

      
      ScreenshotManager = new ScreenshotManager();
      using (KeyboardManager = new KeyboardManager())
      using (WindowManager = new WindowManager())
      using (Context = new AppContext())
      using (PompWnd = new MessagePompWnd())
      {
        KeyboardManager.Initialize();
        ScreenshotManager.Initialize();

        AddCommands();
        AddHotkeys(); // creates a native window
        AddAppMenu();


        Application.Run(Context);
      }
    }

    private static void AddAppMenu()
    {
      Context.AddMenuItem("Desctops/One", () => WindowManager.SwitchToDesctop(0));
      Context.AddMenuItem("Desctops/Two", () => WindowManager.SwitchToDesctop(1));
      Context.AddMenuItem("Exit", Application.Exit);
    }

    private static void AddHotkeys()
    {
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
      KeyboardManager.AddHotkey(ModifierKeys.Control, Keys.PrintScreen, ()=>ScreenshotManager.StartCaptureMode());

      KeyboardManager.AddHotkey(ModifierKeys.None, Keys.LaunchMail, () => ProcessManager.StartApp("calc.exe")); // my keybord doesn't have Calc button :(
      KeyboardManager.AddHotkey(ModifierKeys.None, (Keys)172, () => ProcessManager.StartApp("cmd.exe"));
      KeyboardManager.AddHotkey(ModifierKeys.None, (Keys)168, LockWorkStation);
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

    [DllImport("user32")]
    public static extern void LockWorkStation();
  }
}
