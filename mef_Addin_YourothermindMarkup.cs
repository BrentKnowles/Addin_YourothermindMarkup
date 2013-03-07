
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
			get { return @"1.0.0.0"; }
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
		public ArrayList GetListOfPages(string sLine, ref bool bGetWords)
		{
			return LayoutDetails.Instance.GetCurrentMarkup().GetListOfPages(sLine, ref bGetWords);
	
		}

		/// <summary>
		/// goes through an index-type note, sort of like the index for sendaway
		/// [[name of storyboard to send stuff to]]  -- will test if it exists
		/// [[bygroup or bytitle]] can group these differently dependng on desire
		/// [[]] third, final line, tells which notes from which storyboard to parse, just like sendaway
		/// </summary>
		/// <returns></returns>
		private Hashtable CreateFactList(ref string DestinationStoryBoard, ref bool groupByGroup)
		{
			ArrayList ListOfParsePages = new ArrayList();
			if (LayoutDetails.Instance.CurrentLayout != null && LayoutDetails.Instance.CurrentLayout.CurrentTextNote != null)
			{
				//string sText = ((mdi)_CORE_GetActiveChild()).richText.Text; (dec20 2009 - this did not seem to be used)
				string[] LinesOfText = LayoutDetails.Instance.CurrentLayout.CurrentTextNote.Lines();
				
				bool SearchMode = false; // if true we will actually search for all occurences of teh string
				string SearchTerm = ""; // if doing a search the word we want to search for
				
				try
				{
					
					if (LinesOfText[0].IndexOf("[[") > -1)
						//if (LinesOfText[0].ToLower() == "[[index]]")
					{
						
						// set storyboard where details will go.
						DestinationStoryBoard = LinesOfText[0];
						if (DestinationStoryBoard.IndexOf("fact") == -1 && DestinationStoryBoard.IndexOf("search") == -1)
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
								ListOfParsePages = GetListOfPages(sLine, ref words);
								ListOfParsePages.Sort();
								
								
							}
							else
							{
								// if there's just a page name then we add it to the list of pages
								ListOfParsePages.Add(sLine);
							}
						}
					}
				}
				catch (Exception)
				{
				}
				
				Hashtable TheFacts = new Hashtable();
				
				// THIS REQUIRES parsing each FILE
				// getting hashtable full of facts
				foreach (string notetoopen in ListOfParsePages)
				{
					// now do the actual parse on each of these pages 
					// opening them and inspecting them
					NoteDataInterface note = LayoutDetails.Instance.CurrentLayout.FindNoteByName(notetoopen);
					//DrawingTest.NotePanel panel = ((mdi)_CORE_GetActiveChild()).page_Visual.GetPanelByName(notetoopen);
					if (note != null)
					{
						// need to use RichText box because we want plaintext version of text
						RichTextBox tempBox = new RichTextBox();
						
						tempBox.Rtf = note.Data1;
						if (SearchMode == true)
						{
							FactListMaker.GetSearchItems(tempBox.Text, TheFacts, 
							                             note.GuidForNote + FactListMaker.SEP_INSIDEPHRASE + note.Caption, SearchTerm);
						}
						else
						{
							FactListMaker.GetFacts(tempBox.Text, TheFacts,  note.GuidForNote + FactListMaker.SEP_INSIDEPHRASE + note.Caption);
						}
						tempBox.Dispose();
					}
				}
				
				
				
				return TheFacts;
				
			}
			return null;
		}
		private void ParseFacts()
		{
			bool groupByGroup = true;
			string DestinationStoryBoard = "";
			Hashtable Facts = CreateFactList(ref DestinationStoryBoard, ref groupByGroup);
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
									// now we should have strings of format TItle;GUID
									string[] titleandguid = s.Split(new string[1] { FactListMaker.SEP_INSIDEPHRASE }, StringSplitOptions.None);
									
									
									if (titleandguid != null && titleandguid.Length == 4)
									{
										string sNum = titleandguid[0];
										string sItemToAdd = titleandguid[1];
										string sChapter = titleandguid[3];
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
										
										Storyboard.AddRecordDirectly(sItemToAdd, titleandguid[2], group);
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