// mef_Addin_YourothermindMarkup.cs
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

// Purpose: Adds the YourOtherMindMarkup + FindInNote + FactInNote

namespace MefAddIns
{
	using MefAddIns.Extensibility;
	using System.ComponentModel.Composition;
	using System;
	//using System.Windows.Forms;
	using CoreUtilities;
	using System.Collections;
	using System.IO;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using Layout;
	using HotKeys;
	using YourOtherMind;
	/// <summary>
	/// Provides an implementation of a supported language by implementing ISupportedLanguage. 
	/// Moreover it uses Export attribute to make it available thru MEF framework.
	/// </summary>
	[Export(typeof(mef_IBase))]
	public class Addin_YourothermindMarkup : PlugInBase, mef_IBase
	{
		#region variables

		
	#endregion
		public Addin_YourothermindMarkup()
		{
			guid = "yourothermindmarkup";
		}
		
		public string Author
		{
			get { return @"Brent Knowles"; }
		}
		public string Version
		{

			// 1.1.0.0 - putting in hooks to support Fact Notes parsing "facts" on notes on OTHER layouts (Major)
			// 1.0.4 - detect no space after a period
			// 1.0.3 - holding a reference list when doing Fact List to speed it up
			// 1.0.2 - able to "search" within SubPanels, not just Groups on Storyboards
			get { return @"1.1.1.0"; }
		}
		public string Description
		{
			get { return Loc.Instance.GetString ("Allows Yourothermind markup to appear in text boxes as well as giving access to multi-note search tools and factfinders."); }
		}
		public string Name
		{
			get { return @"YOMMarkup"; }
		}
		

		public override bool DeregisterType ()
		{

			LayoutDetails.Instance.RemoveMarkupFromList(typeof(iMarkupYourOtherMind));
			return true;
			//Layout.LayoutDetails.Instance.AddToList(typeof(NoteDataXML_Picture.NoteDataXML_Pictures), "Picture");
		}
		
//		public override int TypeOfInformationNeeded {
//			get {
//				return (int)GetInformationADDINS.GET_CURRENT_LAYOUT_PANEL;
//			}
//		}
//		public override void SetBeforeRespondInformation (object neededInfo)
//		{
//			CurrentPanel = (LayoutPanel)neededInfo;
//		}
		
//		mef_IBase myAddInOnMainFormForHotKeys = null;
//		Action<mef_IBase> myRunnForHotKeys=null;
//		public override void AssignHotkeys (ref List<HotKeys.KeyData> Hotkeys, ref mef_IBase addin, Action<mef_IBase> Runner)
//		{
//			
//			base.AssignHotkeys (ref Hotkeys, ref addin, Runner);
//			myAddInOnMainFormForHotKeys = addin;
//			myRunnForHotKeys=Runner;
//			Hotkeys.Add (new KeyData (Loc.Instance.GetString ("Picture Capture"), HotkeyAction, Keys.Control, Keys.P, Constants.BLANK, true, "pictureguid"));
//			
//		}
		
//		public void HotkeyAction(bool b)
//		{
//			if (myRunnForHotKeys != null && myAddInOnMainFormForHotKeys != null)
//				myRunnForHotKeys(myAddInOnMainFormForHotKeys);
//			
//		}
		public override void RegisterType()
		{
			LayoutDetails.Instance.AddMarkupToList(new iMarkupYourOtherMind());
			//NewMessage.Show ("Registering Picture");
			//Layout.LayoutDetails.Instance.AddToList(typeof(NoteDataXML_Picture.NoteDataXML_Pictures), "Picture");
		}
		public void RespondToMenuOrHotkey<T>(T form) where T: System.Windows.Forms.Form, MEF_Interfaces.iAccess 
		{
			NewMessage.Show ("Fact or Search! This would only appear if a menu item was hooked up");
			// do nothing. This is not called for mef_Inotes
			return;
		}
		public override string BuildFileNameForActionWithParam ()
		{
			return  System.IO.Path.Combine (System.IO.Path.GetTempPath (), Guid.NewGuid().ToString () + ".txt");
		}

		/// <summary>
		/// May 2012
		/// Pulling this functionality out to use elsewhere
		/// </summary>
		/// <param name="sLine"></param>
		/// <returns></returns>
		public static ArrayList GetListOfPages(string sLine, ref bool bGetWords, LayoutPanelBase usedLayout)
		{
			//NewMessage.Show (LayoutDetails.Instance.GetCurrentMarkup().ToString ());
			return LayoutDetails.Instance.GetCurrentMarkup().GetListOfPages(sLine, ref bGetWords, usedLayout);
		

		}

