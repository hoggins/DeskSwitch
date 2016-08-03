using System.Diagnostics;

namespace DesktopSwitch
{
  public static class ProcessManager
  {
    public static void StartApp(string appPath)
    {
      Process.Start(appPath);
    }

    public static void RunCmd(string cmd)
    {
      var process = new Process();
      var startInfo = new ProcessStartInfo
      {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
        Arguments = cmd
      };
      process.StartInfo = startInfo;
      process.Start();
    }
  }
}
