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
using System;
using System.Collections;
using Epigene;
using Epigene.GAME;
using System.Collections.Generic;
//------------------------------------------------------------------------------
using System.Linq;

namespace Epigene.IO 
{
	public sealed class DBModule : MonoBehaviour
	{

		private const int 		MAX_OUTPUT_LINES 	= 50;
		public string			SessionToken
		{
			get {return _sessionToken;}
			set {_sessionToken = value;}
		}
		private string 			_sessionToken = "";
		private string 			_hashMagic = 
			"E*XPnAPNk@|jG,oWRz_{rN)o2bDfg5Y*9s]";
				
		// Communication protocol type
		// @default http://
		private static string _protocol = ProtocolTypes.HTTP;

		// API endpoint base url that gets appended to the service call url 
		// depending on the event.
		// @default "production" key must be set in the beginning of the game	
		private Dictionary<string, string> allowedServiceBaseURLs = 
			new Dictionary<string, string>();

		// default value random;-)
		private string 		_serviceBaseUrl = "buergerbeteiligungsspiel.de";

		// API endpoint timeout value for requests.
		// @default 10 seconds
		private int _serviceTimeout = 10;
		
		public 	string 	debugMsg 			= "";
		private bool 	debugOutputStopped 	= false;
		
		private string 	_savedGameDebug; //= DebugConfig.SAVED_SCENARIO;
		private bool 	_saveGameDebuggingEnabled 	= false;

		public 	delegate void ResultCallback (WWW result);
		private DBModuleManager dbModuleManager;
		private MainGame mainGame;

		// for adding up construction information
		private string constructionInformation = "";

//------------------------------------------------------------------------------

		void Awake()
		{

			Log.Debug(">>>>>>>>>>>>>>>>>>>>>> DBModule AWAKE ");

			
		}

		// /// <summary>
		// /// ctor
		// /// </summary>
		// // public DBModule()
		// public OnEnable()
		// {
		// 	// Add event handler to receive messags
		// 	//DBModuleManager.EventHandler = EventHandler;
		// 	//Debug.Log ("Instanciate DBModule");
		// }

		/// <summary>
		/// Enable this module will register events and initialize the object
		/// </summary>
		void OnEnable()
		{
			Log.Debug(">>>>>>>>>>>>>>>>>>>>>> DBModule OnEnable ");

			bool oldDebugState 	= debugOutputStopped;
			debugOutputStopped 	= false;
			/*
			if (_sessionToken != MainGame.GetValue ("sessionToken"))
			{
				dbModuleEventSession (MainGame.GetValue ("sessionToken"));
			}*/

			addDebugOutput("\nDBModule added to scene");
			addDebugOutput("\nService URL: " + 
			               (_protocol + 
			 				_serviceBaseUrl));
			addDebugOutput("\nService Timeout: " + 
			               _serviceTimeout);
			
			debugOutputStopped = oldDebugState;
			dbModuleManager = DBModuleManager.Instance;
			if (dbModuleManager != null)
			{
				Debug.Log(">>>>>>>>>>> dbMdule ok");

				dbModuleManager.RegisterEventHandler("ADD_SERVICEURL", addServiceBaseURL);
				dbModuleManager.RegisterEventHandler("APP_CONFIGURATION_LOAD", loadAppConfiguration);
				dbModuleManager.RegisterEventHandler("CONFIG", loadConfigFile);
				dbModuleManager.RegisterEventHandler("GAME_CONFIGURATION_LOAD", loadGameConfiguration);
				dbModuleManager.RegisterEventHandler("MODULE_CONFIG_CHANGE_PROTOCOL", changeProtocol);
				dbModuleManager.RegisterEventHandler("MODULE_CONFIG_CHANGE_SERVER", changeServerType);
				dbModuleManager.RegisterEventHandler("SCREEN_CHANGE", changeScreen);
				dbModuleManager.RegisterEventHandler("SESSION_TOKEN_CHANGE", dbModuleEventSession);
				dbModuleManager.RegisterEventHandler("START_SCENARIO_LOAD", loadStartScenario);
				dbModuleManager.RegisterEventHandler("START_SCENARIO_SAVE", saveStartScenario);
				dbModuleManager.RegisterEventHandler("URL_CHANGE", dbModuleEventUrl);

				#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
				WebPlayerDebugManager.addOutput("HTTP Requests will only be performed online.", 3);
				return;

				#elif UNITY_WEBPLAYER
				dbModuleManager.RegisterEventHandler("GAME_LOAD", LoadGame);
				dbModuleManager.RegisterEventHandler("GAME_SAVE", saveGame);
				dbModuleManager.RegisterEventHandler("HIGHSCORE_SAVE", saveHighscoreSplit);
				dbModuleManager.RegisterEventHandler("DEBUG_HIGHSCORE_SAVE", saveDebugHighscoreSplit);
				dbModuleManager.RegisterEventHandler("CHECK_FOR_SAVED_GAME", checkForSavedGame);
				// TODO: Parameters?
				//trackGameAction (null, -1, null);
				//dbModuleManager.RegisterEventHandler("TRACK_ACTION", trackGameAction);
				dbModuleManager.RegisterEventHandler("TRACK_CONSTRUCTION", trackConstructionAction);
				#endif
			}

			else { Debug.LogWarning("Missing dbmodule");}

		}

