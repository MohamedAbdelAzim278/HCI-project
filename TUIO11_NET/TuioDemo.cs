/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2014 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using System.Net.Sockets;
using System.Globalization;
using System.Text;
public class TuioDemo : Form, TuioListener
{
	private TuioClient client;
	private Dictionary<long, TuioDemoObject> objectList;
	private Dictionary<long, TuioCursor> cursorList;
	private Dictionary<long, TuioBlob> blobList;
	private object cursorSync = new object();
	private object objectSync = new object();
	private object blobSync = new object();
	public Bitmap off;
	public static int width, height;
	private int window_width = 640;
	private int window_height = 480;
	private int window_left = 0;
	private int window_top = 0;
	private int screen_width = Screen.PrimaryScreen.Bounds.Width;
	private int screen_height = Screen.PrimaryScreen.Bounds.Height;
	bool check1 = false, check2 = false, check3 = false, check4 = false, check5 = false, check6 = false;
	private bool fullscreen;
	private bool verbose;
	bool firstGame = false, secondGame = false, secondSenario = false;
	int answer;
	SolidBrush blackBrush = new SolidBrush(Color.Black);
	SolidBrush whiteBrush = new SolidBrush(Color.White);
	float gameMode = 0;
	SolidBrush grayBrush = new SolidBrush(Color.Gray);
	Pen fingerPen = new Pen(new SolidBrush(Color.Blue), 1);

