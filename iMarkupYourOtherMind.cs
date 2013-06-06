// iMarkupYourOtherMind.cs
//
// Copyright (c) 2013 Brent Knowles (http://www.brentknowles.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Review documentation at http://www.yourothermind.com for updated implementation notes, license updates
// or other general information/
// 
// Author information available at http://www.brentknowles.com or http://www.amazon.com/Brent-Knowles/e/B0035WW7OW
// Full source code: https://github.com/BrentKnowles/YourOtherMind
//###
//
// iMarkupYourOtherMind.cs
//
// Authors:
//  Brent Knowles <writer@brentknowles.com>
//
// Copyright (C) 2013 Brent Knowles
// [LICENSE MUST BE REDONE]
//
using System;
using Layout;
using CoreUtilities;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace YourOtherMind
{
	public class iMarkupYourOtherMind : iMarkupLanguage
	{
		public iMarkupYourOtherMind ()
		{
		}
		public override string ToString ()
		{
			return nameAndIdentifier;
		}
		private string nameAndIdentifier= Loc.Instance.GetString("YourOtherMind");
		
		public string NameAndIdentifier {
			get {
				return nameAndIdentifier;
			}
			set {
				nameAndIdentifier = value;
			}
		}

		public string CleanWordRequest(string incoming)
		{
			string sLine = incoming.Replace ("[[words]]", "").Trim ();
			return sLine;
		}
		public bool IsWordRequest (string incoming)
		{
			bool result = false;
			if (incoming.IndexOf ("[[words]]") > -1) {
				result = true;
			}
			return result;
		}

		public bool IsOver (string incoming)
		{
			bool result = false;
			if (incoming.IndexOf ("[[end") > -1) {
				result = true;
			}
			return result;
		}
		public bool IsGroupRequest (string incoming)
		{
			bool result = false;
			if (incoming.IndexOf ("[[Group") > -1) {
				result = true;
			}
			return result;
		}

		public bool IsIndex (string incoming)
		{
			bool result = false;
			if (  "[[index]]" == incoming) {
				result = true;
			}
			return result;
		}

		void PaintLink (PaintEventArgs e, int Start, int End, RichTextBox RichText)
		{
			Graphics g;
			
			g = RichText.CreateGraphics ();
			//Pen myPen = null;// new Pen (Color.FromArgb (60, Color.Yellow)); // Alpha did not seem to work this way
			
			
			// this gets tricky. The loop is just to find all the [[~scenes]] on the note
			// even if offscreen.
			// OK: but with th eoptimization to only show on screen stuff, do we need this anymore???
			// august 10 - settting it back
			
			
			// now trying regex
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex ("\\w\\|\\w",
			                                                                                       System.Text.RegularExpressions.RegexOptions.IgnoreCase |
			                                                                                       System.Text.RegularExpressions.RegexOptions.Multiline);
			System.Text.RegularExpressions.MatchCollection matches = regex.Matches (RichText.Text, Start);
			
			foreach (System.Text.RegularExpressions.Match match in matches) {
				if (match.Index > End)
					break; // we exit if already at end
				
				Point pos = RichText.GetPositionFromCharIndex (match.Index);
				
				if (pos.X > -1 && pos.Y > -1) {
			
					
					int testpos = match.Index + 2;
					


						
					Color colorToUse = Color.FromArgb(255, ControlPaint.Dark (TextUtils.InvertColor(RichText.BackColor)));
						
						// default is [[facts]] and stuff
						pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Bottom, "|");
						System.Drawing.SolidBrush brush1 = new System.Drawing.SolidBrush( colorToUse);
						//myPen.Width = 1;
						//myPen.Color = Color.Red;
						int scaler = Math.Max (10, (int)(Math.Round (RichText.ZoomFactor / 2) * 10));



						Rectangle rec = new Rectangle (new Point (pos.X, pos.Y - 25), new Size ((int)(scaler * 1.5), (int)(scaler * 0.75)));
					    Rectangle rec2 = new Rectangle (new Point (pos.X+20, pos.Y - 25), new Size ((int)(scaler * 1), (int)(scaler * 0.65)));
						//g.DrawLine(myPen, pos.X, pos.Y -10, pos.X + 50, pos.Y-10);
						//g.FillRectangle (brush1, rec);
						g.FillEllipse(brush1, rec);
					g.FillEllipse(brush1, rec2);
						
						
					}
					
					
					/*
                            locationInText = locationInText + sParam.Length;

                            if (locationInText > end)
                            {
                                // don't search past visible end
                                pos = emptyPoint;
                            }
                            else
                                pos = GetPositionForFoundText(sParam, ref locationInText);*/
				}
			 // regex matches
			g.Dispose ();
			//myPen.Dispose ();
		}

		int BuildScaler (float zoomFactor)
		{
			return Math.Max (10, (int)(Math.Round (zoomFactor / 2) * 10));
		}

		void PaintDoubleSpaces (PaintEventArgs e, int Start, int End, RichTextBox RichText)
		{
			Graphics g;
			
			g = RichText.CreateGraphics ();
			//Pen myPen = null;// new Pen (Color.FromArgb (60, Color.Yellow)); // Alpha did not seem to work this way
			
			
			// this gets tricky. The loop is just to find all the [[~scenes]] on the note
			// even if offscreen.
			// OK: but with th eoptimization to only show on screen stuff, do we need this anymore???
			// august 10 - settting it back
			
			
			// now trying regex
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex ("\\w  \\w",
               System.Text.RegularExpressions.RegexOptions.IgnoreCase | RegexOptions.Compiled|
               System.Text.RegularExpressions.RegexOptions.Multiline);
			System.Text.RegularExpressions.MatchCollection matches = regex.Matches (RichText.Text, Start);
			
			foreach (System.Text.RegularExpressions.Match match in matches) {
				if (match.Index > End)
					break; // we exit if already at end
				
				Point pos = RichText.GetPositionFromCharIndex (match.Index);
				
				if (pos.X > -1 && pos.Y > -1) {
					
					
					int testpos = match.Index + 2;
					
					
					
					
					Color colorToUse = Color.FromArgb(175, Color.Red);
					
					// default is [[facts]] and stuff
				//	pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Bottom, "  ");
				//	System.Drawing.SolidBrush brush1 = new System.Drawing.SolidBrush( colorToUse);
					System.Drawing.Pen pen1 = new Pen(colorToUse);
					//myPen.Width = 1;
					//myPen.Color = Color.Red;
			//		int scaler = BuildScaler(RichText.ZoomFactor);
					
					Rectangle rec = GetRectangleForSmallRectangle(g, pos, RichText, match.Index, match.Value, true);// new Rectangle (new Point (pos.X+(int)(scaler*1.5), pos.Y -(5+scaler)), new Size ((int)(scaler * 1.5), (int)(scaler * 1.5)));

					
					//Rectangle rec = new Rectangle (new Point (pos.X+scaler, pos.Y-15), new Size ((int)(scaler * 1.5), (int)(scaler * 0.75)));
				//	Rectangle rec2 = new Rectangle (new Point (pos.X+20, pos.Y - 25), new Size ((int)(scaler * 1), (int)(scaler * 0.65)));
					//g.DrawLine(myPen, pos.X, pos.Y -10, pos.X + 50, pos.Y-10);
					//g.FillRectangle (brush1, rec);
					//g.FillEllipse(brush1, rec);
					rec.Height = 1;
			
					g.DrawRectangle(pen1, rec);
				//	g.FillEllipse(brush1, rec2);
					
				
				}
				
				
				/*
                            locationInText = locationInText + sParam.Length;

                            if (locationInText > end)
                            {
                                // don't search past visible end
                                pos = emptyPoint;
                            }
                            else
                                pos = GetPositionForFoundText(sParam, ref locationInText);*/
			}
			// regex matches
			g.Dispose ();
			//myPen.Dispose ();
		}

		void PaintHeadings (PaintEventArgs e, int Start, int End, RichTextBox RichText, System.Text.RegularExpressions.Regex regex2, Color color, string text, Font f)
		{
			//string exp = ".*\\=([^)]+?)\\=";
		
			// this one was fast but inaccurate (very)
			//string exp = String.Format (".*?\\=\\w([^)]+?)\\w\\={0}?", Environment.NewLine); // added lazy operations to attempt a speed improvement
				Graphics g = e.Graphics;//RichText.CreateGraphics ();
		
			System.Drawing.Pen pen1 = new Pen(color);

			System.Text.RegularExpressions.MatchCollection matches = regex2.Matches (RichText.Text, Start);
			
			foreach (System.Text.RegularExpressions.Match match in matches) {
				if (match.Index > End)
					break; // we exit if already at end

				Point pos = RichText.GetPositionFromCharIndex (match.Index);
				
				if (pos.X > -1 && pos.Y > -1) {
				
					pen1.Width = 8;
					string stext = text+match.Value+text;
					//pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Right,match.Value );
					int newWidth = Convert.ToInt32 (g.MeasureString (stext, f).Width * RichText.ZoomFactor);
					pos =  new Point(pos.X + (newWidth), pos.Y + 10);
					g.DrawLine (pen1, pos.X, pos.Y, pos.X + 500, pos.Y);

//					Rectangle rec = GetRectangleForSmallRectangle(g, pos, RichText, match.Index, match.Value, true);// new Rectangle (new Point (pos.X+(int)(scaler*1.5), pos.Y -(5+scaler)), new Size ((int)(scaler * 1.5), (int)(scaler * 1.5)));
//				rec.Height = 2;
//				
//				g.DrawRectangle(pen1, rec);
				}
			}
		//	g.Dispose (); don't dispose what we are borrowing
		}
		System.Text.RegularExpressions.Regex Mainheading = new System.Text.RegularExpressions.Regex ("^=[^=]+=$",
                 RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled| System.Text.RegularExpressions.RegexOptions.Multiline );
		System.Text.RegularExpressions.Regex Mainheading2 = new System.Text.RegularExpressions.Regex ("^==[^=]+==$",
		                                                                                             RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled| System.Text.RegularExpressions.RegexOptions.Multiline );
		System.Text.RegularExpressions.Regex Mainheading3 = new System.Text.RegularExpressions.Regex ("^===[^=]+===$",
		                                                                                              RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled| System.Text.RegularExpressions.RegexOptions.Multiline );


		public void DoPaint (PaintEventArgs e, int Start, int End, RichTextBox RichText)
		{
			try {
				PaintDoubleSpaces(e, Start, End, RichText);
				PaintLink(e, Start, End, RichText);

				bool doheadings = true;
				if (doheadings)
				{
					Font f = new Font(RichText.Font.FontFamily, RichText.Font.Size, RichText.Font.Style, GraphicsUnit.Pixel, Convert.ToByte(0), false);
					Color newColor = TextUtils.InvertColor(RichText.BackColor);


					Color colorToUse = Color.FromArgb(175, ControlPaint.Dark(newColor));
					PaintHeadings(e, Start, End, RichText, Mainheading, colorToUse,"=",f);

					colorToUse = Color.FromArgb(175,ControlPaint.Light(newColor));
					PaintHeadings(e, Start, End, RichText, Mainheading2, colorToUse,"==",f);

					colorToUse = Color.FromArgb(175, ControlPaint.LightLight(newColor));
					PaintHeadings(e, Start, End, RichText, Mainheading3, colorToUse,"===",f);
				}
				//  int locationInText = GetCharIndexFromPosition(new Point(0, 0));
				string sParam = "[[~scene]]";
				
				
				// assuming function only being used for [[~scene]] delimiters
				
				// I've moved this position up to core function as an optimization
				// instead of being used in each call to GetPositionForFoundText
				//    int start = 
				//    if (locationInText < start)
				//    {
				//        locationInText = start;
				//    }
				
				// The loop is what makes this 
				
				// Point pos = GetPositionForFoundText(sParam, ref locationInText);
				Graphics g;

				//g = RichText.CreateGraphics ();
				g = e.Graphics;
				Pen myPen = new Pen (Color.FromArgb (255, ControlPaint.LightLight (TextUtils.InvertColor(RichText.BackColor)))); 
				
				
				// this gets tricky. The loop is just to find all the [[~scenes]] on the note
				// even if offscreen.
				// OK: but with th eoptimization to only show on screen stuff, do we need this anymore???
				// august 10 - settting it back
				
				
				// now trying regex
				// This worked for = match but it was WAAY too slow
//				System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex (".*\\=([^)]+)\\=",
//				                                                                   System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			

				// this works just experimenting
				System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex ("\\[\\[(.*?)\\]\\]",
				                                                                                      System.Text.RegularExpressions.RegexOptions.IgnoreCase |
					System.Text.RegularExpressions.RegexOptions.Multiline);
				System.Text.RegularExpressions.MatchCollection matches = regex.Matches (RichText.Text, Start);
				
				foreach (System.Text.RegularExpressions.Match match in matches) {
					if (match.Index > End)
						break; // we exit if already at end

					Point pos = RichText.GetPositionFromCharIndex (match.Index);
					
					if (pos.X > -1 && pos.Y > -1) {
						// but we only draw the line IF we are onscreen
						//if (pos.X > -1 && pos.Y > -1)
						
						
						//if (sParam == "[[~scene]]")
						//test instead to see if pos + 2 is a ~
						
						int testpos = match.Index + 2;
						
						/// November 2012 - only want this to appear for ~scene
						if (RichText.Text.Length > (testpos + 1) && RichText.Text [testpos] == '~' && RichText.Text [testpos + 1] == 's') {
							
							if (g != null) {
								//myPen.Color = Color.Yellow;
							//	myPen = new Pen (Color.FromArgb (225, Color.Yellow));
								myPen.Width = 8;
								pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Right, sParam);
								g.DrawLine (myPen, pos.X, pos.Y, pos.X + 500, pos.Y);
							}
						} else {
							Color colorToUse = Color.FromArgb (255, ControlPaint.LightLight (TextUtils.InvertColor(RichText.BackColor)));//Color.FromArgb(255, Color.Green);
							
							// November 2012 - testing to see if there's a period right before which means it will count)
							if (match.Index > 0) {
								if ('.' == RichText.Text [match.Index - 1]) {
									// we have a period so the code .[[f]] will not do anything
									// show in a different color
									colorToUse = Color.Red;// Alpha worked but because not all areas are redrawn at same time did not look right (May 2013) Color.FromArgb(255, Color.Red);
								}
							}
							
							// default is [[facts]] and stuff
							//pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Bottom, sParam);
						//	if (e.Graphics. .Contains(pos))
							{
							//System.Drawing.SolidBrush brush1 = new System.Drawing.SolidBrush (colorToUse);
								System.Drawing.Pen pen1 = new System.Drawing.Pen (colorToUse);
							//myPen.Width = 1;
							//myPen.Color = Color.Red;


							//int scaler = BuildScaler(RichText.ZoomFactor);
							Rectangle rec = GetRectangleForSmallRectangle(g, pos, RichText, match.Index, match.Value, false);// new Rectangle (new Point (pos.X+(int)(scaler*1.5), pos.Y -(5+scaler)), new Size ((int)(scaler * 1.5), (int)(scaler * 1.5)));
							//g.DrawLine(myPen, pos.X, pos.Y -10, pos.X + 50, pos.Y-10);
//							SolidBrush brushEmpty = new SolidBrush(RichText.BackColor);
//							g.FillRectangle (brushEmpty, rec);
							g.DrawRectangle (pen1, rec);
							
							}
						}
						
						
						/*
                            locationInText = locationInText + sParam.Length;

                            if (locationInText > end)
                            {
                                // don't search past visible end
                                pos = emptyPoint;
                            }
                            else
                                pos = GetPositionForFoundText(sParam, ref locationInText);*/
					}
				} // regex matches

				// don't dispose of this, since it is used elsewhere (i.e., we did not create it)
			//	g.Dispose ();
				myPen.Dispose ();
			} catch (Exception ex) {
				NewMessage.Show (String.Format ("Failed in WRITER part Start {0} End {1}", Start, End) + ex.ToString ());
			}
		}
		// used for starting a rectangle or line from the point of a match
		Point GetBottomLeftCornerOfMatch(Point passedPos, RichTextBox box)
		{
			Point newPoint = new Point ( (int)(passedPos.X + box.ZoomFactor), (int)(passedPos.Y - box.ZoomFactor));
			return newPoint;
		}
