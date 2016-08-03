using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSwitch.Managers
{
  public class AliasManager
  {
    private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();

    public void Init()
    {
      AddCommands();
    }

    private void AddCommands()
    {
      ConsoleUi.AddCommand("alias_add", "add an aliase for command line")
        .Method(cx =>
        {
          var input = string.Join(",", cx.Args);
          var args = input.Split(new []{' '}, 2);
          if (args.Length < 2)
            return "please follow format: alias_add alias command";
          _aliases[args[0]] = args[1];
          return "added aliase";
        });
    }
  }
}
