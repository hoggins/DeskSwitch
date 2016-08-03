using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopSwitch
{
  /// <summary>
  /// Represents the window that is used to get the messages.
  /// </summary>
  internal sealed class MessagePompWnd : NativeWindow, IDisposable
  {
    public const int WM_HOTKEY = 0x0312;

    public delegate void MessageHandler<TArgs>(TArgs m);
    public event MessageHandler<Message> MessageArrived = (msg) => { };

    public MessagePompWnd()
    {
      CreateHandle(new CreateParams());
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
      base.WndProc(ref m);

      MessageArrived(m);
    }

    public void Dispose()
    {
      DestroyHandle();
    }
  }

}