//		Rectangle GetRectangleForSmallRectangle (Graphics g, Point passedPos, RichTextBox box, int textposition, string text, bool drawFromBottom)
//		{
//			return GetRectangleForSmallRectangle(g, passedPos, box, textposition, text, drawFromBottom,Constants.BLANK);
//		}
		// returns a rectangle suitable for a small rectangle covering the beginning of the matched text
		Rectangle GetRectangleForSmallRectangle (Graphics g, Point passedPos, RichTextBox box, int textposition, string text, bool drawFromBottom/*, string offsetx*/)
		{   
		
			int scaler = BuildScaler (box.ZoomFactor);
			Point newPos = GetBottomLeftCornerOfMatch (passedPos, box);
			//	int newX = ((int)(scaler * 1.5)) - scaler;
			//	int newY = (int)(scaler * 1.5);

			// default is ignored. We construct width and height based on size of text
			Size newSize = new Size (100, 100);

			using (Font f = new Font(box.Font.FontFamily, box.Font.Size, box.Font.Style, GraphicsUnit.Pixel, Convert.ToByte(0), false)) {

				//char ch = box.Text [textposition];
				//newSize.Width = Convert.ToInt32 (g.MeasureString (ch.ToString(), f).Width * box.ZoomFactor);
				newSize.Width = Convert.ToInt32 (g.MeasureString (text, f).Width * box.ZoomFactor);
				newSize.Height = Convert.ToInt32 (box.Font.Height * box.ZoomFactor);

//				// doh. We need to adjust yPosition to accomdate the way rectangles draw
				if (true == drawFromBottom) {
					newPos.Y = newPos.Y + newSize.Height;
				}
//				if (Constants.BLANK != offsetx) {
//					// by default we start at the first left
//					// but for certain things like headings we actually want to sh ift over towards the right a measure
//					newPos.X = newPos.X + (offsetx.Length * newSize.Width);
//				}
			}


			return new Rectangle(newPos, newSize);
		}
		public ArrayList GetListOfPages(string sLine, ref bool bGetWords)
		{

			ArrayList ListOfParsePages = new ArrayList();
			string[] items = sLine.Split(',');
			if (items != null)
			{
				string sStoryboardName = items[1];
				string sGroupMatch = items[2];
				if (sLine.IndexOf(",words") > -1)
				{
					bGetWords = true;
				}
				ListOfParsePages.AddRange(LayoutDetails.Instance.CurrentLayout.GetListOfGroupEmNameMatching(sStoryboardName, sGroupMatch));
				
				//NewMessage.Show(sStoryboardName + " " + sGroupMatch);
			}
			return ListOfParsePages;
		}
		/// <summary>
		/// Builds the list. for the bookmark system
		/// </summary>
		/// <returns>
		/// The list.
		/// </returns>
		/// <param name='RichText'>
		/// Rich text.
		/// </param>
		public List<TreeItem> BuildList (NoteDataXML_RichText RichText)
		{
			System.Text.RegularExpressions.Regex Mainheading = new System.Text.RegularExpressions.Regex ("^=[^=]+=$",
			                                                                                             RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
			System.Text.RegularExpressions.Regex Mainheading2 = new System.Text.RegularExpressions.Regex ("^==[^=]+==$",
			                                                                                              RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
			System.Text.RegularExpressions.Regex Mainheading3 = new System.Text.RegularExpressions.Regex ("^===[^=]+===$",
			                                                                                              RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
			
			
			//TODO: Move this into the MarkupLanguage
			List<TreeItem> items = new List<TreeItem> ();
			
			System.Text.RegularExpressions.MatchCollection matches = Mainheading.Matches (RichText.GetRichTextBox().Text, 0);
			
			foreach (System.Text.RegularExpressions.Match match in matches) {
				items.Add (new TreeItem(match.Value.Replace ("=",""), 0,match.Index));
			}
			matches = Mainheading2.Matches (RichText.GetRichTextBox().Text, 0);
			
			
			
			foreach (System.Text.RegularExpressions.Match match in matches) {
				items.Add (new TreeItem(match.Value.Replace ("=",""), 1,match.Index));
			}
			matches = Mainheading3.Matches (RichText.GetRichTextBox().Text, 0);
			foreach (System.Text.RegularExpressions.Match match in matches) {
				items.Add (new TreeItem(match.Value.Replace ("=",""), 2,match.Index));
			}
			//need to somehow merge and sort by index 
			// need to a custom fort
			items.Sort ();
			
			return items;
			
		}
	}
}

