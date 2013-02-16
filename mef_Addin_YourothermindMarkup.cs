
// Purpose: Adds the YourOtherMindMarkup + FindInNote + FactInNote

namespace MefAddIns
{
	using MefAddIns.Extensibility;
	using System.ComponentModel.Composition;
	using System;
	//using System.Windows.Forms;
	using CoreUtilities;
	using System.IO;
	using System.Collections.Generic;
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
			get { return "Allows Yourothermind markup to appear in text boxes as well as giving access to multi-note search tools and factfinders."; }
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
		public void RespondToCallToAction<T>(T form) where T: System.Windows.Forms.Form, MEF_Interfaces.iAccess 
		{
			NewMessage.Show ("Fact or Search! This would only appear if a menu item was hooked up");
			// do nothing. This is not called for mef_Inotes
			return;
		}
		public override string BuildFileName ()
		{
			return  System.IO.Path.Combine (System.IO.Path.GetTempPath (), Guid.NewGuid().ToString () + ".txt");
		}


		
		public void ActionWithParam (object param)
		{

			int CountNotes = 0;
			if (LayoutDetails.Instance.CurrentLayout != null) {
				CountNotes = LayoutDetails.Instance.CurrentLayout.CountNotes();
			}

			// will be used by this one
			NewMessage.Show(String.Format ("Facts or Search: Note Count {0}, Filename {1}  ", CountNotes, param.ToString()));
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