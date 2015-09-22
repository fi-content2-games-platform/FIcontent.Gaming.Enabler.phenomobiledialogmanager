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
using Epigene;
using System.IO;
using System.Xml;
using System;
using System.Collections.Generic;
using Epigene.GAME;
using Epigene.MODEL;

namespace Epigene.IO
{
	
	public sealed class GameConfiguration
	{
		/// <summary>
		/// The game configuration file path.
		/// </summary>
		static readonly string gameConfigurationFilename =
			"GameConfiguration.xml";

		public bool initialized;

		/// <summary>
		/// Holds all key-value pairs parsed from GameConfiguration.xml
		/// </summary>
		private Dictionary<string, string> gameConfiguration = 
			new Dictionary<string, string>();

		/// <summary>
		/// Different functions to access Dictionary gameConfiguration
		/// </summary>
		public bool HasKey(string key)
		{
			return gameConfiguration.ContainsKey (key);
		}

		public string GetValue(string key)
		{
			if (!gameConfiguration.ContainsKey (key)) 
				WebPlayerDebugManager.addOutput(key + " not found.", 3);
			else
			{ 
				//WebPlayerDebugManager.addOutput(key + ": " + gameConfiguration[key], 0);
				return gameConfiguration[key];
			}
			return "";
		}

		public void AddValue(string key, string value)
		{
			if (HasKey(key) == false)
				gameConfiguration.Add(key, value);
			else
				gameConfiguration[key] = value;
		}


		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static GameConfiguration Instance
		{
			get{ return instance;}
		}
		static readonly GameConfiguration instance = new GameConfiguration();


		GameConfiguration()
		{
			Log.Debug("GameConfiguration initialized.");
		}//GameConfiguration()

		public void Load()
		{		
			#if UNITY_EDITOR
			// For requested XML with Editor
			DBModuleManager.Instance.Event 
				("GAME_CONFIGURATION_LOAD",
				 Application.dataPath + "/Resources/ExternalFiles/assets/",				 
				 AppConfiguration.Instance.configPath + gameConfigurationFilename);
			#elif UNITY_STANDALONE || UNITY_IOS
			// For requested XML with Standalone
			if (GameManager.Instance.standaloneBetaMode)
			{
				DBModuleManager.Instance.Event 
					("GAME_CONFIGURATION_LOAD",
					 GameManager.Instance.mainGame.ExternalFilesPath,					 
					 AppConfiguration.Instance.configPath + gameConfigurationFilename);
			}
			else
			{
				TextAsset download = Resources.Load("ExternalFiles/assets/config/GameConfiguration") as TextAsset;
				
				loadFromXmlOrUseDefaults (download.text);
			}
			#elif UNITY_WEBPLAYER
			// For requested XML with web player
			
			DBModuleManager.Instance.Event 
				("GAME_CONFIGURATION_LOAD",
                 null,				 
				 AppConfiguration.Instance.configPath + gameConfigurationFilename);
			#endif
		}

		/// <summary>
		/// Loads the configuration from xml
		/// </summary>
		public void loadFromXmlOrUseDefaults (string xml)
		{
			if (!string.IsNullOrEmpty (xml))
			{
				XmlReader reader = 
					XmlReader.Create (new StringReader(xml));
				parseXmlConfiguration (reader);
			}
			initialized = true;
			assignGameVariables ();
		}


		/// <summary>
		/// Parses the xml configuration to class fields.
		/// </summary>
		/// <param name="doc">Document.</param>
		void parseXmlConfiguration (XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element 
				    && reader.LocalName == "property")
				{
					AddValue(reader.GetAttribute ("key"), 
					         reader.GetAttribute ("value"));
				}
			}
		}


		/// <summary>
		/// Calls the GameScenarios function to collect the newly parsed variable values
		/// </summary>
		void assignGameVariables ()
		{
			WebPlayerDebugManager.addOutput("GameConfiguration.xml loaded.", 0);
			GameManager.Instance.Event("CONFIG", "ScenarioLoad", "");
		}
	}
}

