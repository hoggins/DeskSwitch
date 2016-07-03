using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InteractiveConsole;

namespace DesktopSwitch
{
  class ConsoleUi : ConsoleManager
  {
    public static readonly ConsoleUi Current = new ConsoleUi();
    private static Thread _inputThread;
    private static bool _isShown;

    #region Base

    public static RawConsoleCommand AddCommand(string name, string description)
    {
      return Current.AddCommand(new RawConsoleCommand(name, description));
    }

    public new static void WriteLine()
    {
      ((ConsoleManager)Current).WriteLine();
    }

    public new static void WriteLine(string value)
    {
      ((ConsoleManager)Current).WriteLine(value);
    }

    public new static void WriteLine(String format, params Object[] args)
    {
      ((ConsoleManager)Current).WriteLine(format, args);
    }

    #endregion

    public static void ToggleConsole()
    {
      if (_isShown)
        HideConsole();
      else
        ShowConsole();
    }

    public static void InitConsole(bool hidden = true)
    {
      AllocConsole();
      _isShown = true;

      if (hidden)
        HideConsole();

      StartReading();
    }

    public static void ShowConsole()
    {
      _isShown = true;
      var handle = GetConsoleWindow();
      if (handle != IntPtr.Zero)
        ShowWindow(handle, SW_SHOW);
    }

    public static void HideConsole()
    {
      _isShown = false;
      var handle = GetConsoleWindow();
      if (handle != IntPtr.Zero)
        ShowWindow(handle, SW_HIDE);
    }

    private static void StartReading()
    {
      if (_inputThread != null)
        return;
      _inputThread = new Thread(() =>
      {
        Current.Start();
      }) {IsBackground = true};
      _inputThread.Start();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
  }
}