		static void HandleFaceForNote (NoteDataInterface note, ref Hashtable TheFacts, bool SearchMode, string SearchTerm)
		{
			try {
				RichTextBox tempBox = new RichTextBox ();
				tempBox.Rtf = note.Data1;
				//NewMessage.Show (tempBox.Rtf);
				if (SearchMode == true) {
					FactListMaker.GetSearchItems (tempBox.Text, TheFacts, note.GuidForNote + FactListMaker.SEP_INSIDEPHRASE + note.Caption, SearchTerm);
				} else {
					FactListMaker.GetFacts (tempBox.Text, TheFacts, note.GuidForNote + FactListMaker.SEP_INSIDEPHRASE + note.Caption);
				}

			//	NewMessage.Show (TheFacts.Count.ToString());

				tempBox.Dispose ();
			} catch (System.Exception ex) {
				NewMessage.Show (ex.ToString ());
			}

			//return TheFacts;
		}

		/// <summary>
		/// goes through an index-type note, sort of like the index for sendaway
		/// [[name of storyboard to send stuff to]]  -- will test if it exists
		/// [[bygroup or bytitle]] can group these differently dependng on desire
		/// [[]] third, final line, tells which notes from which storyboard to parse, just like sendaway
		/// </summary>
		/// <returns></returns>
		private static Hashtable CreateFactList(ref string DestinationStoryBoard, ref bool groupByGroup, LayoutPanelBase LayoutToUse, string[] LinesOfText)
		{
			//NewMessage.Show ("In YOM Markup");
			/*
			 * 28/06/2014 - generalizing so this can be used on other layouts than the Current
			 * */

			ArrayList ListOfParsePages = new ArrayList();
			if (LayoutToUse != null /* && LayoutDetails.Instance.CurrentLayout.CurrentTextNote != null*/)
			{
				//string sText = ((mdi)_CORE_GetActiveChild()).richText.Text; (dec20 2009 - this did not seem to be used)

				
				bool SearchMode = false; // if true we will actually search for all occurences of teh string
				string SearchTerm = ""; // if doing a search the word we want to search for
				
				try
				{
					
					if (LinesOfText[0].IndexOf("[[") > -1)
						//if (LinesOfText[0].ToLower() == "[[index]]")
					{
						
						// set storyboard where details will go.
						DestinationStoryBoard = LinesOfText[0];
						if (DestinationStoryBoard.ToLower().IndexOf("fact") == -1 && DestinationStoryBoard.ToLower ().IndexOf("search") == -1)
						{
							NewMessage.Show(Loc.Instance.GetString ("Fact Storyboards must have the word fact in them. Search Storyboards must have the word search. Quitting!"));
							return null;
						}
						if (DestinationStoryBoard.IndexOf("search") > -1)
						{
							SearchMode = true;
						}
						DestinationStoryBoard = DestinationStoryBoard.Replace("[[", "");
						DestinationStoryBoard = DestinationStoryBoard.Replace("]]", "");
						
						groupByGroup = true;
						// we are actually an index note
						// which will instead list a bunch of other pages to use
						// we now iterate through LinesOfText[1] to end and parse those instead
						for (int i = 1; i < LinesOfText.Length; i++)
						{
							string sLine = LinesOfText[i];
							
							
							
							// second line should be the 'type'
							if (1 == i)
							{
								if (true == SearchMode)
								{
									// grab the search term
									SearchTerm = sLine;
								}
								else
									if (sLine.ToLower() == "[[bychapter]]")
								{
									groupByGroup = false;
								}
							}
							else
								if (sLine.IndexOf("[[Group") > -1)
							{
								bool words = false; // not needed for this but required for function
								ArrayList tmp  = GetListOfPages(sLine, ref words, LayoutToUse);

//								if (ListOfParsePages.Count == 0)
//								{
//									NewMe
//								}
//								else
								if (tmp == null || tmp.Count == 0)
								{
									//NewMessage.Show (Loc.Instance.GetString("No pages found while looking for : " + sLine));
									//NewMessage.Show (Loc.Instance.GetString("Layout looking for was called : " + LayoutToUse.Caption + " and it has this many notes = " + LayoutToUse.CountNotes()));
								}
								else
								{
									ListOfParsePages.AddRange(tmp);
									ListOfParsePages.Sort();
								}
								
								
							}
							else
							{
								//NewMessage.Show ("Adding " + sLine);
								// if there's just a page name then we add it to the list of pages
								ListOfParsePages.Add(sLine);
							}
						}
					}
				}
				catch (Exception ex)
				{
					NewMessage.Show (ex.ToString());
				}
				
				Hashtable TheFacts = new Hashtable();


				// SEPTEMBER 2013 For future reference, you cannot simply skip searching subnotes
				// when doing a fact parse or something because it won't then find the child note
				// That is, in LayoutPanel I could parse in something to only search child notes, but then 
				// if what we want to search is in a particular subnote, it won't find that. So this is not an easy solution --
				// I won't just be able to trim subpanels outo f the search to speed it up.

				// September 16 2013 - trying to optimize speed of the Fact finding by holding a reference array
				ArrayList notes_tmp = new ArrayList();
				//NewMessage.Show ("1");
				// THIS REQUIRES parsing each FILE
				// getting hashtable full of facts
				foreach (string notetoopen in ListOfParsePages)
				{
					// now do the actual parse on each of these pages 
					// opening them and inspecting them
					//NewMessage.Show ("Looking for " + notetoopen);
					//NewMessage.Show ("looking for " + notetoopen);
					NoteDataInterface note = LayoutToUse.FindNoteByName(notetoopen, ref notes_tmp);
					//LayoutDetails.Instance.CurrentLayout.FindNoteByName(notetoopen);	
					//DrawingTest.NotePanel panel = ((mdi)_CORE_GetActiveChild()).page_Visual.GetPanelByName(notetoopen);
					if (note != null)		
					{

					//	NewMessage.Show ("Examining " + note.Caption);
						// june 2013 - moving code from LayoutDetails into here to search subpanels
						// if a panel is specified then we open each note on that panel?
						List<NoteDataInterface> subpages = note.ListOfSubnotesAsNotes();
						if (subpages != null)
						{
							subpages.Sort ();
							foreach (NoteDataInterface subnote in subpages)
							{
								HandleFaceForNote (subnote, ref TheFacts, SearchMode, SearchTerm);
							}
							/*September 16 2013- Major revision attempt 2-- just push an actual list of notes here
							// September 16 2013 - trying to optimize speed of the Fact finding by holding a reference array
							ArrayList notes_tmp_subnotes = new ArrayList();

							// may 27 2013 - also need to sort subpages coming from a panel
							

							subpages.Sort ();
							foreach (string s in subpages)
							{
								NoteDataInterface subnote = LayoutDetails.Instance.CurrentLayout.FindNoteByName(s, ref notes_tmp_subnotes);	
								//	subnote = LayoutDetails.Instance.CurrentLayout.GoToNote(subnote);
								if (null != subnote)
								{
								//	NewMessage.Show ("searching " +subnote.Caption + " for " + SearchTerm);
									HandleFaceForNote (subnote, ref TheFacts, SearchMode, SearchTerm);
								}
							}
							*/
						}
						else
						{
						//	NewMessage.Show ("handled");
						// need to use RichText box because we want plaintext version of text
							//RichTextBox tempBox;
							HandleFaceForNote (note, ref TheFacts, SearchMode, SearchTerm);
						}
						//tempBox.Dispose();
					} // null
					else
					{
						NewMessage.Show ("Note was null for " + notetoopen);
					}
				}
				
				
			//	NewMessage.Show("Facts count = " + TheFacts.Count.ToString());
				return TheFacts;

			}
			return null;
		}

