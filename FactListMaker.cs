using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CoreUtilities;

namespace MefAddIns
{
	/// <summary>
	/// This handsome little class will take a bit of code
	/// from SendTextAway but for the purpose of 
	/// building (eventually) a dynamic list of facts on a story board
	/// 
	/// Step #1 -
	///   Just get a list of facts in the form
	///  Ghouls are green[[ghouls]]. Roses are red[[flowers]]. Ghouls eat brains[[ghouls]]. 
	///
	/// </summary>
	public static class FactListMaker
	{
		public static int facts = 0; // count all the facts we add so we can pre numbers to them (may 23 2012)
		public const string SEP_PHRASES = "|| ";
		public const string SEP_INSIDEPHRASE = "|;";
		
		
		
		public static Hashtable GetSearchItems(string sourceText, Hashtable Facts, string ExtraInfoToEncode, string SearchTerm)
		{
			if (sourceText.IndexOf(SearchTerm) > -1)
			{
				string group = "Found";
				
				string add_me = SearchTerm + SEP_INSIDEPHRASE + "" + SEP_INSIDEPHRASE + ExtraInfoToEncode;
				
				// we use SearchTerm as the actually search item
				if (Facts.ContainsKey(group))
				{
					
					
					
					// add to string list
					string sList = Facts[group].ToString();
					// may 30 2012 - changed the , to a |,
					sList = sList + SEP_PHRASES + add_me;
					Facts[group] = sList;
				}
				else
				{
					// start a tally 
					Facts.Add(group, add_me);
					
				}
			}
			return Facts;
			
		}
		
		/// <summary>
		/// Returns an array of all the found facts
		/// 
		/// USAGE: hashtable = GetFacts(richText.Text);
		/// 
		/// Then another routine will take that hashtable and build an on-the-fly
		/// storyboard out of it.
		/// 
		/// Ideally clicking the links on the storyboard will take us to the place in the appropriate
		/// FILE --> which will require more work (add GUID to string somehow??)
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="Facts">Assumes Facts has already been prepared</param>
		/// <param name="ExtraInfoToEncode">If we need to store extra information we do so this weay</param>
		/// <returns></returns>
		public static Hashtable GetFacts(string sourceText, Hashtable Facts, string ExtraInfoToEncode)
		{
			
			int nFirstBracket = sourceText.IndexOf("[["); // in case on line with other brackets
			
			int end_index = 0; // this will be changed during the iteration to be the end of the last note found
			
			while (nFirstBracket > -1)
			{
				if (end_index < sourceText.Length)
				{
					
					
					// may 13 2012
					// alternative start point
					// instead of grabbing frm very beginning, just grab from end of last sentence?
					
					
					// THIS WORKED but woudl grab the end of the first sentence after end_index but 
					// I'd like to sang the end of the nearest
					//int alternative_start = sourceText.IndexOf('.', end_index, nFirstBracket - end_index);
					
					
					int alternative_start = sourceText.LastIndexOf('.', nFirstBracket);
					// we don't want to to go back further than the last 'end'
					if (alternative_start > -1 && alternative_start < nFirstBracket && alternative_start > end_index)
					{
						end_index = alternative_start;
					}
					
					string sText = sourceText.Substring(end_index, nFirstBracket - end_index);
					
					
					try
					{
						end_index = sourceText.IndexOf("]]", end_index + 1);// Was in the wrong place,nFirstBracket); // in case on line with other brackets
					}
					catch (Exception ex)
					{
						NewMessage.Show(ex.ToString());
					}
					if (sText != "")
					{
						// this is the group, i.e., 'ghouls'
						string sCode = "";
						try
						{
							sCode = sourceText.Substring(nFirstBracket + 2, end_index - (nFirstBracket + 2));
						}
						catch (Exception)
						{
							
							/*   return?;
                             // we have code messing things up, skip ahead
                             end_index = end_index + 1;
                             sCode = "~scene"; // bug -fix, turn malformed codes into 'skips'*/
						}
						
						
						if ( /*sCode.IndexOf("~scene") == -1 && sCode.IndexOf("~center") == -1*/
						    YomParse.StringContainsASystemKeyword(sCode) == false
						    && sCode != "title")
						{
							
							/*
                             //bug: sText is 'getting' more data than it should. Makes no logical sense
                             // HACK: Just going to trim it a second time. No reason why I should have to do this though
                             int nHackBracketsShouldNotExists = sText.IndexOf("[[");
                             if (nHackBracketsShouldNotExists > -1)
                             {
                                 sText = sText.Substring(0, nHackBracketsShouldNotExists);
                             }
                             // END HACK
                             */
							
							
							// basic format is
							// Entry;GUID, Entry2;GUID
							if (ExtraInfoToEncode != "")
							{
								sText = sText + SEP_INSIDEPHRASE + ExtraInfoToEncode;
							}
							
							
							// string cleanup
							sText = sText.Replace("]]", "");
							
							
							
							if (sText.StartsWith("."))
							{
								// trim starting period
								sText = sText.TrimStart(new char[1] { '.' });
							}
							
							
							facts++;
							string sExtraZero = "";
							if (facts < 10) sExtraZero = "0";
							
							// may 12 2012 adding the # of the fact here.
							sText = sExtraZero + facts.ToString() + SEP_INSIDEPHRASE + sText.Trim();
							
							if (Facts.ContainsKey(sCode))
							{
								
								
								
								// add to string list
								string sList = Facts[sCode].ToString();
								// may 30 2012 - changed the , to a |,
								sList = sList + SEP_PHRASES + sText;
								Facts[sCode] = sList;
							}
							else
							{
								// start a tally 
								Facts.Add(sCode, sText);
								
							}
						} // we don't add ~scene or title
						
						// NOW REMOVE the found text + the code
						// NOPE: We are just using end index to help us here
						// sourceText = sourceText.Replace(sText + "[[" + sCode +"]]", "");
						
					}
					nFirstBracket = sourceText.IndexOf("[[", end_index);
				}
				else
				{
					nFirstBracket = -1;
				}
			}
			
			
			
			
			return Facts;
		}
	}
}
