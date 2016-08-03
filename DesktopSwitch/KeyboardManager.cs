using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopSwitch
{
  public class KeyboardManager : IDisposable
  {
    private readonly Dictionary<long, List<Action>> _hotkeys = new Dictionary<long, List<Action>>();
    private int _currentId;

    public void Initialize()
    {
      AppController.PompWnd.MessageArrived += OnMessageArrived;
    }

    public void AddHotkey(ModifierKeys modifier, Keys key, Action action)
    {
      var idx = MakeIndex(modifier, key);
      List<Action> actions;
      if (!_hotkeys.TryGetValue(idx, out actions))
        _hotkeys.Add(idx, actions = new List<Action>());
      actions.Add(action);

      if (!RegisterHotKey(AppController.PompWnd.Handle, ++_currentId, (uint)modifier, (uint)key))
        ConsoleUi.WriteLine("Fail to register hotkey {0}({1}) {2}({3})", modifier, (uint)modifier, key, (uint)key);
    }

    public void Dispose()
    {
      for (int i = _currentId; i > 0; i--)
        UnregisterHotKey(AppController.PompWnd.Handle, i);
    }

    private void OnMessageArrived(Message m)
    {
      if (m.Msg != MessagePompWnd.WM_HOTKEY)
        return;

      var modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
      var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
      GlobalKeyPressed(MakeIndex(modifier, key));
    }

    private void GlobalKeyPressed(long keyIdx)
    {
      List<Action> actions;
      if (!_hotkeys.TryGetValue(keyIdx, out actions))
        return;

      for (int i = actions.Count - 1; i >= 0; i--)
        actions[i].Invoke();
    }

    private static long MakeIndex(ModifierKeys modifier, Keys key)
    {
      return ((long) modifier << 32) | (int) key;
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
  }

  [Flags]
  public enum ModifierKeys : uint
  {
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8
  }
}