		/// <summary>
		/// Gets the facts on remote note.
		/// 
		/// Intended to be called remotely.
		/// </summary>
		/// <returns>
		/// The facts on remote note.
		/// </returns>
		public static Hashtable GetFactsOnRemoteNote (LayoutPanelBase layout, string[] code)
		{
			//NewMessage.Show ("Hello there");

			bool groupByGroup = true;
			string DestinationStoryBoard = "";
			Hashtable Facts = CreateFactList(ref DestinationStoryBoard, ref groupByGroup, layout, code);
			//NewMessage.Show ("Fact count is " + Facts.Count.ToString());
			return Facts;
		}

		private void ParseFacts()
		{
			bool groupByGroup = true;
			string DestinationStoryBoard = "";
			string[] LinesOfText = LayoutDetails.Instance.CurrentLayout.CurrentTextNote.Lines();
			Hashtable Facts = CreateFactList(ref DestinationStoryBoard, ref groupByGroup, LayoutDetails.Instance.CurrentLayout, LinesOfText);
			if (null == Facts)
			{
				return;
			}
			// NOW BUILD the actual 
			// text required to populate the Data table
			FactListMaker.facts = 0; // reset fact counter

			if (LayoutDetails.Instance.CurrentLayout == null ) NewMessage.Show (Loc.Instance.GetString ("Please open a layout first."));

			// test if destination storyboard exists

			NoteDataXML_GroupEm Storyboard = (NoteDataXML_GroupEm)LayoutDetails.Instance.CurrentLayout.FindNoteByName(DestinationStoryBoard);
			Storyboard = (NoteDataXML_GroupEm)LayoutDetails.Instance.CurrentLayout.GoToNote(Storyboard);
			//DrawingTest.NotePanel storypanel = ((mdi)_CORE_GetActiveChild()).page_Visual.GetPanelByName(DestinationStoryBoard);
			if (Storyboard != null)
			{
				
				CoreUtilities.Links.LinkTable linktable = LayoutDetails.Instance.CurrentLayout.GetLinkTable ();
				if (linktable != null /*&& linktable.appearance.ShapeType == DrawingTest.Appearance.shapetype.Table*/)
				{
					// so we now have a storyboard
					if (NewMessage.Show(Loc.Instance.GetString ("Delete?"),Loc.Instance.GetStringFmt ("If you press yes the contents of the storyboard {0} will be deleted.", DestinationStoryBoard),
					                    MessageBoxButtons.YesNo, null) == DialogResult.Yes)
					{
						LayoutDetails.Instance.CurrentLayout.Cursor = Cursors.WaitCursor;
						//this.Cursor = Cursors.WaitCursor;
						Storyboard.DeleteRecordsForStoryboard();
						Storyboard.SetFactMode();
						foreach (DictionaryEntry gg in Facts)
						{
							//Console.WriteLine("Key  and  value are " + gg.Key + "   " + gg.Value);
						//	Console.Read();
							
							// break apart the 'information
							string[] factsforentry = gg.Value.ToString().Split(new string[1]{FactListMaker.SEP_PHRASES}, StringSplitOptions.RemoveEmptyEntries);
							if (factsforentry != null)
							{
								
								foreach (string s in factsforentry)
								{

									FactRecord factRecord = FactRecord.CreateFactRecord(s, false);
									// now we should have strings of format TItle;GUID
								//	string[] titleandguid = s.Split(new string[1] { FactListMaker.SEP_INSIDEPHRASE }, StringSplitOptions.None);
									
								//	fish|07|;Where in hell did she go?
								//		What|;5fee548d-2a2f-4f37-b912-01833f7ca198|;Chapter 2


									// 0 = position??
									// 1 = text near it
									// 2 = guid of note found
									// 3 = chapter
									if (factRecord != null)
									//if (titleandguid != null && titleandguid.Length == 4)
									{
										string sNum = factRecord.position;//titleandguid[0];
										string sItemToAdd = factRecord.text;//titleandguid[1];
										string sChapter = factRecord.chapter;//titleandguid[3];
										string group = "";
										if (true == groupByGroup)
										{
											group = gg.Key.ToString();
											sItemToAdd = sNum + ";"+sItemToAdd + "; " + sChapter;
										}
										else
										{
											// grab title
											group = sChapter; // the URL for now
											sItemToAdd = sNum + ";"+sItemToAdd + "; " + gg.Key.ToString();
											
										}
										sItemToAdd = sItemToAdd.Trim();
										
										Storyboard.AddRecordDirectly(sItemToAdd, factRecord.noteguid, group);
									}
									
								}
							}
							
							
							
							
							
						}
						
						Storyboard.RefreshGroupEm();
					}
					else
					{
						LayoutDetails.Instance.CurrentLayout.Cursor = Cursors.Default;
						return;
					}
				}
			}
			else
			{
				NewMessage.Show("That storyboard does not exist.");
				return;
			}
			
			LayoutDetails.Instance.CurrentLayout.Cursor = Cursors.Default;
		}

