using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopSwitch
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      KeyPreview = true;
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
      Console.WriteLine(@"key pres {0}, {1}", e.Modifiers, e.KeyCode);
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {

    }
  }
}
