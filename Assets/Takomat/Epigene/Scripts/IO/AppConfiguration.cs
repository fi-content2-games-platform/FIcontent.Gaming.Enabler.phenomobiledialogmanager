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
using Epigene.GAME;
//------------------------------------------------------------------------------

namespace Epigene.IO
{
	
	public sealed class AppConfiguration 
	{
		/// <summary>
		/// The app configuration file path.
		/// </summary>
		static readonly string appConfigurationPath = 
			"/assets/AppConfiguration.xml";
		
				
		// start scenario
		public string 	scenarioURL 			= "assets/startScenario.xml";
		public string	scenarioMenuURL			= "assets/startScenarioMenu.xml";
		
		// abilities
		public bool 	saveAndLoadGameEnabled 	= true;

		//Screen HID
		public bool		useHIDTouch				= false;
		
		// configuration
		//Path to external Assets Folder
		public bool		exFilesPathIsAbsolute	= false; 
		public string	externalFilesPath		= "assets/";
		//subfolders of externalFilesPath
		public bool		exResourcePathIsAbsolute= false;
		public string	externalResourcePath	= "visuals/";
		public string 	configPath 				= "";
		public string	ipConfigPath			= "";
		
		// URL to use in order to check for internet connection
		public string 	connectionCheckURL 		= "http://www.takomat-bb.stage.endertech.net/";
		
		// db module configuration
		public bool 	displayDBModule 		= false;
		// this value configures which communication protocol to use.
		public string 	protocol 				= "http://";
		// this value configures which remote URL to use. Valid values are 'production' and 'staging'
		public string 	serverType 				= "production";

		
		//------------------------------------------------------------------------------
		
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static AppConfiguration Instance
		{
			get{ return instance;}
		}
		static readonly AppConfiguration instance = new AppConfiguration();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epigene.IO.AppConfiguration"/> class.
		/// </summary>
		AppConfiguration()
		{
		}

		public void Load()
		{
			Log.Debug("AppConfiguration initialized. Loading XML...");

			#if UNITY_EDITOR
			// For requested XML with Editor

			DBModuleManager.Instance.Event(
				"APP_CONFIGURATION_LOAD",
			    Application.dataPath,
				"/Resources/ExternalFiles" + appConfigurationPath);
			#elif UNITY_STANDALONE || UNITY_IOS
			// For requested XML with Standalone
			if (GameManager.Instance.standaloneBetaMode)
			{
				DBModuleManager.Instance.Event(
					"APP_CONFIGURATION_LOAD",
				    Application.dataPath,
				    appConfigurationPath);
			}
			else
			{
				TextAsset download = Resources.Load("ExternalFiles/assets/AppConfiguration") as TextAsset;
				
				loadFromXmlOrUseDefaults (download.text);
			}
			#elif UNITY_WEBPLAYER
			// For requested XML with web player
			DBModuleManager.Instance.Event (
				"APP_CONFIGURATION_LOAD",
			    Application.dataPath,
			    appConfigurationPath);
			#endif
		}//AppConfiguration()
		
		/// <summary>
		/// Loads the configuration from xml file or uses config defaults.
		/// </summary>
		public void loadFromXmlOrUseDefaults (string xml)
		{
			if (!string.IsNullOrEmpty (xml))
			{
				XmlReader reader = XmlReader.Create (new StringReader(xml));
				parseXmlConfiguration (reader);
				// Log.Info ("Using XML configuration.");
			}
			else
			{
				Log.Info ("XML request is empty. Using defaults.");
			}
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
					assignXmlProperties (reader.GetAttribute("key"), 
					                     reader.GetAttribute ("value"));
				}
			}
			GameManager.Instance.Event
				("CONFIG",
				 "APP_CONFIGURATION_LOADED",
				 "");
			WebPlayerDebugManager.addOutput("AppConfiguration.xml loaded.", 0);
		}// parseXmlConfiguration()
		
		/// <summary>
		/// Assigns the property values by name.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		void assignXmlProperties (string name, string value)
		{
			//WebPlayerDebugManager.addOutput(name + ": " + value, 1);
			switch (name)
			{
			case "scenarioURL":
				scenarioURL = value;
				break;
			case "scenarioMenuURL":
				scenarioMenuURL = value;
				break;
			case "exFilesPathIsAbsolute":
				exFilesPathIsAbsolute = bool.Parse (value);
				break;
			case "externalFiles":
				externalFilesPath = value;
				GameManager.Instance.mainGame.ExternalFilesPath =
					externalFilesPath;
				break;
			case "saveAndLoadGameEnabled":
				saveAndLoadGameEnabled = bool.Parse (value);
				break;
			case "configPath":
				configPath = value;
				WebPlayerDebugManager.addOutput("Changed config path to " + configPath, 1);
				break;
			case "exResourcePathIsAbsolute":
				exResourcePathIsAbsolute = bool.Parse (value);
				break;
			case "externalResourcePath":
				#if !UNITY_EDITOR 
				externalResourcePath = value;
				GameManager.Instance.mainGame.ExternalResourcePath =
					externalResourcePath;
				#endif
				break;
			case "ipConfigPath":
				ipConfigPath = value;
				break;
			case "connectionCheckURL":
				connectionCheckURL = value;
				break;
			case "displayDebugWindow":
				displayDBModule = bool.Parse (value);
				#if !UNITY_EDITOR
				GameManager.Instance.debugMode = displayDBModule;
				#endif
				WebPlayerDebugManager.addOutput("App Config displayDBModule", 1);
				break;
			case "useHIDTouch":
				useHIDTouch = bool.Parse (value);
				#if !UNITY_EDITOR
				GameManager.Instance.hidTouch = useHIDTouch;
				#endif
				//WebPlayerDebugManager.addOutput("App Config displayDBModule", 1);
				break;
			case "protocol":
				protocol = value;
				DBModuleManager.Instance.Event(
					"MODULE_CONFIG_CHANGE_PROTOCOL",
					null,
					value);
				break;
			case "serverType":
				serverType = value;
				DBModuleManager.Instance.Event(
					"MODULE_CONFIG_CHANGE_SERVER",
					null,
					value);
				break;
			default:
				Log.Info ("The ApConfiguration property name '" + name + "' can't be found.");
				break;
			}
		}// assignProperties()
		
	}// AppConfiguration
	
}// Epigene.IO