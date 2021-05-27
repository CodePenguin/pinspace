
namespace Pinspaces.Controls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinWindowForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.pinspaceDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pinspaceDropDownButton});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 497);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(933, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // pinspaceDropDownButton
            // 
            this.pinspaceDropDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pinspaceDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pinspaceDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("pinspaceDropDownButton.Image")));
            this.pinspaceDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pinspaceDropDownButton.Name = "pinspaceDropDownButton";
            this.pinspaceDropDownButton.Size = new System.Drawing.Size(67, 20);
            this.pinspaceDropDownButton.Text = "Pinspace";
            // 
            // PinWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.statusStrip);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PinWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pinspace";
            this.ResizeEnd += new System.EventHandler(this.Form_LocationOrPositionChanged);
            this.LocationChanged += new System.EventHandler(this.Form_LocationOrPositionChanged);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripDropDownButton pinspaceDropDownButton;
    }
}

