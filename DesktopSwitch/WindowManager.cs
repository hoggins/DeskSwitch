using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DesktopSwitch
{
  public class WindowManager
  {
    public const int DesktopsCount = 4;
    public int CurrentDesctop { get; private set; }
    private readonly HashSet<IntPtr>[] _windowsByDesktop;

    public WindowManager()
    {
      _windowsByDesktop = new HashSet<IntPtr>[DesktopsCount];
      for (var i = 0; i < DesktopsCount; i++)
        _windowsByDesktop[i] = new HashSet<IntPtr>();
    }

    public void UpdateCurrentDesctop()
    {
      _windowsByDesktop[CurrentDesctop] = GetActiveWindows();
    }

    /// <param name="idx">zerobased index of the desctop</param>
    public void SwitchToDesctop(int idx)
    {
      ConsoleUi.WriteLine("Switch from {0} to {1}", CurrentDesctop, idx);
      if (idx == CurrentDesctop && idx < 0 && idx >= DesktopsCount)
        return;
      UpdateCurrentDesctop();

      var currentWnds = _windowsByDesktop[CurrentDesctop];
      var targetWnds = _windowsByDesktop[idx];

      var toHide = currentWnds.Where(c => !targetWnds.Contains(c));
      var toShow = targetWnds.Where(t => !currentWnds.Contains(t));

      CurrentDesctop = idx;

      foreach (var ptr in toHide)
        ShowWindow(ptr, SW_HIDE);

      foreach (var ptr in toShow)
        ShowWindow(ptr, SW_SHOW);
    }

    public static HashSet<IntPtr> GetActiveWindows()
    {
      var collection = new HashSet<IntPtr>();
      EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
      {
        StringBuilder strbTitle = new StringBuilder(255);
        int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
        string strTitle = strbTitle.ToString();

        if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
        {
          // debug filter
          int processId;
          GetWindowThreadProcessId(hWnd, out processId);
          if (Process.GetProcessById(processId).ProcessName != "notepad")
            return true;

          ConsoleUi.WriteLine(strTitle);
          collection.Add(hWnd);
        }
        return true;
      };

      EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
      return collection;
    }

    public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

    [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

//    [DllImport("user32.dll", SetLastError = true)]
//    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

    private static HashSet<IntPtr> GetActiveWindows1()
    {
      var openWindowProcesses = System.Diagnostics.Process.GetProcesses()
        .Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName != "explorer");

      openWindowProcesses = openWindowProcesses.Where(p => p.ProcessName == "notepad");

      ConsoleUi.WriteLine(string.Join("\r\n", openWindowProcesses.Select(p => p.MainWindowTitle)));
      return new HashSet<IntPtr>(openWindowProcesses.Select(p=>p.MainWindowHandle));
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