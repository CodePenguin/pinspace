
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renamePinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePinMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeColorMenuItem,
            this.newPinMenuItem,
            this.renamePinMenuItem,
            this.removePinMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(155, 92);
            this.contextMenuStrip.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.ContextMenuStrip_Closing);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_Opening);
            // 
            // changeColorMenuItem
            // 
            this.changeColorMenuItem.Name = "changeColorMenuItem";
            this.changeColorMenuItem.Size = new System.Drawing.Size(154, 22);
            this.changeColorMenuItem.Text = "Change color...";
            this.changeColorMenuItem.Click += new System.EventHandler(this.ChangeColorMenuItem_Click);
            // 
            // newPinMenuItem
            // 
            this.newPinMenuItem.Name = "newPinMenuItem";
            this.newPinMenuItem.Size = new System.Drawing.Size(154, 22);
            this.newPinMenuItem.Text = "New";
            // 
            // renamePinMenuItem
            // 
            this.renamePinMenuItem.Name = "renamePinMenuItem";
            this.renamePinMenuItem.Size = new System.Drawing.Size(154, 22);
            this.renamePinMenuItem.Text = "Rename...";
            this.renamePinMenuItem.Click += new System.EventHandler(this.RenamePinMenuItem_Click);
            // 
            // removePinMenuItem
            // 
            this.removePinMenuItem.Name = "removePinMenuItem";
            this.removePinMenuItem.Size = new System.Drawing.Size(154, 22);
            this.removePinMenuItem.Text = "Remove";
            this.removePinMenuItem.Click += new System.EventHandler(this.RemovePinMenuItem_Click);
            // 
            // PinWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PinWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pinspace";
            this.ResizeEnd += new System.EventHandler(this.Form_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.Form_LocationChanged);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newPinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renamePinMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeColorMenuItem;
    }
}