	public TuioDemo(int port)
	{

		verbose = true;
		fullscreen = false;
		width = window_width;
		height = window_height;

		this.ClientSize = new System.Drawing.Size(width, height);
		this.Name = "TuioDemo";
		this.Text = "TuioDemo";

		this.Closing += new CancelEventHandler(Form_Closing);
		this.KeyDown += new KeyEventHandler(Form_KeyDown);

		this.SetStyle(ControlStyles.AllPaintingInWmPaint |
						ControlStyles.UserPaint |
						ControlStyles.DoubleBuffer, true);

		objectList = new Dictionary<long, TuioDemoObject>(128);
		cursorList = new Dictionary<long, TuioCursor>(128);

		off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
		String host = System.Net.Dns.GetHostName();
		tcpClient = new TcpClient(host, 4444);
		stream = tcpClient.GetStream();

		client = new TuioClient(port);
		client.addTuioListener(this);
		client.connect();
		if (stream.DataAvailable)
		{
			byte[] buffer = new byte[1024];
			int received = stream.Read(buffer, 0, buffer.Length);
			string data = Encoding.UTF8.GetString(buffer, 0, received);
			string[] data1 = data.Split(',');
			float.TryParse(data1[0], NumberStyles.Any, CultureInfo.InvariantCulture, out gameMode);
			//float.TryParse(data1[1], NumberStyles.Any, CultureInfo.InvariantCulture, out CY);
			if (gameMode == 1)
				firstGame = true;
			else
				if (gameMode == 2)
				secondGame = true;

		}
	}
	float CX = 0, CY = 0;
	private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		if (e.KeyData == Keys.S)
		{
			firstGame = true;
		}
		if (e.KeyData == Keys.A)
		{
			secondGame = true;
		}
		if (e.KeyData == Keys.W)
		{
			
		}
		if (e.KeyData == Keys.F1)
		{
			if (fullscreen == false)
			{

				width = screen_width;
				height = screen_height;

				window_left = this.Left;
				window_top = this.Top;

				this.FormBorderStyle = FormBorderStyle.None;
				this.Left = 0;
				this.Top = 0;
				this.Width = screen_width;
				this.Height = screen_height;

				fullscreen = true;
			}
			else
			{

				width = window_width;
				height = window_height;

				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.Left = window_left;
				this.Top = window_top;
				this.Width = window_width;
				this.Height = window_height;

				fullscreen = false;
			}
		}
		else if (e.KeyData == Keys.Escape)
		{
			this.Close();

		}
		else if (e.KeyData == Keys.V)
		{
			verbose = !verbose;
		}

	}

	private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		client.removeTuioListener(this);

		client.disconnect();
		System.Environment.Exit(0);
	}

	public void addTuioObject(TuioObject o)
	{
		lock (objectSync)
		{
			objectList.Add(o.SessionID, new TuioDemoObject(o));
		}
		if (verbose) Console.WriteLine("add obj " + o.SymbolID + " (" + o.SessionID + ") " + o.X + " " + o.Y + " " + o.Angle);
	}

	public void updateTuioObject(TuioObject o)
	{
		lock (objectSync)
		{
			objectList[o.SessionID].update(o);
		}
		if (verbose) Console.WriteLine("set obj " + o.SymbolID + " " + o.SessionID + " " + o.X + " " + o.Y + " " + o.Angle + " " + o.MotionSpeed + " " + o.RotationSpeed + " " + o.MotionAccel + " " + o.RotationAccel);
	}

	public void removeTuioObject(TuioObject o)
	{
		lock (objectSync)
		{
			objectList.Remove(o.SessionID);
		}
		if (verbose) Console.WriteLine("del obj " + o.SymbolID + " (" + o.SessionID + ")");
	}

	public void addTuioCursor(TuioCursor c)
	{
		lock (cursorSync)
		{
			cursorList.Add(c.SessionID, c);
		}
		if (verbose) Console.WriteLine("add cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y);
	}

	public void updateTuioCursor(TuioCursor c)
	{
		if (verbose) Console.WriteLine("set cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y + " " + c.MotionSpeed + " " + c.MotionAccel);
	}

	public void removeTuioCursor(TuioCursor c)
	{
		lock (cursorSync)
		{
			cursorList.Remove(c.SessionID);
		}
		if (verbose) Console.WriteLine("del cur " + c.CursorID + " (" + c.SessionID + ")");
	}

	public void addTuioBlob(TuioBlob b)
	{
		lock (blobSync)
		{
			blobList.Add(b.SessionID, b);
		}
		if (verbose) Console.WriteLine("add blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area);
	}

	public void updateTuioBlob(TuioBlob b)
	{
		if (verbose) Console.WriteLine("set blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area + " " + b.MotionSpeed + " " + b.RotationSpeed + " " + b.MotionAccel + " " + b.RotationAccel);
	}

	public void removeTuioBlob(TuioBlob b)
	{
		lock (blobSync)
		{
			blobList.Remove(b.SessionID);
		}
		if (verbose) Console.WriteLine("del blb " + b.BlobID + " (" + b.SessionID + ")");
	}

	public void refresh(TuioTime frameTime)
	{
		Invalidate();
	}
	public void GameSad()
	{
		Graphics g = this.CreateGraphics();
		g.FillRectangle(whiteBrush, new Rectangle(0, 0, width, height));
		Font drawFont = new Font("Arial", 16);
		SolidBrush drawBrush = new SolidBrush(Color.Black);
		float x = 0;
		float y = 10;
		int w = 60, start = 80;
		StringFormat drawFormat = new StringFormat();
		//g.DrawString("For sound, show ID 0", drawFont, drawBrush, x, y + 25, drawFormat);
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(start, 100, w, w));
		g.DrawString("ID 0", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(start, 100, w, w));
		g.DrawString("ID 1", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start, 100, w, w));
		g.DrawString("ID 2", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.White), new Rectangle(start, 100, w, w));
		g.DrawString("ID 3", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(start, 100, w, w));
		g.DrawString("ID 4", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(start, 100, w, w));
		// draw the cursor path
		g.DrawString("ID 5", drawFont, drawBrush, start + 5, 170, drawFormat);
		Pen pen = new Pen(Color.Black);
		g.DrawRectangle(pen, new Rectangle(270, 300, w + 50, w + 50));
		g.DrawString("Choose ID 6 to Reset", drawFont, drawBrush, 230, 450, drawFormat);

	}

	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
		// Getting the graphics object
		Graphics g = pevent.Graphics;

		g.FillRectangle(whiteBrush, new Rectangle(0, 0, width, height));
		Font drawFont = new Font("Arial", 16);
		SolidBrush drawBrush = new SolidBrush(Color.Black);
		float x = 0;
		float y = 10;
		int w = 60, start = 80;
		StringFormat drawFormat = new StringFormat();
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(start, 100, w, w));
		g.DrawString("ID 0", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(start, 100, w, w));
		g.DrawString("ID 1", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start, 100, w, w));
		g.DrawString("ID 2", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.White), new Rectangle(start, 100, w, w));
		g.DrawString("ID 3", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(start, 100, w, w));
		g.DrawString("ID 4", drawFont, drawBrush, start + 5, 170, drawFormat);
		start += 82;
		g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(start - 2, 98, w + 4, w + 4));
		g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(start, 100, w, w));
		// draw the cursor path
		g.DrawString("ID 5", drawFont, drawBrush, start + 5, 170, drawFormat);
		Pen pen = new Pen(Color.Black);
		g.DrawRectangle(pen, new Rectangle(270, 300, w + 50, w + 50));
		g.DrawString("Choose ID 6 to Reset", drawFont, drawBrush, 230, 450, drawFormat);

		if (firstGame)
		{

		}
		if (secondGame)
		{
			g.DrawRectangle(pen, new Rectangle(500, 300, w + 50, w + 50));
			g.DrawString("Enter the order of the mixture", drawFont, drawBrush, 230, 412, drawFormat);
		}
		if (cursorList.Count > 0)
		{
			lock (cursorSync)
			{
				foreach (TuioCursor tcur in cursorList.Values)
				{
					List<TuioPoint> path = tcur.Path;
					TuioPoint current_point = path[0];

					for (int i = 0; i < path.Count; i++)
					{
						TuioPoint next_point = path[i];
						g.DrawLine(fingerPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
						current_point = next_point;
					}
					g.FillEllipse(grayBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
					Font font = new Font("Arial", 10.0f);
					g.DrawString(tcur.CursorID + "", font, blackBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
				}
			}
		}

		// draw the objects
		if (objectList.Count > 0)
		{
			lock (objectSync)
			{
				foreach (TuioDemoObject tobject in objectList.Values)
				{
					
						if (tobject.SymbolID == 0)
							check1 = true;
						if (tobject.SymbolID == 1)
							check2 = true;
						if (tobject.SymbolID == 2)
							check3 = true;
						if (tobject.SymbolID == 3)
							check4 = true;
						if (tobject.SymbolID == 4)
							check5 = true;
						if (tobject.SymbolID == 5)
							check6 = true;
						if (tobject.SymbolID == 6)
						{
							check1 = false;
							check2 = false;
							check3 = false;
							check4 = false;
							check5 = false;
							check6 = false;
							g.FillRectangle(new SolidBrush(Color.White), new Rectangle(270, 300, w + 50, w + 50));
						}
						//if (tobject.SymbolID == 6)
						//	check6 = true;
						//tobject.paint(g, check1, check2, check3);
					
					tobject.paint(g);
				}
			}
		}
		if (firstGame)
		{
			if (check1)
			{
				g.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(270, 300, w + 50, w + 50));
				if (check2)
					g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(270, 300, w + 50, w + 50));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.Olive), new Rectangle(270, 300, w + 50, w + 50));//Black
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.LightYellow), new Rectangle(270, 300, w + 50, w + 50));//White
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(270, 300, w + 50, w + 50));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(270, 300, w + 50, w + 50));//Blue
			}
			if (check2)
			{
				g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(270, 300, w + 50, w + 50));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(270, 300, w + 50, w + 50));//yellow
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.DarkRed), new Rectangle(270, 300, w + 50, w + 50));//Black
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.PaleVioletRed), new Rectangle(270, 300, w + 50, w + 50));//white
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(270, 300, w + 50, w + 50));//green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.Purple), new Rectangle(270, 300, w + 50, w + 50));//Blue
			}
			if (check3)
			{
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(270, 300, w + 50, w + 50));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(270, 300, w + 50, w + 50));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.DarkRed), new Rectangle(270, 300, w + 50, w + 50));//Red
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(270, 300, w + 50, w + 50));//White
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.DarkGreen), new Rectangle(270, 300, w + 50, w + 50));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(270, 300, w + 50, w + 50));//Blue
			}
			if (check4)
			{
				g.FillRectangle(new SolidBrush(Color.White), new Rectangle(270, 300, w + 50, w + 50));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.LightYellow), new Rectangle(270, 300, w + 50, w + 50));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.PaleVioletRed), new Rectangle(270, 300, w + 50, w + 50));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(270, 300, w + 50, w + 50));//Black
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.LightGreen), new Rectangle(270, 300, w + 50, w + 50));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(270, 300, w + 50, w + 50));//Blue
			}
			if (check5)
			{
				g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(270, 300, w + 50, w + 50));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(270, 300, w + 50, w + 50));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(270, 300, w + 50, w + 50));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.DarkGreen), new Rectangle(270, 300, w + 50, w + 50));//Black
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.LightGreen), new Rectangle(270, 300, w + 50, w + 50));//White
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.PowderBlue), new Rectangle(270, 300, w + 50, w + 50));//Blue
			}
			if (check6)
			{
				g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(270, 300, w + 50, w + 50));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(270, 300, w + 50, w + 50));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.Purple), new Rectangle(270, 300, w + 50, w + 50));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(270, 300, w + 50, w + 50));//Black
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(270, 300, w + 50, w + 50));//White
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.PowderBlue), new Rectangle(270, 300, w + 50, w + 50));//Green
			}
		}

		if (secondGame && secondSenario == false)
		{
			secondSenario = true;
			Random rr = new Random();
			int r = rr.Next(0, 4);
			if (r == 0)
			{
				answer = 0;
				g.FillRectangle(new SolidBrush(Color.PowderBlue), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
			else
			 if (r == 1)
			{
				answer = 1;
				g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
			else
			 if (r == 2)
			{
				answer = 2;
				g.FillRectangle(new SolidBrush(Color.LightYellow), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
			else
			if (r == 3)
			{
				answer = 3;
				g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
			else
			if (r == 4)
			{
				answer = 4;
				g.FillRectangle(new SolidBrush(Color.DarkRed), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
			else
			if (r == 5)
			{
				answer = 4;
				g.FillRectangle(new SolidBrush(Color.Olive), new Rectangle(272, 302, w + 46, w + 46));//Blue
			}
		}
		if (secondSenario)
		{
			//g.FillRectangle(pen, new Rectangle(500, 300, w + 50, w + 50));
			if (check1)
			{
				g.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(502, 302, w + 46, w + 46));
				if (check2)
				{
					g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(502, 302, w + 46, w + 46));//Red
					if (answer == 3)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.Olive), new Rectangle(502, 302, w + 46, w + 46));//Black
				else
				if (check4)
				{
					g.FillRectangle(new SolidBrush(Color.LightYellow), new Rectangle(502, 302, w + 46, w + 46));//White
					if (answer == 2)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(502, 302, w + 46, w + 46));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(502, 302, w + 46, w + 46));//Blue
			}
			if (check2)
			{
				g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(502, 302, w + 46, w + 46));
				if (check1)
				{
					g.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(502, 302, w + 46, w + 46));//yellow
					if (answer == 3)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check3)
				{
					g.FillRectangle(new SolidBrush(Color.DarkRed), new Rectangle(502, 302, w + 46, w + 46));//Black
					if (answer == 4)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.PaleVioletRed), new Rectangle(502, 302, w + 46, w + 46));//white
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(502, 302, w + 46, w + 46));//green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.Purple), new Rectangle(502, 302, w + 46, w + 46));//Blue
				
			}
			if (check3)
			{
				g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(502, 302, w + 46, w + 46));
				if (check1)
				{
					g.FillRectangle(new SolidBrush(Color.Olive), new Rectangle(502, 302, w + 46, w + 46));//yellow
					if (answer == 5)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.DarkRed), new Rectangle(502, 302, w + 46, w + 46));//Red
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(502, 302, w + 46, w + 46));//White
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.DarkGreen), new Rectangle(502, 302, w + 46, w + 46));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(502, 302, w + 46, w + 46));//Blue
			}
			if (check4)
			{
				g.FillRectangle(new SolidBrush(Color.White), new Rectangle(502, 302, w + 46, w + 46));
				if (check1)
				{
					g.FillRectangle(new SolidBrush(Color.LightYellow), new Rectangle(502, 302, w + 46, w + 46));//yellow
					if (answer == 2)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.PaleVioletRed), new Rectangle(502, 302, w + 46, w + 46));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(502, 302, w + 46, w + 46));//Black
				else
				if (check5)
					g.FillRectangle(new SolidBrush(Color.LightGreen), new Rectangle(502, 302, w + 46, w + 46));//Green
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(502, 302, w + 46, w + 46));//Blue
			}
			if (check5)
			{
				g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(502, 302, w + 46, w + 46));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(502, 302, w + 46, w + 46));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(502, 302, w + 46, w + 46));//Red
				else
				if (check3)
					g.FillRectangle(new SolidBrush(Color.DarkGreen), new Rectangle(502, 302, w + 46, w + 46));//Black
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.LightGreen), new Rectangle(502, 302, w + 46, w + 46));//White
				else
				if (check6)
					g.FillRectangle(new SolidBrush(Color.PowderBlue), new Rectangle(502, 302, w + 46, w + 46));//Blue
			}
			if (check6)//powder blue
            {
                g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(502, 302, w + 46, w + 46));
				if (check1)
					g.FillRectangle(new SolidBrush(Color.YellowGreen), new Rectangle(502, 302, w + 46, w + 46));//yellow
				else
				if (check2)
					g.FillRectangle(new SolidBrush(Color.DarkOliveGreen), new Rectangle(502, 302, w + 46, w + 46));//Red
				else
				if (check3)
				{
					g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(502, 302, w + 46, w + 46));//Black
					if (answer == 1)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
				else
				if (check4)
					g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(502, 302, w + 46, w + 46));//White
				else
				if (check5)
				{
					g.FillRectangle(new SolidBrush(Color.PowderBlue), new Rectangle(502, 302, w + 46, w + 46));//Red
					if (answer == 0)
						g.DrawString("Congratulations", drawFont, Brushes.Red, 230, 200, drawFormat);
				}
            }
		}

	}

	private void InitializeComponent()
	{
		this.SuspendLayout();
		// 
		// TuioDemo
		// 
		this.ClientSize = new System.Drawing.Size(284, 261);
		this.Name = "TuioDemo";
		this.Load += new System.EventHandler(this.TuioDemo_Load);
		this.ResumeLayout(false);

	}
	public int port = 3333;
	public TcpClient tcpClient;
	public NetworkStream stream;
	private void TuioDemo_Load(object sender, EventArgs e)
	{
		
	}

	public static void Main(String[] argv)
	{
		int port = 3333;
		switch (argv.Length)
		{
			case 1:
				port = int.Parse(argv[0], null);
				if (port == 0) goto default;
				break;
			case 0:
				port = 3333;
				break;
			default:
				Console.WriteLine("usage: java TuioDemo [port]");
				System.Environment.Exit(0);
				break;
		}

		TuioDemo app = new TuioDemo(port);
		Application.Run(app);
	}
}