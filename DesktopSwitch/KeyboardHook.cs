using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopSwitch
{
  /*/// <summary>
  /// http://www.liensberger.it/web/blog/?p=207
  /// </summary>
  internal sealed class KeyboardHook : IDisposable
  {

    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    private MessagePompWnd _pompWnd;
    private int _currentId;

    public KeyboardHook(MessagePompWnd pompWnd)
    {
      _pompWnd = pompWnd;
      _pompWnd.
    }

    public void OnMessage(Message m)
    {
      if (m.Msg != MessagePompWnd.WM_HOTKEY)
        return;

      // get the keys.
      Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
      ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

      // invoke the event to notify the parent.
      if (KeyPressed != null)
        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
    }

    /// <summary>
    /// Registers a hot key in the system.
    /// </summary>
    /// <param name="modifier">The modifiers that are associated with the hot key.</param>
    /// <param name="key">The key itself that is associated with the hot key.</param>
    public void RegisterHotKey(ModifierKeys modifier, Keys key)
    {
      // increment the counter.
      _currentId = _currentId + 1;

      // register the hot key.
      if (!RegisterHotKey(_pompWnd.Handle, _currentId, (uint)modifier, (uint)key))
        throw new InvalidOperationException("Couldn’t register the hot key.");
    }

    

    public void Dispose()
    {
      // unregister all the registered hot keys.
      for (int i = _currentId; i > 0; i--)
      {
        UnregisterHotKey(_pompWnd.Handle, i);
      }
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  }

  /// <summary>
  /// Event Args for the event that is fired after the hot key has been pressed.
  /// </summary>
  public class KeyPressedEventArgs : EventArgs
  {
    private ModifierKeys _modifier;
    private Keys _key;

    internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
    {
      _modifier = modifier;
      _key = key;
    }

    public ModifierKeys Modifier
    {
      get { return _modifier; }
    }

    public Keys Key
    {
      get { return _key; }
    }
  }

  /// <summary>
  /// The enumeration of possible modifiers.
  /// </summary>*/
  
}