		// the param is the filename to the temp file
		public void ActionWithParamForNoteTextActions (object param)
		{
			LayoutDetails.Instance.CurrentLayout.SaveLayout();
			ParseFacts();
//			int CountNotes = 0;
//			if (LayoutDetails.Instance.CurrentLayout != null) {
//				CountNotes = LayoutDetails.Instance.CurrentLayout.CountNotes();
//			}
//
//			// will be used by this one
//			NewMessage.Show(String.Format ("Facts or Search: Note Count {0}, Filename {1}  ", CountNotes, param.ToString()));
		}
		public override string dependencymainapplicationversion { get { return "1.0.0.0"; }}
		
		//override string GUID{ get { return  "notedataxml_picture"; };
		public PlugInAction CalledFrom { 
			get
			{
				PlugInAction action = new PlugInAction();
				//	action.HotkeyNumber = -1;
				action.MyMenuName = "Parse Facts/Search";//Loc.Instance.GetString ("Screen Capture");
				action.ParentMenuName = "";//"NotesMenu";
				action.IsOnContextStrip = false;
				action.IsANote = false;
				action.IsOnAMenu = false;
				action.IsNoteAction = true;
				action.QuickLinkShows = false;
				action.ToolTip ="";//Loc.Instance.GetString("Allows images to be added to Layouts, as well as a Screen Capture tool.");
				//action.Image = FileUtils.GetImage_ForDLL("camera_add.png");
				action.GUID = GUID;
				return action;
			} 
		}
		
		
		
	}
}