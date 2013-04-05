// YomParse.cs
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
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreUtilities
{
	/// <summary>
	/// A wrapper (only partially complete) to abstract the keywords and such
	/// that I use for my parsing language I developed
	/// </summary>
	static public class YomParse
	{
		// constants
		const int KEYWORDS = 3;
		// types
		enum KeywordTypes  { SCENE, CENTER_TEXT, LEFT_TEXT };
		// Variables
		static public string prefix = "[[";
		static public string postfix = "]]";
		static public int[] system_id = new int[KEYWORDS] { (int)KeywordTypes.SCENE, 
			(int)KeywordTypes.CENTER_TEXT, 
			(int)KeywordTypes.LEFT_TEXT };
		static public string[] system_keywords = new string[KEYWORDS] { "~scene", "~center", "~left" };
		
		
		static public string KEYWORD_SCENE  { get {  return system_keywords[(int)KeywordTypes.SCENE];  }  }
		
		/// <summary>
		/// returns TRUE if sSourceLine contains a reference to the specified keyword
		/// </summary>
		/// <param name="sSourceLine"></param>
		/// <returns></returns>
		static public bool KeywordMatch(string sSourceLine, string Keyword)
		{
			if (sSourceLine.IndexOf(Keyword) > -1)
			{
				// keyword existed on line
				return true;
			}
			return false;
		}
		
		// Methods
		/// <summary>
		/// returns true if the supplies string has any of the system_keywords in it.
		/// </summary>
		/// <param name="sString"></param>
		/// <returns></returns>
		static public bool StringContainsASystemKeyword(string sString)
		{
			foreach (string s in system_keywords)
			{
				if (sString.IndexOf(s) > -1)
				{
					return true;
				}
			}
			return false;
		}
		
	}
}
