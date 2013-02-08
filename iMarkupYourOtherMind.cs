using System;
using Layout;
using CoreUtilities;

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
	}
}

