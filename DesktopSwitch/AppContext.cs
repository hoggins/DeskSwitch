using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopSwitch
{
  public class AppContext : ApplicationContext
  {
    private readonly AppMenu _appMenu = new AppMenu();

    public AppContext()
    {
      _appMenu.InitIcon();
    }

    public void AddMenuItem(string itemPath, Action action)
    {
      _appMenu.AddItem(itemPath, action);
    }

    public void SetIcon(Icon icon)
    {
      _appMenu.SetIcon(icon);
    }

    protected override void Dispose(bool disposing)
    {
      _appMenu.Dispose();

      base.Dispose(disposing);
    }
  }
}