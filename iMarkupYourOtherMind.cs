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



		public void DoPaint (PaintEventArgs e, int Start, int End, RichTextBox RichText)
		{
			try {
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

				g = RichText.CreateGraphics ();
				Pen myPen = new Pen (Color.FromArgb (60, Color.Yellow)); // Alpha did not seem to work this way
				
				
				// this gets trick. The loop is just to find all the [[~scenes]] on the note
				// even if offscreen.
				// OK: but with th eoptimization to only show on screen stuff, do we need this anymore???
				// august 10 - settting it back
				
				
				// now trying regex
				System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex ("\\[\\[",
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
								myPen.Color = Color.Yellow;
								myPen.Width = 8;
								pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Right, sParam);
								g.DrawLine (myPen, pos.X, pos.Y, pos.X + 500, pos.Y);
							}
						} else {
							Color colorToUse = Color.Green;
							
							// November 2012 - testing to see if there's a period right before which means it will count)
							if (match.Index > 0) {
								if ('.' == RichText.Text [match.Index - 1]) {
									// we have a period so the code .[[f]] will not do anything
									// show in a different color
									colorToUse = Color.Red;
								}
							}
							
							// default is [[facts]] and stuff
							pos = CoreUtilities.General.BuildLocationForOverlayText (pos, DockStyle.Bottom, sParam);
							System.Drawing.SolidBrush brush1 = new System.Drawing.SolidBrush (colorToUse);
							//myPen.Width = 1;
							//myPen.Color = Color.Red;
							int scaler = Math.Max (10, (int)(Math.Round (RichText.ZoomFactor / 2) * 10));
							Rectangle rec = new Rectangle (new Point (pos.X, pos.Y - 25), new Size ((int)(scaler * 1.5), (int)(scaler * 0.75)));
							//g.DrawLine(myPen, pos.X, pos.Y -10, pos.X + 50, pos.Y-10);
							g.FillRectangle (brush1, rec);
							
							
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
				g.Dispose ();
				myPen.Dispose ();
			} catch (Exception ex) {
				NewMessage.Show (String.Format ("Failed in WRITER part Start {0} End {1}", Start, End) + ex.ToString ());
			}
		}

	}
}

