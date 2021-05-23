
namespace Pinspaces
{
    partial class PinWindowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.pinspacePanel = new Pinspaces.PinspacePanel();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.statusStrip.Location = new System.Drawing.Point(0, 497);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(933, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // pinspacePanel
            // 
            this.pinspacePanel.AutoScroll = true;
            this.pinspacePanel.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.pinspacePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pinspacePanel.Location = new System.Drawing.Point(0, 0);
            this.pinspacePanel.Name = "pinspacePanel";
            this.pinspacePanel.Size = new System.Drawing.Size(933, 497);
            this.pinspacePanel.TabIndex = 3;
            // 
            // PinWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.pinspacePanel);
            this.Controls.Add(this.statusStrip);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PinWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pinspace";
            this.ResizeEnd += new System.EventHandler(this.Form_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.Form_LocationChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private PinspacePanel pinspacePanel;
    }
}

