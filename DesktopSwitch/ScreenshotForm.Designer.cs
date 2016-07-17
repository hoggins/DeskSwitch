using System;
using System.Windows.Forms;

namespace DesktopSwitch
{
  partial class ScreenshotForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.refresh_timer = new System.Windows.Forms.Timer(this.components);
      this._saveBtn = new System.Windows.Forms.Button();
      this._cancelBtn = new System.Windows.Forms.Button();
      this._actionsContainer = new System.Windows.Forms.Panel();
      this._actionsContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // refresh_timer
      // 
      this.refresh_timer.Enabled = true;
      this.refresh_timer.Interval = 50;
      this.refresh_timer.Tick += new System.EventHandler(this.refresh_timer_Tick);
      // 
      // _saveBtn
      // 
      this._saveBtn.Location = new System.Drawing.Point(3, 44);
      this._saveBtn.Name = "_saveBtn";
      this._saveBtn.Size = new System.Drawing.Size(75, 23);
      this._saveBtn.TabIndex = 0;
      this._saveBtn.Text = "Save";
      this._saveBtn.UseVisualStyleBackColor = true;
      this._saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
      // 
      // _cancelBtn
      // 
      this._cancelBtn.Location = new System.Drawing.Point(3, 15);
      this._cancelBtn.Name = "_cancelBtn";
      this._cancelBtn.Size = new System.Drawing.Size(75, 23);
      this._cancelBtn.TabIndex = 1;
      this._cancelBtn.Text = "Cancel";
      this._cancelBtn.UseVisualStyleBackColor = true;
      this._cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
      // 
      // _actionsContainer
      // 
      this._actionsContainer.Controls.Add(this._cancelBtn);
      this._actionsContainer.Controls.Add(this._saveBtn);
      this._actionsContainer.Location = new System.Drawing.Point(12, 12);
      this._actionsContainer.Name = "_actionsContainer";
      this._actionsContainer.Size = new System.Drawing.Size(128, 70);
      this._actionsContainer.TabIndex = 2;
      // 
      // ScreenshotForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 261);
      this.Controls.Add(this._actionsContainer);
      this.Name = "ScreenshotForm";
      this.Text = "ScreenshotForm";
      this.Load += new System.EventHandler(this.ScreenshotForm_Load);
      this._actionsContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Timer refresh_timer;
    private Button _saveBtn;
    private Button _cancelBtn;
    private Panel _actionsContainer;
  }
}