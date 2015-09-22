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

using Epigene;

namespace Epigene.UI
{
	/// <summary>
	/// Localization manager
	/// Internationalization = I18N
	/// This clas will load the texts from "localistaion" text file into memory.
	/// It will only holds the texts which are selected by the SetLanguage() and 
	/// make sure only those kept in memory.
	/// </summary>
	public sealed class I18nManager
	{

		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static I18nManager Instance
		{
			get { return instance; }
		}
		private static readonly I18nManager instance = new I18nManager();

		/// <summary>
		/// name of the currently used section
		/// </summary>
		public string Section
		{
			get { return this.section;}
			set { this.section = value;}
		}
		private string section;

		/// <summary>
		/// current language
		/// </summary>
		public string Language
		{
			get { return language;}
			set { SetLanguage(value);}
		}
		private string language;

		/// <summary>
		/// dictionary holds all key data pair
		/// </summary>
		private Dictionary<string, string> dict;
		private List<string> langKeys;
		private string fileName = "";		

		/// <summary>
		/// Ctor, initialization.
		/// </summary>	
		I18nManager()
		{

			Log.Debug("I18nManager initialized");

			langKeys = new List<string>();
			dict = new Dictionary<string, string>();

		}//ctor

		/// <summary>
		/// Load db file.
		/// </summary>	
		public void LoadDbFile(string fileName)
		{
			Log.Debug("LoadDBFile:"+fileName);
			this.fileName = fileName;
			LoadLangKeys();
			LoadLang(langKeys[0]);

		}//LoadDbFile()

		/// <summary>
		/// Load the key file.
		/// </summary>	
		void LoadLangKeys()
		{
			langKeys.Clear();

			TextAsset fileRead = Resources.Load<TextAsset>(fileName);
			Log.Assert(fileRead, "Missing localization file: "+fileName);

			string[] lines = fileRead.text.Split('\n');

			char[] charsToTrim = {  ']', '[', ' ', '\t', '\r', '\n' };

			foreach (string line in lines)
			{
				if (line.StartsWith(";"))
				{
					continue;
				}
				if (line.StartsWith("["))
				{
	
					string lang = line.Trim(charsToTrim);
					int idx = lang.IndexOf(".");
					if(idx >=0)
					{
						lang = lang.Substring(0, idx);
					}
					if (!langKeys.Contains(lang))
					{
						langKeys.Add(lang);
					}
				}
			}
			// foreach (string lang in langKeys) { Log.Info(lang); }
		}//LoadLangKeys()

		/// <summary>
		/// Set the language to the selected langKey.
		/// </summary>	
		public void SetLanguage(string langKey)
		{
			
			//foreach(string l in langKeys)
			//	Log.Info("langs:"+l);

			Log.Assert(langKeys.Contains(langKey), "Missing language: " + langKey);
			LoadLang(langKey);

			language = langKey;

		}//SetLanguage()

