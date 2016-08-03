using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DesktopSwitch
{
  public class ScreenshotSettings
  {
    [JsonProperty] public string OutputPath;
    [JsonProperty] public bool IsFullscreen = false;
  }

  public class ScreenshotManager
  {
    public bool IsCapturing;

    private readonly Lazy<ScreenshotSettings> _settings = AppController.Settings.GetOrNew<ScreenshotSettings>();
    private ScreenshotSettings Settings { get { return _settings.Value; } }

    private Thread _wndThread;
    private Bitmap _screenshot;
    private ScreenshotForm _form;

    private const string FilePrefix = "Screenshot_";
    private const string FileExtension = ".png";

    public void Initialize()
    {
      TryInitDefaults();
      AddAdminCommands();
    }

    private void TryInitDefaults()
    {
      if (Settings.OutputPath != null)
        return;

      using (var settings = AppController.Settings.GetEditContext<ScreenshotSettings>())
        settings.Value.OutputPath = Application.StartupPath;
    }

    public void StartCaptureMode()
    {
      if (IsCapturing)
        AbortCapturing();
      IsCapturing = true;

      _screenshot = TakeScreenshot();

      _wndThread = new Thread(StartForm);
      _wndThread.Start();
    }

    private void StartForm()
    {
      _form = new ScreenshotForm();
      _form.Bounds = GetScreenBoundingBox();
      if (Settings.IsFullscreen)
      {
        _form.TopMost = true;
//        _form.WindowState = FormWindowState.Maximized;
//        _form.FormBorderStyle = FormBorderStyle.None;
        _form.FormBorderStyle = FormBorderStyle.Sizable;
        _form.BackColor = Color.White;
      }
      _form.Controller = this;
      _form.Screenshot = _screenshot;
      _form.Show(AppController.Context.MainForm);

      Application.EnableVisualStyles();
      Application.Run(_form);
    }

    private static Rectangle GetScreenBoundingBox()
    {
      var bb = GetGlobalScreenRect(Screen.AllScreens[0].Bounds);
      var minX = bb.Left;
      var maxX = bb.Right;
      var minY = bb.Top;
      var maxY = bb.Bottom;
      foreach (var screen in Screen.AllScreens)
      {
        bb = GetGlobalScreenRect(screen.Bounds);
        if (minX > bb.Left) minX = bb.Left;
        if (maxX < bb.Right) maxX = bb.Right;
        if (minY > bb.Top) minY = bb.Top;
        if (maxY > bb.Bottom) maxY = bb.Bottom;
      }
      return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    private static Rectangle GetGlobalScreenRect(Rectangle rect)
    {
      return new Rectangle(rect.Location.X, rect.Location.Y, rect.Width, rect.Height);
    }

    public void AbortCapturing()
    {
      if (_form != null)
      {
        var form = _form;
        _form = null;
        form.Invoke((MethodInvoker)delegate
        {
            // close the form on the forms thread
          form.Close();
        });
        
      }

      if (_wndThread != null)
      {
        _wndThread.Abort();
        _wndThread = null;
      }

      if (_screenshot != null)
      {
        _screenshot.Dispose();
        _screenshot = null;
      }
    }

    public static Bitmap TakeScreenshot()
    {
      //http://stackoverflow.com/questions/5049122/capture-the-screen-shot-using-net
      var screenBb = GetScreenBoundingBox();
      var bmpScreenCapture = new Bitmap(screenBb.Width, screenBb.Height);

      using (Graphics g = Graphics.FromImage(bmpScreenCapture))
        g.CopyFromScreen(0, 0, 0, 0, bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);

      bmpScreenCapture.Save("raw.png", ImageFormat.Png);

      return bmpScreenCapture;
    }

    public void Save(Rectangle rect)
    {
      var fileName = GetOutFileName();
      Bitmap cropped = (Bitmap)_screenshot.Clone(rect, _screenshot.PixelFormat);
      cropped.Save(fileName, ImageFormat.Png);

      SetClipboardFile(fileName);

      ShowInFolder(fileName);

      AbortCapturing();
    }

    private void ShowInFolder(string fileName)
    {
      Process.Start("explorer.exe", Path.GetDirectoryName(fileName));
    }

    private void SetClipboardFile(string fileName)
    {
      System.Collections.Specialized.StringCollection FileCollection = new System.Collections.Specialized.StringCollection();


      FileCollection.Add(fileName);


      
        // close the form on the forms thread
        //form.Close();
      //  Clipboard.SetFileDropList(FileCollection);
      

    }

    private string GetOutFileName()
    {
      var files = Directory.GetFiles(Settings.OutputPath).Select(Path.GetFileName).Where(f=>f.StartsWith(FilePrefix) && f.EndsWith(FileExtension));
      var usedIds = files.Select(ParseFileId).ToList();
      var lastId = usedIds.Any() ? usedIds.Max() : 0;
      return Path.Combine(Settings.OutputPath, string.Concat(FilePrefix, lastId+1, FileExtension));
    }

    private int ParseFileId(string fileName)
    {
      var valStr = fileName.Substring(FilePrefix.Length, fileName.Length - FilePrefix.Length - FileExtension.Length);
      int val;
      int.TryParse(valStr, out val);
      return val;
    }

    public void SetupOutputPath(string path)
    {
      var dialog = new FolderBrowserDialog();
      if (dialog.ShowDialog() != DialogResult.OK)
        return;

      using (var settings = AppController.Settings.GetEditContext<ScreenshotSettings>())
        settings.Value.OutputPath = dialog.SelectedPath;
    }

    public void AddAdminCommands()
    {
      ConsoleUi.AddCommand("scr", "make a screenshot")
        .Method(cx =>
        {
          StartCaptureMode();
        });
      ConsoleUi.AddCommand("scr_setup", "setup output folder")
        .Method(cx =>
        {
          SetupOutputPath(null);
        });
    }
  }

  public class DesctopDrawer : IDisposable
  {
    private IntPtr _desktopPtr;
    private Graphics _graphics;

    public Graphics GetCx()
    {
      _desktopPtr = GetDC(IntPtr.Zero);
      _graphics = Graphics.FromHdc(_desktopPtr);
      return _graphics;
    }

    public void Dispose()
    {
      _graphics.Dispose();
      _graphics = null;
      ReleaseDC(IntPtr.Zero, _desktopPtr);
    }

    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("User32.dll")]
    public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

  }
}
