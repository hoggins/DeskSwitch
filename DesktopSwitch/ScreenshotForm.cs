using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopSwitch
{
  public partial class ScreenshotForm : Form
  {
    public ScreenshotManager Controller;
    public Bitmap Screenshot;

    private bool _inputArea;
    private Point? _clipFrom;
    private Point? _clipTo;

    public ScreenshotForm()
    {
      InitializeComponent();

      HideUi();
    }

    private void ScreenshotForm_Load(object sender, EventArgs e)
    {
      DoubleBuffered = true;
      Paint += OnPaint;
      MouseClick += OnMouseClick;
      MouseDown += OnMouseDown;
      MouseMove += OnMouseMove;
      MouseUp += OnMouseUp;
    }

    private void OnMouseClick(object sender, MouseEventArgs mouseEventArgs)
    {

    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
      _clipFrom = e.Location;
      _clipTo = e.Location;

      _inputArea = true;
      HideUi();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (_inputArea)
        _clipTo = e.Location;
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
      _inputArea = false;
      _clipTo = e.Location;

      ShowUi(_clipTo.Value);
    }

    private void SaveBtn_Click(object sender, EventArgs e)
    {
      if (_clipFrom.HasValue && _clipTo.HasValue)
        Controller.Save(ToRectangle(_clipFrom.Value, _clipTo.Value));
    }

    private void CancelBtn_Click(object sender, EventArgs e)
    {

    }

    private void HideUi()
    {
      _actionsContainer.Visible = false;
    }

    private void ShowUi(Point p)
    {
      _actionsContainer.Location = new Point(p.X - _actionsContainer.Width, p.Y - _actionsContainer.Height);
      _actionsContainer.Visible = true;
    }

    private void refresh_timer_Tick(object sender, EventArgs e)
    {
      Refresh();
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
      var g = e.Graphics;
      
      g.DrawImage(Screenshot, new Point(0,0));
      
      if (_clipFrom.HasValue && _clipTo.HasValue)
      {
        var rectangle = ToRectangle(_clipFrom.Value, _clipTo.Value);
        var selectionPen = new Pen(Color.Red, 5);
        selectionPen.Alignment = PenAlignment.Outset;
        g.DrawRectangle(selectionPen, rectangle);
      }
    }

    private Rectangle ToRectangle(Point p1, Point p2)
    {
      int x1, x2, y1, y2;
      if (p1.X < p2.X)
      {
        x1 = p1.X;
        x2 = p2.X;
      }
      else
      {
        x1 = p2.X;
        x2 = p1.X;
      }
      if (p1.Y < p2.Y)
      {
        y1 = p1.Y;
        y2 = p2.Y;
      }
      else
      {
        y1 = p2.Y;
        y2 = p1.Y;
      }

      return new Rectangle(x1,y1,x2-x1,y2-y1);
    }
  }
}
