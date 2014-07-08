using System;
using CoreUtilities;

namespace MefAddIns
{
	public class FactRecord : object
	{

		//	fish|07|;Where in hell did she go?
		//		What|;5fee548d-2a2f-4f37-b912-01833f7ca198|;Chapter 2
		
		// 0 = position??
		// 1 = text near it
		// 2 = guid of note found
		// 3 = chapter

		public override string ToString ()
		{
			string result = "";
			try {
				result = string.Format ("theFact = {0}, text= {1}, chapter = {2}", theFact, text, chapter);
			} catch (System.Exception ex) {
				NewMessage.Show (ex.ToString ());
			}
			return result;
		}

		public FactRecord ()
		{
		}
		/// <summary>
		/// Creates the fact record.
		/// 
		/// If expanded is true then we know two additioanl items have been added -- the FACT name(i.e., fish for [[fish]]) and the layout guid (this is used for Fact notes)
		/// basic mode is used for the standard Fact parsing onto Storyboards
		/// </summary>
		/// <returns>
		/// The fact record.
		/// </returns>
		/// <param name='sourceString'>
		/// Source string.
		/// </param>
		/// <param name='Expanded'>
		/// If set to <c>true</c> expanded.
		/// </param>
		public static FactRecord CreateFactRecord (string sourceString, bool Expanded)
		{
			FactRecord result = null;

			string[] titleandguid = sourceString.Split (new string[1] { FactListMaker.SEP_INSIDEPHRASE }, StringSplitOptions.None);
			if (titleandguid != null && ((Expanded == false && titleandguid.Length == 4) || (Expanded == true && titleandguid.Length ==6))) {

				try
				{
				result = new FactRecord();
				//theFact = titleandguid[0];
				result.position = titleandguid[0];
				result.text = titleandguid[1];
				result.noteguid = titleandguid[2];
				result.chapter = titleandguid[3];


				if (Expanded)
				{
					result.theFact = titleandguid[4];
					result.layoutguid = titleandguid[5];
				}
				}
				catch (System.Exception ex){
					NewMessage.Show(ex.ToString ());
				}
			}
			return result;
		}
		public string theFact = "";
		public string position = "";
		public string text = "";
		
		public string chapter = "";

		// expanded details at end
		public string noteguid = "";
			public string layoutguid = "";
	}

}

