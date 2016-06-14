namespace RoboCup.AtHome.CommandGenerator.GUI
{
	partial class QRDialog
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
			this.pbQRCode = new System.Windows.Forms.PictureBox();
			this.lblQRText = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbQRCode)).BeginInit();
			this.SuspendLayout();
			// 
			// pbQRCode
			// 
			this.pbQRCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pbQRCode.Location = new System.Drawing.Point(9, 9);
			this.pbQRCode.Margin = new System.Windows.Forms.Padding(0);
			this.pbQRCode.Name = "pbQRCode";
			this.pbQRCode.Size = new System.Drawing.Size(466, 398);
			this.pbQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pbQRCode.TabIndex = 0;
			this.pbQRCode.TabStop = false;
			// 
			// lblQRText
			// 
			this.lblQRText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblQRText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblQRText.Location = new System.Drawing.Point(9, 410);
			this.lblQRText.Margin = new System.Windows.Forms.Padding(3);
			this.lblQRText.Name = "lblQRText";
			this.lblQRText.Size = new System.Drawing.Size(466, 40);
			this.lblQRText.TabIndex = 1;
			this.lblQRText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// QRDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(484, 462);
			this.Controls.Add(this.lblQRText);
			this.Controls.Add(this.pbQRCode);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Name = "QRDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "QRDialog";
			this.Load += new System.EventHandler(this.QRDialog_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.QRDialog_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QRDialog_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.QRDialog_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pbQRCode)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbQRCode;
		private System.Windows.Forms.Label lblQRText;
	}
}