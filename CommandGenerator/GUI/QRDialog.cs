using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RoboCup.AtHome.CommandGenerator;

namespace RoboCup.AtHome.CommandGenerator.GUI
{
	public partial class QRDialog : Form
	{
		public QRDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Creates and returns a new instance of QRDialog form control after displaying it showing a QR code with the provided text encoded.
		/// </summary>
		/// <param name="text">The text to display in the QR dialog</param>
		/// <returns>The form</returns>
		public static QRDialog OpenQRWindow(string text)
		{
			QRDialog dialog = new QRDialog();

			dialog.pbQRCode.Image = GenerateQRBitmap(text, Math.Min(dialog.pbQRCode.Width, dialog.pbQRCode.Height));
			dialog.lblQRText.Text = text;
			dialog.Text = "QR: " + text;
			dialog.Show();
			return dialog;
		}

		/// <summary>
		/// A QR Encoder for converting generated commands to QR codes.
		/// </summary>
		protected static readonly Gma.QrCodeNet.Encoding.QrEncoder encoder;

		static QRDialog()
		{
			encoder = new Gma.QrCodeNet.Encoding.QrEncoder();
		}

		/// <summary>
		/// Generates a bitmap with a QRCode encoding provided string
		/// </summary>
		/// <param name="text">String to encode.</param>
		/// <param name="size">The size of the QR code to generate</param>
		/// <returns>Bitmap with a QRCode encoding provided string</returns>
		public static Image GenerateQRBitmap(string text, int size)
		{
			Gma.QrCodeNet.Encoding.BitMatrix matrix = encoder.Encode(text).Matrix;
			int pixelSize = Math.Max(1, size / Math.Min(matrix.Height, matrix.Width));
			size = pixelSize * Math.Min(matrix.Height, matrix.Width);
			Bitmap bmp = new Bitmap(size, size);
			Graphics g = Graphics.FromImage(bmp);
			for (int i = 0; i < matrix.Width; ++i)
			{
				for (int j = 0; j < matrix.Width; ++j)
				{
					g.FillRectangle(
						matrix[i, j] ? Brushes.Black : Brushes.White,
						i * pixelSize,
						j * pixelSize,
						pixelSize,
						pixelSize);
				}
			}
			return bmp;
		}

		private void QRDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.Hide();
			this.Dispose();
			Application.Exit();
		}

		private void QRDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
		}

		private void QRDialog_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					this.Hide();
					this.Close();
					return;

				case Keys.F:
					if(e.Control){
						this.WindowState = this.WindowState == FormWindowState.Maximized? FormWindowState.Normal : FormWindowState.Maximized;
					}
					return;
			}
		}

		private void QRDialog_Load(object sender, EventArgs e)
		{
			this.Focus();
			this.BringToFront();
		}
	}
}
