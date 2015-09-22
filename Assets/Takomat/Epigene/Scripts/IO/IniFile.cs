//------------------------------------------------------------------------------
// Copyright (c) 2014-2015 takomat GmbH and/or its licensors.
// All Rights Reserved.

// The coded instructions, statements, computer programs, and/or related material
// (collectively the "Data") in these files contain unpublished information
// proprietary to takomat GmbH and/or its licensors, which is protected by
// German federal copyright law and by international treaties.

// The Data may not be disclosed or distributed to third parties, in whole or in
// part, without the prior written consent of takoamt GmbH ("takomat").

// THE DATA IS PROVIDED "AS IS" AND WITHOUT WARRANTY.
// ALL WARRANTIES ARE EXPRESSLY EXCLUDED AND DISCLAIMED. TAKOMAT MAKES NO
// WARRANTY OF ANY KIND WITH RESPECT TO THE DATA, EXPRESS, IMPLIED OR ARISING
// BY CUSTOM OR TRADE USAGE, AND DISCLAIMS ANY IMPLIED WARRANTIES OF TITLE,
// NON-INFRINGEMENT, MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE OR USE.
// WITHOUT LIMITING THE FOREGOING, TAKOMAT DOES NOT WARRANT THAT THE OPERATION
// OF THE DATA WILL gameengine_dialogsmanagerBE UNINTERRUPTED OR ERROR FREE.

// IN NO EVENT SHALL TAKOMAT, ITS AFFILIATES, LICENSORS BE LIABLE FOR ANY LOSSES,
// DAMAGES OR EXPENSES OF ANY KIND (INCLUDING WITHOUT LIMITATION PUNITIVE OR
// MULTIPLE DAMAGES OR OTHER SPECIAL, DIRECT, INDIRECT, EXEMPLARY, INCIDENTAL,
// LOSS OF PROFITS, REVENUE OR DATA, COST OF COVER OR CONSEQUENTIAL LOSSES
// OR DAMAGES OF ANY KIND), HOWEVER CAUSED, AND REGARDLESS
// OF THE THEORY OF LIABILITY, WHETHER DERIVED FROM CONTRACT, TORT
// (INCLUDING, BUT NOT LIMITED TO, NEGLIGENCE), OR OTHERWISE,
// ARISING OUT OF OR RELATING TO THE DATA OR ITS USE OR ANY OTHER PERFORMANCE,
// WHETHER OR NOT TAKOMAT HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH LOSS
// OR DAMAGE.
//------------------------------------------------------------------------------
// This class is part of the epigene(TM) Software Framework.
// All license issues, as above described, have to be negotiated with the
// takomat GmbH, Cologne.
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epigene.IO
{

	/// <summary>
	/// Internal class for hold values for each line in an Ini file.
	/// </summary>
	public class IniLine
	{
		public string section;
		public string key;
		public string value;

		public IniLine()
		{
			section = "";
			key = "";
			value = "";
		}

		public IniLine(string section, string key, string value)
		{
			this.section = section;
			this.key = key;
			this.value = value;
		}

		public override string ToString()
		{ 
			return string.Format("IniLine: section=\"{0}\" key=\"{1}\" value=\"{2}\"", section, key, value);
		}
	}

	/// <summary>
	/// Class to parse Ini file.
	/// </summary>
	public class IniFile 
	{
		/// <summary>
		/// Currenlty loaded file
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName
		{
			get {return fileName;}
		}
		private string fileName;

		/// <summary>
		/// The raw text data;
		/// </summary>
		private string text;

		/// <summary>
		/// The content 
		/// </summary>
		private List<IniLine> lines;


		/// <summary>
		/// Table of sections. Each sections holds numbers of IniLine objects.
		/// </summary>
		//protected Hashtable sections;


		/// <summary>
		/// Initializes a new instance of the <see cref="IniFile"/> class.
		/// </summary>
		public IniFile()
		{
			lines = new List<IniLine>();
			//sections = new Hashtable();

		}//ctor()

		/// <summary>
		/// Load the specified fileName from the Resource directory.
		/// </summary>
		/// <param name="fileName">File name and path realtive to Resource directory.</param>
		public virtual void Load(string fileName)
		{
			TextAsset txt = Resources.Load<TextAsset>(fileName);
			if(txt == null)
			{
				//Debug.LogWarning("no file:"+fileName);
				this.fileName = "";
				throw new System.Exception("No file to load:"+fileName);
			}
			else
			{
				Debug.Log("<color=yellow>Ini file</color> loaded: <color=yellow>"+fileName+"</color>");
				this.fileName = fileName;
				text = txt.ToString();
				parse();
			}

		}//LoadText()

		/// <summary>
		/// Parse this file. As a result, map will be set with keys/value pairs.
		/// </summary>
		protected virtual void parse()
		{

			//List<string> tmpSec = new List<string>();
			string currSection = "";
			lines.Clear();

			string[] txtLines = text.Split(new[]{ '\r', '\n'});
			foreach(string l in txtLines)
			{
				//skip empty lines or comments
				if(l != "" && !l.StartsWith(";") && !l.StartsWith("//") && !l.StartsWith("#"))
				{
					//Debug.Log ("->"+l);
					if(l.StartsWith("["))
					{
						//parse Section, remove [] and spaces
						currSection = l.Replace("[", "");
						currSection = currSection.Replace("]", "");
						currSection = Regex.Replace(currSection, @"\s+", "");
					}
					else if(l.Contains("="))
					{
						//parse key/value line
						string[] kv = l.Split('=');
						IniLine r = new IniLine();
						r.key = kv[0];
						r.value = kv[1].Replace("\"","");
						r.section = currSection;	//sections[sections.Count-1];
						lines.Add(r);

						Debug.Log (r.ToString());
					}
				}
			}
		}//parse()

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="IniFile"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="IniFile"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("IniFile: FileName=\"{0}\" Content=[{1}]", fileName, text);
		}//ToString()


	}//class IniFile
}