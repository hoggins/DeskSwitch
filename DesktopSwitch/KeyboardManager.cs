using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DeploymentUtil.Utils;

namespace DesktopSwitch
{
  public class KeyboardManager : IDisposable
  {
    private readonly KeyboardHook _keyboard;
    private readonly Dictionary<long, List<Action>> _hotkeys = new Dictionary<long, List<Action>>();

    public KeyboardManager()
    {
      _keyboard = new KeyboardHook();
      _keyboard.KeyPressed += GlobalKeyPressed;
    }

    public void AddHotkey(ModifierKeys modifier, Keys key, Action action)
    {
      var idx = MakeIndex(modifier, key);
      List<Action> actions;
      if (!_hotkeys.TryGetValue(idx, out actions))
        _hotkeys.Add(idx, actions = new List<Action>());
      actions.Add(action);

      // note: this call may fail because KeyboardHook is init yet
      _keyboard.RegisterHotKey(modifier, key);
    }

    private void GlobalKeyPressed(object sender, KeyPressedEventArgs e)
    {
      var idx = MakeIndex(e.Modifier, e.Key);
      List<Action> actions;
      if (!_hotkeys.TryGetValue(idx, out actions))
        return;

      for (int i = actions.Count - 1; i >= 0; i--)
        actions[i].Invoke();
    }

    public void Dispose()
    {
      _keyboard.Dispose();
    }

    private static long MakeIndex(ModifierKeys modifier, Keys key)
    {
      return ((long) modifier << 32) | (int) key;
    }
  }
}