		/// <summary>
		/// Disable game object will remove listeners and clean up.
		/// </summary>
		void OnDisable()
		{
			Log.Debug(">>>>>>>>>>>>>>>>>>>>>> DBModule OnDisable");

			// Remove event handler from sender
			if (dbModuleManager != null)
			{
				dbModuleManager.RemoveEventHandler("ADD_SERVICEURL", addServiceBaseURL);
				dbModuleManager.RemoveEventHandler("APP_CONFIGURATION_LOAD", loadAppConfiguration);
				dbModuleManager.RemoveEventHandler("CONFIG", loadConfigFile);
				dbModuleManager.RemoveEventHandler("GAME_CONFIGURATION_LOAD", loadGameConfiguration);
				dbModuleManager.RemoveEventHandler("MODULE_CONFIG_CHANGE_PROTOCOL", changeProtocol);
				dbModuleManager.RemoveEventHandler("MODULE_CONFIG_CHANGE_SERVER", changeServerType);
				dbModuleManager.RemoveEventHandler("SCREEN_CHANGE", changeScreen);
				dbModuleManager.RemoveEventHandler("SESSION_TOKEN_CHANGE", dbModuleEventSession);
				dbModuleManager.RemoveEventHandler("START_SCENARIO_LOAD", loadStartScenario);
				dbModuleManager.RemoveEventHandler("START_SCENARIO_SAVE", saveStartScenario);
				dbModuleManager.RemoveEventHandler("URL_CHANGE", dbModuleEventUrl);
				
				#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
				return;
				#elif UNITY_WEBPLAYER
				dbModuleManager.RemoveEventHandler("GAME_LOAD", LoadGame);
				dbModuleManager.RemoveEventHandler("GAME_SAVE", saveGame);
				dbModuleManager.RemoveEventHandler("HIGHSCORE_SAVE", saveHighscoreSplit);
				dbModuleManager.RemoveEventHandler("DEBUG_HIGHSCORE_SAVE", saveDebugHighscoreSplit);
				dbModuleManager.RemoveEventHandler("CHECK_FOR_SAVED_GAME", checkForSavedGame);
				// TODO: Parameters?
				//trackGameAction (null, -1, null);
				//dbModuleManager.RemoveEventHandler("TRACK_ACTION", trackGameAction);
				dbModuleManager.RemoveEventHandler("TRACK_CONSTRUCTION", trackConstructionAction);
				#endif

				dbModuleManager.Clear();
			}
		}