		/// <summary>
		/// Load the text by langKey
		/// </summary>	
		void LoadLang(string langKey)
		{
			//Log.Info ("<color=cyan>I18nManager</color> load language = <color=cyan>"+langKey+"</color>");

			dict.Clear();

			TextAsset fileRead = Resources.Load<TextAsset>(fileName);
			Log.Assert(fileRead, "Missing localization file: "+fileName);

			string[] lines = fileRead.text.Split('\n');
			bool found = false;
			string section = "";

			foreach (string line in lines)
			{
				if (line.StartsWith(";"))
				{
					continue;
				}
				if (line.StartsWith("["))
				{
					int id = line.IndexOf(".");
					string langGroup = (id > 0) ? "[" + langKey + "." : "[" + langKey + "]";
					//Log.Info("LangGroup:"+langGroup);

					if (line.StartsWith(langGroup))
					{
						found = true;
						//Log.Info("section:"+section);

						id = line.IndexOf(".");
						if(id >= 0 )
						{
							int start = id + 1;
							int end = line.IndexOf("]");
							if(end > 0)
							{
								section = line.Substring(start, end - start);
							}
							//Log.Info("section:"+section);
						}
						else
						{
							section = "";
						}

						//Log.Info("line="+line+"  -> section="+section);
					}
					else
					{
						found = false;
					}
				}
				else if (found && line.Contains( "=" ))
				{
					string[] l = line.Split('=');
					string k = "";
					string d = "";
					char[] charsToTrim = { ' ', '\t', '\r', '\n', '"' };
					if(section.Length > 0)
					{
						k = section + "." + l[0];
						d = l[1].TrimEnd(charsToTrim);
						d = (d.Length > 0) ? d.Substring(1, d.Length - 1) : d;

					}
					else
					{
						k = l[0];
						d = l[1].Trim(charsToTrim);
						d = d.TrimEnd(charsToTrim);

					}

					//Log.Debug("k="+k+" d="+d);
					if(dict.ContainsKey(k))
					{
						Log.Error("Key already exist in localatizion file:" + fileName + " key:" + k);
						return;
					}

					d = d.Replace("\\n", "\n");
					dict.Add(k, d);
					//dict.Add(section + "." + l[0], l[1].Substring(1, l[1].Length - 2));
				}
			}
			//foreach(KeyValuePair<string, string> entry in dict) { Log.Info(entry.Key + ": " + entry.Value); }
		}//LoadLang()

		/// <summary>
		/// Set the section
		/// </summary>	
		public void SetSection(string section)
		{
			//Log.Info("This section="+section);
			this.section = section;
		}//SetSection()

		/// <summary>
		/// Gets the text by key
		/// If the key contains "." it will treat 
		/// it as section.key
		/// </summary>	
		public string Get(string key)
		{

			if(key != null)
			{
				int i = key.IndexOf(".");			
				if(i > 0)
				{
					string s = key.Substring(0, i);
					string k = key.Substring(i+1);
					//Log.Info("s="+s+"  k="+k);
					return Get(s,k);
				}
				return Get(this.section, key);
			}

			return "";
		}//Get()

		/// <summary>
		/// Gets the text by section and key
		/// </summary>	
		public string Get(string section, string key)
		{

			//Log.Info("<color=cyan>NUM keys:</color>"+dict.Count);
			//foreach(string d in dict.Keys)
			//	Log.Info("keys:"+d);

			string k = (section != null && section.Length > 0) ? section + "." + key : key;
			// // Log.GameTimes("Missing key: " + k + " section " + section);
			if (!dict.ContainsKey(k))
			{
				// Log.GameTimes("Missing key: " + k + " section " + section);
				return key;
			}
			return dict[k];
		}//Get()

		public bool Set(string section, string key, string value)
		{
			
			//Log.Info("<color=cyan>NUM keys:</color>"+dict.Count);
			//foreach(string d in dict.Keys)
			//	Log.Info("keys:"+d);
			
			string k = (section != null && section.Length > 0) ? section + "." + key : key;
			// // Log.GameTimes("Missing key: " + k + " section " + section);
			if (!dict.ContainsKey(k))
			{
				// Log.GameTimes("Missing key: " + k + " section " + section);
				return false;
			}
			else
			{
				dict[k] = value;
				return true;
			}
		}//Get()

		/// <summary>
		/// Check if a key is valid
		/// </summary>
		public bool Contains(string key)
		{

			string k = (section != null && section.Length > 0) ? section + "." + key : key;
			return dict.ContainsKey(k);
		}

		/// <summary>
		/// Check if a key is valid
		/// </summary>
		public bool Contains(string section, string key)
		{	
			string k = (section != null && section.Length > 0) ? section + "." + key : key;
			return dict.ContainsKey(k);
		}
		/// <summary>
		/// Get loaded keys from section
		/// </summary>
		public List<string> GetKeys(string section)
		{
			
			List<string> keys = new List<string>();
			foreach(KeyValuePair<string,string> entry in dict)
			{
				string key = entry.Key;
				if(key.StartsWith(section))
					keys.Add(key);
			}

			return keys;
		}//GetKeys()

	}//class I18nManager

}//namespace 