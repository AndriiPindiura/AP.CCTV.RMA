namespace AP.CCTV.RMA
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axCamMonitor1 = new AxACTIVEXLib.AxCamMonitor();
            ((System.ComponentModel.ISupportInitialize)(this.axCamMonitor1)).BeginInit();
            this.SuspendLayout();
            // 
            // axCamMonitor1
            // 
            this.axCamMonitor1.Enabled = true;
            this.axCamMonitor1.Location = new System.Drawing.Point(18, 24);
            this.axCamMonitor1.Name = "axCamMonitor1";
            this.axCamMonitor1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCamMonitor1.OcxState")));
            this.axCamMonitor1.Size = new System.Drawing.Size(247, 220);
            this.axCamMonitor1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.axCamMonitor1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axCamMonitor1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxACTIVEXLib.AxCamMonitor axCamMonitor1;
    }
}