		/// <summary>
		/// Converts a string to a byte array
		/// </summary>
		private byte[] GetBytes (string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void processExternalInterfaceCall (string callName) 
		{
			if (Application.isWebPlayer) 
			{
				addDebugOutput("\n\t> ExternalInterface available. Redircting");
				Application.ExternalCall(callName);
			} 
			else 
			{
				addDebugOutput("\n\t> ExternalInterface NOT available");
			}
		}


		/// <summary>
		/// Developmental event to update the API endpoint avoiding recompiling.
		/// @param serviceUrl The new base service URL to use
		///[EventHandler(event = "DbModuleEvent.URL_CHANGE", properties = "serviceUrl")]
		/// </summary>
		public void dbModuleEventUrl (string eventId, string param) 
		{
			_serviceBaseUrl = param;
			addDebugOutput("\n# Service URL set: " +
			               _serviceBaseUrl);
		}


		/// <summary>
		/// Updates the session token
		/// @param sessionToken The new session token to use
		/// [EventHandler(event = "SessionEvent.SESSION_TOKEN_CHANGE", 
		/// properties = "sessionToken")]
		/// </summary>
		public void dbModuleEventSession (string eventId, string param) 
		{
			_sessionToken = param;
			
			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			addDebugOutput("\n# Session token set: " + _sessionToken);
			debugOutputStopped = oldDebugState;
		}

		public void addServiceBaseURL (string _eventId, string _param)
		{
			//WebPlayerDebugManager.addOutput("AddServiceURL: " + _eventId + " " + _param, 1);
			allowedServiceBaseURLs.Add (_eventId, _param);
			return;
		}

		/// <summary>
		/// Updates the communication protocol
		/// @param protocol	The new communication protocol to use
		/// [EventHandler(event = "ModuleConfigEvent.CHANGE_PROTOCOL", 
		/// properties = "protocol", priority = "10")]
		/// </summary>
		public void changeProtocol (string  eventId, string param) 
		{
			_protocol = param;
			
			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			//WebPlayerDebugManager.addOutput (" Communication protocol changed. New Protocol: " + _protocol, 1);
			addDebugOutput("\n# Communication protocol changed.");
			addDebugOutput("\n\tNew Protocol: " + _protocol);
			addDebugOutput("\n\tNew Service URL: " + (_protocol + _serviceBaseUrl));
			debugOutputStopped = oldDebugState;
		}

		
		/// <summary>
		/// Updates the server instance
		/// @param serverType	The new server type to use
		/// [EventHandler(event = "ModuleConfigEvent.CHANGE_SERVER", 
		/// properties = "serverType", priority = "10")]
		/// </summary>
		public void changeServerType (string eventId, string param) 
		{
			// requirement to be set
			if(!(allowedServiceBaseURLs.ContainsKey ("production"))) 
			{
				WebPlayerDebugManager.addOutput(
					"Required Servertype production not set"+
					"Please set it in the MainGame App!" , 1);
				return;
			}
			_serviceBaseUrl = allowedServiceBaseURLs["production"];

			// now set the type
			if(!(allowedServiceBaseURLs.ContainsKey (param)))
			{
				WebPlayerDebugManager.addOutput(
					"Servertype " + param + 
					" unknown. Default is production: " 
					+ _serviceBaseUrl, 1);
				return;
			}

			#if UNITY_EDITOR
			_serviceBaseUrl = allowedServiceBaseURLs[param];
			#elif UNITY_WEBPLAYER

			string newServiceBaseUrl = allowedServiceBaseURLs[param];


			if (newServiceBaseUrl.Count(x => x == '.') > 1)
				newServiceBaseUrl = newServiceBaseUrl.Remove(0,
					newServiceBaseUrl.LastIndexOf('.', 
              			newServiceBaseUrl.LastIndexOf('.')-1)+1); 
			if (Application.dataPath.Contains(newServiceBaseUrl))
			{
				WebPlayerDebugManager.addOutput(newServiceBaseUrl + " in " + Application.dataPath, 1);
				string appdataPathMainAddress = Application.dataPath;
				appdataPathMainAddress = appdataPathMainAddress.Remove(0, appdataPathMainAddress.IndexOf("://") + 3);
				appdataPathMainAddress = appdataPathMainAddress.Remove(appdataPathMainAddress.IndexOf("/epigene_unity3d"));
				//WebPlayerDebugManager.addOutput("Final path " + appdataPathMainAddress, 1);
				newServiceBaseUrl = appdataPathMainAddress;
			}
			else
			{
				WebPlayerDebugManager.addOutput("BaseURL is not correct, AppConfiguration.xml might be corrupt or the game is running on the wrong server!", 3);
				//Epigene.UI.UIManager.Instance.ActivateScreen("GameOver");
				return;
			}
			_serviceBaseUrl = newServiceBaseUrl;

			#else
			_serviceBaseUrl = allowedServiceBaseURLs[param];
			#endif


			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			//WebPlayerDebugManager.addOutput (" Server type changed. New Type: " + param +  "new adress: " + _serviceBaseUrl, 1);
			addDebugOutput("\n# Server type changed.");
			addDebugOutput("\n\tNew server type: " + param);
			addDebugOutput("\n\tNew Service URL: " + (_protocol + _serviceBaseUrl));
			debugOutputStopped = oldDebugState;
		}


		/// <summary>
		/// Changes the active screen index.
		/// @param screen	the new screen index
		/// [EventHandler(event = "ScreenEvent.CHANGE", properties = "screen")]
		/// </summary>
		public void changeScreen (string eventId, string param) 
		{
			if (param == "HIGHSCORE") {	//ScreenIDs.HIGHSCORE
				addDebugOutput("\n> #Redirect to scoreboard");
				processExternalInterfaceCall("stmugNavigateToHighscores");

			} else if (param == "EXIT") { //ScreenIDs.EXIT
				addDebugOutput("\n> #Close Game Popup");
				processExternalInterfaceCall("closeFancyBox");
			}

			// create dummy event
			//DBModuleManager.Event(DBModuleEvent.SCREEN_CHANGE, null, null);
		}

		/// <summary>
		/// Loads the app Configuration
		/// [EventHandler(event = "DBModuleEvent.APP_CONFIGURATION_LOAD")]
		/// </summary>
		public void loadAppConfiguration (string baseUrl, string serviceUrl) 
		{
			Debug.Log(">>>>>>>>>>>>>> loadAppConfiguration");
			
			string oldBaseUrl  = _serviceBaseUrl;
			string oldProtocol = _protocol;
			if (baseUrl != null)
			{
				dbModuleEventUrl ("", baseUrl);
				#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
				changeProtocol ("", "file://");
				#elif UNITY_WEBPLAYER
				changeProtocol ("", "");
				#endif
			}

			WWWForm param = new WWWForm ();
			param.AddField ("", "");
			sendHTTPRequest (serviceUrl, param, loadAppConfigurationResult);
			dbModuleEventUrl ("", oldBaseUrl);
			changeProtocol ("", oldProtocol);
		}
		
		private void loadAppConfigurationResult (WWW result) 
		{
			Debug.Log(">>>>>>>>>>>>>> loadAppConfigurationResult");
			
			if (!string.IsNullOrEmpty (result.error))
			{
				WebPlayerDebugManager.addOutput("AppConfiguration request error: " + result.error, 3);
				WebPlayerDebugManager.addOutput("No XML loaded. Using defaults.", 3);
			}
			else
			{
				//hand over data to client
				AppConfiguration.Instance.loadFromXmlOrUseDefaults (result.text);
			}
		}

		/// <summary>
		/// Loads the game Configuration
		/// [EventHandler(event = "DBModuleEvent.GAME_CONFIGURATION_LOAD")]
		/// </summary>
		public void loadGameConfiguration (string baseUrl, string serviceUrl) 
		{
			Debug.Log(">>>>>>>>>>>>>> loadGameConfiguration");
			
			string oldBaseUrl  = _serviceBaseUrl;
			string oldProtocol = _protocol;
			if (baseUrl != null)
			{
				dbModuleEventUrl ("", baseUrl);
				changeProtocol ("", "file://");
			}

			WWWForm param = new WWWForm ();
			param.AddField ("", "");
			sendHTTPRequest (serviceUrl, param, loadGameConfigurationResult);
			dbModuleEventUrl ("", oldBaseUrl);
			changeProtocol ("", oldProtocol);
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void loadGameConfigurationResult (WWW result) 
		{
			Debug.Log(">>>>>>>>>>>>>> loadGameConfigurationResult");

			//hand over data to client
			if (!string.IsNullOrEmpty (result.error))
			{
				WebPlayerDebugManager.addOutput("GameConfiguration request error: " + result.error, 3);
				WebPlayerDebugManager.addOutput("No XML loaded. Using defaults.", 3);
			}
			else
			{
				//hand over data to client
				GameConfiguration.Instance.loadFromXmlOrUseDefaults (result.text);
			}
		}

		/// <summary>
		/// Loads the start scenario data
		/// [EventHandler(event = "DBModuleEvent.START_SCENARIO_SAVE", properties="gameData")]
		/// </summary>
		public void saveStartScenario (string _eventId, string _param) 
		{
			byte[] scenarioBytes = GetBytes(_param);
			string gameDataEncoded = "";
			if (scenarioBytes.Length > 0) 
			{
				gameDataEncoded = System.Convert.ToBase64String(scenarioBytes);
			}
			
			addDebugOutput ("Save presimulated start scenario to DB");
			
			if (!string.IsNullOrEmpty(_sessionToken)) 
			{
				WWWForm param = new WWWForm ();
				param.AddField ("sessionToken", _sessionToken);
				param.AddField ("scenarioData", gameDataEncoded);
				param.AddField ("scenarioType", "start");
				string serviceUrl = "/api.php/event/scenario/save";
				//Checks if _sessionToken is actual
				if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
				{
					dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
				}
				param.AddField ("sessionToken", _sessionToken);

				sendHTTPRequest (serviceUrl, param, saveScenarioResult);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void saveScenarioResult (WWW result) 
		{
			dbModuleManager.Event ("GAME_SAVE_COMPLETE", null, result.text);
			WebPlayerDebugManager.addOutput ("Save Scenario Result: " + result.text, 1);
			addDebugOutput("\n> DONE");
		}


		/// <summary>
		/// Loads the start scenario data
		/// [EventHandler(event = "DBModuleEvent.START_SCENARIO_LOAD")]
		/// </summary>
		public void loadStartScenario (string baseUrl, string serviceUrlAddition) 
		{
			//WebPlayerDebugManager.addOutput("Load Start Scenario: " + baseUrl + serviceUrlAddition, 1);
			string oldBaseUrl  = _serviceBaseUrl;
			string oldProtocol = _protocol;

			if (baseUrl != null)
			{
				dbModuleEventUrl ("", baseUrl);
				#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
				changeProtocol ("", "file://");
				#elif UNITY_WEBPLAYER
				changeProtocol ("", "");
				#endif
			}
			Log.Info("Base Url: " + baseUrl + " Service URL Addition: " + serviceUrlAddition);

			string serviceUrl 		= serviceUrlAddition;
			//string serviceUrl 		= "/api.php/event/scenario/load";
			
			WWWForm param = new WWWForm ();
			//param.AddField ("sessionToken", _sessionToken);
			param.AddField ("scenarioType", "start");
			//Checks if _sessionToken is actual
			if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
			{
				dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
			}
			param.AddField ("sessionToken", _sessionToken);

			sendHTTPRequest (serviceUrl, param, loadStartScenarioResult);
			dbModuleEventUrl ("", oldBaseUrl);
			changeProtocol ("", oldProtocol);
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void loadStartScenarioResult (WWW result) 
		{
			//byte[] startScenarioData = System.Convert.FromBase64String (result.text);
			
			//WebPlayerDebugManager.addOutput ("StartScenario Loaded.", 1);

			//hand over data to client
			//DBModuleManager.Event (DBModuleEvent.START_SCENARIO_COMPLETE, null, startScenarioData.ToString ());+
			Epigene.GAME.GameManager.Instance.Event("CONFIG",
			                                        "StartScenarioLoad",
			                                        result.text);

			addDebugOutput("\n> Load Success");
		}
		

		/// <summary>
		/// Saves the given game scenario data to the DB
		/// [EventHandler(event = "DBModuleEvent.GAME_SAVE", properties = "gameData")]
		/// </summary>
		private void saveGame (string _eventId, string _param) 
		{
			byte[] gameData = GetBytes(_param);
			//TODO
			// save bytearray to DB for current user
			string 	 gameDataEncoded =  "";
			if (gameData.Length > 0) 
			{
				gameDataEncoded =  System.Convert.ToBase64String(gameData);
			}
			WebPlayerDebugManager.addOutput("saveGame param = "+_param+", eventId = "+_eventId, 1);

			//debug Code
			if (_saveGameDebuggingEnabled)
			{
				_savedGameDebug = gameDataEncoded;
				// Log.Info ("savedGameDebug: " + _savedGameDebug);
				saveGameResult(null);
			}
			// production code
			else
			{
				string serviceUrl = "/api.php/event/game/save"; // ?gameData="abc"&sessionToken=1
				WWWForm param 	  = new WWWForm();

				if(constructionInformation != "")
					_param += ";"+constructionInformation;

				param.AddField ("gameData", _param);
				//Checks if _sessionToken is actual
				if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
				{
					dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
				}
				param.AddField ("sessionToken", _sessionToken);

				sendHTTPRequest(serviceUrl, param, saveGameResult);
			}
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void saveGameResult (WWW result) 
		{
			// inform client that the game has been saved
			WebPlayerDebugManager.addOutput ("save Game Result: " + result.text, 1);
		}


		/// <summary>
		/// Checks if the current user has save a game before
		/// [EventHandler(event = "DBModuleEvent.CHECK_FOR_SAVED_GAME")]
		/// </summary>
		public void checkForSavedGame (string _eventId, string _param) 
		{
			if (_saveGameDebuggingEnabled)
			{
				checkForSavedGameResult(null);
			}
			//production code
			else
			{
				string serviceUrl = "/api.php/event/game/check";
				WWWForm param = new WWWForm ();
				//Checks if _sessionToken is actual
				if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
				{
					dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
				}
				param.AddField ("sessionToken", _sessionToken);

				sendHTTPRequest(serviceUrl, param, checkForSavedGameResult);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		void checkForSavedGameResult (WWW result)
		{
			// inform client about the result
			WebPlayerDebugManager.addOutput ("check for saved Game Result: " + result.text, 1);
			string hasSavedGame = "0"; // No

			//debug code
			if (_saveGameDebuggingEnabled)
			{
				if (!string.IsNullOrEmpty(_savedGameDebug))
				{
					hasSavedGame = "1"; // Yes
				}
			}
			//production code
			else
			{
				if ((string.IsNullOrEmpty(result.text)) && result.text == "1")
				{
					hasSavedGame = "1"; // Yes
				}
			}

			// Log.Info ("checkForSavedGame: " +
			//          "\n> result.text: " + result.text +
			//          "\n> hasSavedGame: " + hasSavedGame);
			dbModuleManager.Event ("CHECK_FOR_SAVED_GAME_COMPLETE", null, hasSavedGame);
		}


		/// <summary>
		/// Loads the last game (byte array) the current user has saved
		/// [EventHandler(event = "DBModuleEvent.GAME_LOAD")]
		/// </summary>
		public void LoadGame (string _eventId, string _param) 
		{
			WebPlayerDebugManager.addOutput ("Load Game: " + _eventId, 1);

			// debug code
			if (_saveGameDebuggingEnabled)
			{
				loadGameResult(null);
			}
			// production code
			else
			{
				string serviceUrl = "/api.php/event/game/load";
				// load last game the current user has saved
				WWWForm param = new WWWForm ();
				//Checks if _sessionToken is actual
				if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
				{
					dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
				}
				param.AddField ("sessionToken", _sessionToken);

				sendHTTPRequest(serviceUrl, param, loadGameResult);
			}
		}


		/// <summary>
		/// current division of gameData information after finishing sim:
		///  result.text.Split(";");
		///  =>	[0]: socialTotalscore&economyTotalscore&ecologyTotalscore
		///		[1](-[2]): constructionModel:numberBuiltOfModel
		/// 	
		/// 	afterwards in turns:
		///		- constructionType&day-month-year&h:m:s&givenConstructionName&monthsSinceStart
		///			-> information for calling addConstruction() in FinanceManager.cs
		/// 	- ConstructionTypeName&positionX&positionY&positionZ
		/// 		-> information for calling BuildConstruction() in Simulation.cs
		/// ex.: 
		/// 12333&456666&78999;
		/// Epigene.MODEL.E0001_Pipe:2;
		/// E0001&02-13-2015&12:34:56&&0;
		/// PIPE&238.9609&301.9727&0;
		/// E0001&04-17-2015&12:34:56&&0;
		/// PIPE&238.9609&301.9727&0
		/// </summary>
		private void loadGameResult (WWW result) 
		{
			WebPlayerDebugManager.addOutput ("Load Game Result: " + result.text, 1);

			string[] infoArray = result.text.Split(';');

			for(int i=0; i<infoArray.Length; i++) {
				WebPlayerDebugManager.addOutput("__ "+infoArray[i], 1);
				GameManager.Instance.Event(
					"GAME_LOAD_COMPLETE", 
					"load"+i.ToString(), 
					infoArray[i]);
			}

			/*/ client expects two byte arrays
			string decoded;
			
			// debug code
			if (_saveGameDebuggingEnabled)
			{
				decoded = _savedGameDebug;
			}
			// production code
			else
			{
				decoded = result.text;
			}
			
			byte[] gameData = System.Convert.FromBase64String(decoded);
			
			// inform client about the result
			dbModuleManager.Event ("GAME_LOAD_COMPLETE", null, gameData.ToString ());*/

			// inform client about the result
			dbModuleManager.Event ("GAME_LOAD_COMPLETE", null, result.text);
		}
		
		/// <summary>
		/// @param data	holds information about construction Type, BuildTime, (name), months passed when built.
		///[EventHandler(event = "TrackingEvent.TRACK_CONSTRUCTION", properties = "actionName, actionIndex, data")]
		/// </summary>
		private void trackConstructionAction(string eventId, string data)
		{
			string serviceUrl = "/api.php/event/game/save"; // ?gameData="abc"&sessionToken=1
			WWWForm param 	  = new WWWForm();

			if(constructionInformation != "")
				data = constructionInformation+";"+data;

			constructionInformation = data;

			WebPlayerDebugManager.addOutput ("tracking construction: " + data, 1);
			Log.GameTimes("_____________ tracking construction: "+data);
				
			param.AddField ("gameData", data);
			//Checks if _sessionToken is actual
			if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
			{
				dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
			}
			param.AddField ("sessionToken", _sessionToken);
			
			sendHTTPRequest(serviceUrl, param, saveGameResult);
		}


		/// <summary>
		/// @param actionName	human readable name of the action. Valid values are defined in com.takomat.modules.model.constants.GameActions
		/// @param actionIndex	zero based action index
		/// @param data			optional additional data that might be passed with the event.
		/// 						For the action FINISHED_GAME_YEAR this parameter contains the number
		/// 						of years that have already been played.
		///[EventHandler(event = "TrackingEvent.TRACK_ACTION", properties = "actionName, actionIndex, data")]
		/// </summary>
		public void trackGameAction (string 			actionName, 
		                             int 				actionIndex, 
		                             UnityEngine.Object data		) 
		{
			//TODO
			string serviceUrl = "/api.php/event/track";
			WWWForm param = new WWWForm ();
			param.AddField ("actionIndex", actionIndex);
			param.AddField ("actionName",  actionName);
			param.AddField ("data", 	   data.ToString());
			
			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			
			addDebugOutput("\ntrack Game Action");
			addDebugOutput("\n\t>actionIndex: " +
			               actionIndex);
			addDebugOutput("\n\t>actionName: " +
			               actionName);
			addDebugOutput("\n\t>data: " +
			               data);
			
			debugOutputStopped = oldDebugState;
			//Checks if _sessionToken is actual
			if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
			{
				dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
			}
			param.AddField ("sessionToken", _sessionToken);

			sendHTTPRequest (serviceUrl, param, null);
		}

		/// <summary>
		/// Splits EventId String for saveHighscore into single variables
		/// </summary>
		/// <param name="eventId">Event identifier.</param>
		/// <param name="param">Parameter.</param>
		public void saveHighscoreSplit (string eventId, string param)
		{
			WebPlayerDebugManager.addOutput("Highscore save initialized.", 4);
			char[] 	 parameterDelimiters = new char[]{'&'};
			
			string[] parameters = param.Split (
				parameterDelimiters, 
				System.StringSplitOptions.None);
			
			saveHighscore(float.Parse (parameters[0]), 
			              float.Parse (parameters[1]), 
			              float.Parse (parameters[2]), 
			              float.Parse (parameters[3]),
			              parameters[4],
			              parameters[5],
			              parameters[6],
			              parameters[7],
			              parameters[8]);

		}

		/// <summary>
		/// Saves the given highscore data to the DB
		/// @param socialPoints		score gained on the social account
		/// @param ecologyPoints		score gained on the ecology account
		/// @param economyPoints		score gained on the economy account
		/// @param totalPoints		overall score
		/// @param eMix				String describing the current energy mix like that: Kohle:20%|Gas:10%|... (energy types are in german)
		/// @param energyPrice		current price per kWh in Euro Cent
		/// @param username			user name
		/// @param achievmentList	comma separated URL-encoded list of achievements gained by the user (achievment names are in german)
		/// @param eMixChartImage	base64 String encoding a pie chart which renders the current energy mix
		///[EventHandler(event = "DBModuleEvent.HIGHSCORE_SAVE", properties = "socialPoints, ecologyPoints, economyPoints, totalPoints, eMix, energyPrice, username, achievmentList, image")]
		/// </summary>
		public void saveHighscore (float  socialPoints, 
		                           float  ecologyPoints, 
		                           float  economyPoints, 
		                           float  totalPoints,
		                           string eMix, 
		                           string energyPrice, 
		                           string username, 
		                           string achievmentList, 
		                           string eMixChartImage) 
		{
			if (username == "")
				return;

			int sumOfPoints  = 0;
				sumOfPoints += (int)socialPoints;
				sumOfPoints += (int)ecologyPoints;
				sumOfPoints += (int)economyPoints;
				sumOfPoints += (int)totalPoints;

			string textToHash 	= sumOfPoints.ToString () +
							      _sessionToken +
								  _hashMagic;
			string digest 		= GetMD5HashString(textToHash);

			//Float Convertion: 135.5f   ToString() -> "135.5",   "135.5"   toServer -> "135.50"
			//					135.0f 	 ToString() -> "135",     "135"     toServer -> "135.00"
			//					15000.5f ToString() -> "15000.5", "15000.5" toServer -> "15000.50"

			string serviceUrl = "/api.php/event/score/save";
			WWWForm param = new WWWForm ();
					param.AddField ("socialPoints", socialPoints.ToString());
					param.AddField ("ecologyPoints", ecologyPoints.ToString());
					param.AddField ("economyPoints", economyPoints.ToString());
					param.AddField ("totalPoints", totalPoints.ToString());
					param.AddField ("eMix", eMix);
					param.AddField ("energyPrice", energyPrice);
					param.AddField ("username", username);
					param.AddField ("achievmentList", achievmentList);
					param.AddField ("eMixChartImage", eMixChartImage);
					param.AddField ("digest", digest);
			//Checks if _sessionToken is actual
			if (_sessionToken != GameManager.Instance.mainGame.GetValue ("sessionToken"))
			{
				dbModuleEventSession ("", GameManager.Instance.mainGame.GetValue("sessionToken"));
			}
			param.AddField ("sessionToken", _sessionToken);
			
			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			
			WebPlayerDebugManager.addOutput(   "\nsave Highscore" +
								               /*"\n\t>socialPoints: " + 
								               socialPoints +
											   "\n\t>ecologyPoints: " +
											   ecologyPoints +
											   "\n\t>economyPoints: " + 
											   economyPoints +
											   "\n\t>totalPoints: " + 
										       totalPoints + 
											   "\n\t>eMix: " + 
											   eMix+"\n\t>energyPrice: " + 
											   energyPrice +*/
											   "\n\t>username: " + 
										       username +
				                                "\n\t>SessionToken: " + 
			                               	 	_sessionToken +
											   /*"\n\t>achievmentList: " + 
											   achievmentList +*/
											   "\n\t>sumOfPoints: " + 
											   sumOfPoints +
											   "\n\t>digest: " + 
											   digest, 0);
			debugOutputStopped = oldDebugState;	

			sendHTTPRequest(serviceUrl, param, saveHighscoreResult);
		}

		/// <summary>
		/// 
		/// </summary>
		private void saveHighscoreResult (WWW result) 
		{
			WebPlayerDebugManager.addOutput ("Save Highscore Result: " + result.text, 1);
			// inform client that the highscore has been saved
			dbModuleManager.Event ("HIGHSCORE_SAVE_COMPLETE", null, result.text);
		}
		
		/// <summary>
		/// 
		/// </summary>
		private string GetMD5HashString(string unhashed)
		{
			WebPlayerDebugManager.addOutput("Generating MD5 hash value from: \"" + unhashed + "\"", 1);
			byte[] unhashedBytes = System.Text.Encoding.ASCII.GetBytes(unhashed);
			System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
			byte[] hashedBytes = md5Hasher.ComputeHash(unhashedBytes);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < hashedBytes.Length; i++)
			{
				sb.Append(hashedBytes[i].ToString("x2"));
			}
			return sb.ToString();
		}


		public void saveDebugHighscoreSplit (string eventId, string param)
		{
			char[] 	 parameterDelimiters = new char[]{'&'};

			string[] parametersDebug = param.Split (
				parameterDelimiters, 
				System.StringSplitOptions.None);
			
			saveDebugHighscore(float.Parse (parametersDebug[0]), 
			                   float.Parse (parametersDebug[1]), 
			                   float.Parse (parametersDebug[2]), 
			                   float.Parse (parametersDebug[3]),
			                   parametersDebug[4],
			                   parametersDebug[5],
			                   parametersDebug[6],
			                   parametersDebug[7],
			                   parametersDebug[8]);
		}

		/// <summary>
		/// Prints the highscore string to the output window and sends it to the server.
		/// @param socialPoints		score gained on the social account
		/// @param ecologyPoints		score gained on the ecology account
		/// @param economyPoints		score gained on the economy account
		/// @param totalPoints		overall score
		/// @param eMix				String describing the current energy mix like that: Kohle:20%|Gas:10%|... (energy types are in german)
		/// @param energyPrice		current price per kWh in Euro Cent
		/// @param username			user name
		/// @param achievmentList	comma separated URL-encoded list of achievements gained by the user (achievment names are in german)
		/// @param eMixChartImage	base64 String encoding a pie chart which renders the current energy mix
		///[EventHandler(event = "DBModuleEvent.DEBUG_HIGHSCORE_SAVE", properties = "socialPoints, ecologyPoints, economyPoints, totalPoints, eMix, energyPrice, username, achievmentList, image")]
		/// </summary>
		public void saveDebugHighscore (float  socialPoints, 
		                                float  ecologyPoints, 
		                                float  economyPoints, 
		                                float  totalPoints,
		                                string eMix, 
		                                string energyPrice, 
		                                string username, 
		                                string achievmentList, 
		                                string eMixChartImage) 
		{
			bool oldDebugState = debugOutputStopped;
			debugOutputStopped = false;
			addDebugOutput("\n DEBUG save Highscore");
			
			//debug code for rendering the energy mix chart
			byte[] imageBytes = System.Convert.FromBase64String(eMixChartImage);
			//TODO If testing with Score Bar AI Image, change resolution to 277, 277
			Texture2D img = new Texture2D(150, 150);
			img.LoadImage(imageBytes);
			//GameObject.FindGameObjectWithTag("WebDebug").renderer.material.SetTexture ("_MainTex", img);
			
			/* 
			//TODO this works with updating the texture,
			// but it seems the data is corrupt. 
			// Disabled for now, because we set the image when it's been generated
			// and not when it's received.
			GameObject obj = GameObject.FindGameObjectWithTag("WebDebug");
			obj.renderer.material.mainTexture = img;
			obj.renderer.material.color = Color.white;
			obj.renderer.material.shader = Shader.Find("Sprites/Default");
			*/
			// send to backend
			saveHighscore(socialPoints, ecologyPoints, economyPoints, totalPoints, eMix, energyPrice, "debug Highscore", achievmentList, eMixChartImage);
			debugOutputStopped = oldDebugState;
		}

		
		/// <summary>
		/// Send the HTTP request tot he API endpoint using the passed parameters.
		/// @param serviceUrl Path component of the event destination
		/// @param params Parameters of the API request
		/// </summary>
		/// TODO
		private bool sendHTTPRequest (string 			serviceUrl, 
		                              WWWForm 			param, 
		                              ResultCallback 	resultCallback) 
		{
			WebPlayerDebugManager.addOutput("HTTP call initialized", 1);
			// skip requests if there's no base URL
			if (_serviceBaseUrl.Length == 0)
			{
				WebPlayerDebugManager.addOutput("No base URL. HTTP call skipped", 3);
				Log.GameTimes("No base URL. HTTP call skipped");
				return false;
			}			
			
			// skip requests if the session token is set to -1 (offline mode)
			if ((_sessionToken == "-1") && (_protocol != "file://"))
			{
				WebPlayerDebugManager.addOutput("Session token is -1 " + 
				                                "(offline). HTTP call skipped", 3);
				return false;
			}
			
			string url = _protocol + _serviceBaseUrl + serviceUrl;
			//WebPlayerDebugManager.addOutput ("DBModule Request-URL: " + url+"?"+System.Text.Encoding.Default.GetString(param.data), 4);
			Log.GameTimes("DBModule Request-URL: " + url+"?"+System.Text.Encoding.Default.GetString(param.data));
			//http://takomat-bb-dev.stage.endertech.net/api.php/event/game/save?gameData=abc&sessionToken=gfd6tba4d52h9f6n0mdeu8pl36%7c7bacab5a
			
			StartCoroutine(request(url, param, resultCallback));

			//always true, Error Message in coroutine
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		private IEnumerator request (string 		url, 
		                             WWWForm 		param,
		                             ResultCallback resultCallback)
		{
			WebPlayerDebugManager.addOutput("request initialized.", 4);
			WWW service = new WWW(url, param);
			yield return service;

			//To make sure the data is available
			yield return new WaitForSeconds (0.5f);
			if (!string.IsNullOrEmpty (service.error))
			{
				WebPlayerDebugManager.addOutput("Error loading Data.", 4);
				onHTTPServiceFault(service);
				yield break;
			}
			else
			{
				WebPlayerDebugManager.addOutput("Data will be loaded.", 4);
				onHTTPServiceResult(service);
				resultCallback(service);
				yield break;
			}
		}
		

		/// <summary>
		/// Log the successful result of the request.
		/// @param event ResultEvent
		/// </summary>
		private void onHTTPServiceResult (WWW result) 
		{
			WebPlayerDebugManager.addOutput("HTTP Call Result: " +
			                                "\n >error: " + 
			                                result.error +
			                                "\n >bytes downloaded: " + 
			                                result.bytesDownloaded.ToString() +
			                                "\n >message: " + 
			                                result.text +
			                                "\n >isDone: " +
			                                result.isDone.ToString(), 0);
			dbModuleManager.Event ("RESULT", null, null);
		}

		/// <summary>
		/// Log the unsuccessful result of the request.
		/// @param event FaultEvent
		/// </summary>
		private void onHTTPServiceFault(WWW result)
		{
			/* LARS : Excluded for the example
			sendErrorNotification(result.error);
			WebPlayerDebugManager.addOutput("HTTP Call Fault: " +
			                                "\n error: " + 
			                                result.error +
			                                "\n bytes downloaded: " + 
			                                result.bytesDownloaded.ToString() +
			                                "\n message: " + 
			                                result.text, 3);
			dbModuleManager.Event ("FAULT", null, null);
			*/
		}

		/// <summary>
		/// 
		/// </summary>
		private void sendErrorNotification (string errorMessage) 
		{
			dbModuleManager.Event ("ERROR", null, errorMessage);
		}

		/// <summary>
		/// Truncates the given message.
		/// @param msg	message to truncate
		/// </summary>
		private string truncateText (string msg) 
		{
			if (debugMsg.Length != 0)
			{
				int startIndex = debugMsg.Length;
				for (int i = 0; i < MAX_OUTPUT_LINES; i++)
				{
					startIndex = debugMsg.LastIndexOf("\n", startIndex - 1);
				}
				debugMsg = debugMsg.Substring(startIndex, debugMsg.Length);
			}
			return debugMsg;
		}


		/// <summary>
		/// Clears the debug output
		/// </summary>
		private void clearOutput () 
		{
			debugMsg = "";
			//removeDebugEMixImage();
		}


		/// <summary>
		/// Toggles debug output
		/// </summary>
		private void toggleOutput () 
		{
			debugOutputStopped = !debugOutputStopped;
		}
		

		/// <summary>
		/// Adds the givem message to the debug output
		/// @param msg	message to add
		/// </summary>
		private void addDebugOutput (string msg) 
		{
			if (!debugOutputStopped) 
			{
				debugMsg += msg;
				// Log.Info (msg);
			}
		}


		/// <summary>
		/// Loads a config file from an absolute path.
		/// </summary>
		/// <param name="path">Path to the config file.</param>
		/// <param name="resultCall">EventId for the callBackEvent.</param>
		private void loadConfigFile(string path, string resultCall)
		{
			// skip requests if there's no base URL
			if (path.Length == 0)
			{
				WebPlayerDebugManager.addOutput("No path. Config Load call skipped", 3);
				return;
			}			
			
			// skip requests if the session token is set to -1 (offline mode)
			if (resultCall.Length == 0)
			{
				WebPlayerDebugManager.addOutput("No resultCall. Config Load call skipped", 3);
				return;
			}
			
			StartCoroutine(configRequest(path, resultCall));
			return;
		}
		
		/// <summary>
		/// 
		/// </summary>
		private IEnumerator configRequest (string 	url, 
		                                   string	resultCall)
		{
			
			//WebPlayerDebugManager.addOutput ("Config path: " + url + ", CallBack: " + resultCall, 4);
			
			WWW service = new WWW(url);
			yield return service;
			
			//To make sure the data is available
			yield return new WaitForSeconds (1.0f);
			if (!string.IsNullOrEmpty (service.error))
			{
				onHTTPServiceFault(service);
				yield break;
			}
			else
			{
				loadConfigFileResult(service, resultCall);
				yield break;
			}
		}
		
		private void loadConfigFileResult (WWW result, string resultCall) 
		{
			GameManager.Instance.Event(
				"CONFIG",
				resultCall,
				result.text);
			
			addDebugOutput("\n> Load Success");
		}
	}//class
}//namespace  