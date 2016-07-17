using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
  }

  public class ScreenshotManager
  {
    private const bool IsFullscreen = false;

    public bool IsCapturing;

    private readonly Lazy<ScreenshotSettings> _settings = AppController.Settings.GetOrNew<ScreenshotSettings>();
    private ScreenshotSettings Settings { get { return _settings.Value; } }

    private Thread _wndThread;
    private Bitmap _screenshot;
    private ScreenshotForm _form;

    public void Initialize()
    {
      if (Settings.OutputPath == null)
      {
        using (var settings = AppController.Settings.GetEditContext<ScreenshotSettings>())
        {
          settings.Value.OutputPath = Application.StartupPath;
        }
      }
    }

    public void StartCaptureMode()
    {
      if (IsCapturing)
        AbortCapturing();
      IsCapturing = true;

      _screenshot = TakeScreenshot();

      _wndThread = new Thread(() =>
      {
        _form = new ScreenshotForm();
        _form.Bounds = Screen.PrimaryScreen.Bounds;
        if (IsFullscreen)
        {
          _form.BackColor = Color.White;
          _form.FormBorderStyle = FormBorderStyle.None;
          _form.TopMost = true;
        }
        _form.Screenshot = _screenshot;
        _form.Show(AppController.Context.MainForm);

        Application.EnableVisualStyles();
        Application.Run(_form);
      });
      _wndThread.Start();
    }

    private void AbortCapturing()
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
      var bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

      using (Graphics g = Graphics.FromImage(bmpScreenCapture))
      {
        //g.CopyFromScreen(0, 0, Screen.PrimaryScreen.Bounds.X,Screen.PrimaryScreen.Bounds.Y,bmpScreenCapture.Size,CopyPixelOperation.SourceCopy);
        g.CopyFromScreen(0, 0, 0, 0, bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);
      }
      bmpScreenCapture.Save("test.png", ImageFormat.Png);

      return bmpScreenCapture;
    }

    public void Save(Rectangle rect)
    {
      var fileName = GetOutFileName();
    }

    private string GetOutFileName()
    {
      return "test.png";
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
