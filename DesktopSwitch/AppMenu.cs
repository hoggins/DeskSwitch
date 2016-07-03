using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopSwitch
{
  public class AppMenu : IDisposable
  {
    private NotifyIcon _trayIcon;
    private Icon _icon;

    private readonly MenuNode _menuRoot = MenuNode.MakeRoot();


    public void AddItem(string itemPath, Action action)
    {
      var subPath = itemPath.Split('/');
      var currentNode = _menuRoot;
      for (int i = 0; i < subPath.Length; i++)
      {
        var name = subPath[i];
        if (i == subPath.Length - 1)
        {
          currentNode.AddSubNode(name, action);
        }
        else
        {
          currentNode = currentNode.GetOrAddSubNode(name);
        }
      }

      UpdateMenu();
    }

    public void InitIcon()
    {
      if (_trayIcon != null)
        return;

      _trayIcon = new NotifyIcon()
      {
        Icon = Resources.TrayIconNumber1,
//        ContextMenu = new ContextMenu(new MenuItem[]
//        {
//          new MenuItem("Test", Test),
//          new MenuItem("Exit", Exit),
//        }),
        Visible = true
      };

      UpdateMenu();
    }

    private void UpdateMenu()
    {
      if (_trayIcon == null)
        return;

      _trayIcon.ContextMenu = new ContextMenu(BuildMenuList(_menuRoot));
    }

    private MenuItem[] BuildMenuList(MenuNode node)
    {
      var res = new List<MenuItem>();
      foreach (var subNode in node.SubNodes.Values)
      {
        if (subNode.SubNodes != null)
        {
          res.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, subNode.Name, null, null, null, BuildMenuList(subNode)));
        }
        else if (subNode.Action != null)
        {
          var localNode = subNode;
          res.Add(new MenuItem(subNode.Name, (sender, args) => localNode.Action()));
        }
        else
        {
          res.Add(new MenuItem(subNode.Name + " <no action>"));
        }
      }
      return res.ToArray();
    }

    private void Test(object sender, EventArgs e)
    {
      
      _trayIcon.ContextMenu = new ContextMenu(new MenuItem[] {
        new MenuItem("Test 2", Test), 
        new MenuItem("Exit", Exit),
        new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Test >", (o, args) => {}, (o, args) => {}, (o, args) => { }, new MenuItem[]
        {
          new MenuItem("Test 2", Test), new MenuItem("Test 3", Test), 
        }), 
      });
    }

    public void SetIcon(Icon icon)
    {
      _icon = icon;
      if (_trayIcon != null)
        _trayIcon.Icon = icon;
    }

    void Exit(object sender, EventArgs e)
    {
      

      Application.Exit();
    }

    public void Dispose()
    {
      _trayIcon.Dispose();
    }


    internal class MenuNode
    {
      public string Name;
      public Action Action;
      public Dictionary<string, MenuNode> SubNodes;

      private MenuNode(string name)
      {
        Name = name;
      }

      public static MenuNode MakeRoot()
      {
        return new MenuNode(@"root")
        {
          SubNodes = new Dictionary<string, MenuNode>()
        };
      }

      public MenuNode GetOrAddSubNode(string name)
      {
        if (SubNodes == null)
          SubNodes = new Dictionary<string, MenuNode>();

        MenuNode node;
        if (!SubNodes.TryGetValue(name, out node))
          SubNodes.Add(name, node = new MenuNode(name));

        return node;
      }

      public MenuNode AddSubNode(string name, Action action)
      {
        if (SubNodes == null)
          SubNodes = new Dictionary<string, MenuNode>();

        return SubNodes[name] = new MenuNode(name){Action = action};
      }
    }
  }
}