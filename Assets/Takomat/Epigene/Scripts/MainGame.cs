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

//using Epigene;
using Epigene.GAME;
using Epigene.MODEL;
using Epigene.UI;
using Epigene.VIEW;
using Epigene.AUDIO;
using Epigene.Network;


//TODO devide this later to TWISTBueBe
//using TWISTBueBe;

//------------------------------------------------------------------------------
//namespace TWISTBueBe
using Epigene.IO;


namespace Epigene
{
	/// <summary>
	/// Start the game. Add each screens 
	/// to the UIManager and set the first one to active.
	/// Each screen have to manage their own logic.
	/// This class is only a start point for the game
	/// and does only the basic initialization.
	/// It stays also by restarting a game.
    /// Restarting a Game is nothing else than switching 
    /// Again to the start screen and before that 
    /// reset all the a game specific values.
    /// See further reading 
	/// Use this for logics which is not specific for any screen
	/// like log levels or language for the whole game.
	/// You can always alter from these in every screen
	/// 
	/// The Layer semantic in TWISTBueBe : 
	/// starting with "User Layer 8 HUD"
	/// 
	/// HUD        : All elements of head up display
	/// Player     : All elements representing the Player itself in the game 
	///              , which is the scoreBar for example
	/// NPC        : All NPC visuals
	/// Video      : IntroMovie 
	/// Dialog     : Dialog Layer
	/// Navigation : 
	/// Map        : simulation Map of the city we take care of
	/// Intro      : Screen Layer definition all 
	/// CityHallOffice  : CoordLady office
	/// Wall            :  vor
	/// Popup           :  
	/// TimeLineCP      : Civiv Participitation 
	/// TimeLineSim     : Simulation 
	/// ArtistCommunity : HotSpot Dialog Place  1 
	/// GrandPaFlat     : HotSpot Dialog Place  2
	/// CityHall        : 
	/// HotSpots        : In the map we have HotSpots
	/// DadFlat         : HotSpot Dialog Place  3
	/// ScoreResult     : 
	/// </summary>
	public class MainGame : MonoBehaviour 
	{
		protected UIScreen screen;

		NetworkLayer networkLayer;

		//define the name of the screen you want to start
		public string startScreenName = "Screen1";
		public Camera camera2D;

		//language for the game
		public enum Languages {EN, DE, NL, DEP}; //TODO: change to dynamic definition from localisation.txt
		public Languages language = Languages.EN;
		//set log level from editor
		public Log.LogLevel logLevel = Log.LogLevel.ALL;

		//set debug mode
		public bool debug 	= false;
		public bool startAchievements = false;
		public bool standaloneBeta = false;
		public bool offline	 = false;
		public bool standaloneSingleLevel = false;

		/// <summary>
		/// Muse music or sound for start
		/// </summary>
		public bool muteSfx = false; // SFX 
		public bool muteSoundBackground = false; // Background Sound loops

		/// <summary>
		/// if device has HID driver or not
		/// </summary>
		public bool hidTouch = false;

		//public backgroundMusic

		public static string audioResources 	= "Audio";
		public static string configResources 	= "Config/";
		public static string localizatoinDB 	= "Config/localisation";
#if EPIGENE_UI_46
        public static string hudPrefabs         = "Prefabs/HUD46/";
#else
		public static string hudPrefabs 		= "Prefabs/HUD/";
#endif
		public static string popupPrefabs		= "Prefabs/";
		public static string modelPrefabs		= "Models/";

		public UITooltip tooltip;

		private string externalFilesPath = "";
		public	string ExternalFilesPath
		{
			get { return externalFilesPath; }
			set
			{
				if(AppConfiguration.Instance.exFilesPathIsAbsolute)
					externalFilesPath = value;
				else //add Apllication data Path
					externalFilesPath = 
						Application.dataPath + "/" + value;
				WebPlayerDebugManager.addOutput("Changed external Files Path to " +
				                                externalFilesPath, 1);
			}
		}

		//Set by AppConfiguration.xml, leave blank to use dummys
		private string externalResourcePath = "";
		public	string ExternalResourcePath
		{
			get { return externalResourcePath; }
			set 
			{ 
				if(AppConfiguration.Instance.exResourcePathIsAbsolute)
					externalResourcePath = value;
				else //add Apllication data Path
					externalResourcePath = 
						Application.dataPath + "/" + value;
				WebPlayerDebugManager.addOutput("Changed external Resource Path to " +
				                                externalFilesPath, 1);
			}
		}


//------------------------------------------------------------------------------

		// Webpage interface 				
		public bool useDBModule = true;

		public string userParameters = 
			"?userName=Bernard&sessionToken=LordOfTheRings";
		
		// private static Dictionary<string, string> 
			// parameters = new Dictionary<string, string>(); 
		private Dictionary<string, string> parameters;			

		// END
//------------------------------------------------------------------------------
		

		//internal data
		protected GameManager gm;
		protected UIManager   ui;
		protected I18nManager i18n;
		protected AudioManager am;

		private GameObject[] screens;
		private Object[] objects;
		private UIScreen startScreen;


		private bool m_fullscreenswitched = false;
		private bool m_fullscreensecond = false;

		private bool received = false;
	
		// Use this for initialization
		public virtual void Awake() 
		{
			Log.Debug("__________________________________ AWAKE _____________________________");

			parameters = new Dictionary<string, string>();


			// if(useDBModule)
			// {
			// 	// StartCoroutine("xmlLoadRoutine");
			// 	LoadAppConfigurationXml();
			// }
				
			//testParameters instead of document.location.search 
			// for testing Web Player: Variables in PHP are
			// $UserName and $SessionToken

			Log.Level = logLevel;
			Log.Info("Starting MainGame ... name "+this.name);

			ui = UIManager.Instance;			

			ui.InitUI(gameObject);

			if (!GameManager.Instance.offline)
				GameManager.Instance.offline = offline;
			else
				offline = GameManager.Instance.offline;

			
			ui.ConfigResources = configResources;
			if(tooltip)
				ui.Tooltip = (UITooltip)Instantiate(tooltip, Vector3.zero, Quaternion.identity);
			
			//set ui camera to custom 2D camera
			Log.Assert(camera2D != null, "You have to assign a 2D camera in "+gameObject.name);
			UIManager.UICamera = camera2D;

			gm = GameManager.Instance;
			gm.mainGame  = this;
			gm.debugMode = debug;
			gm.standaloneBetaMode = standaloneBeta;
			gm.standaloneSingleLevelMode = standaloneSingleLevel;
			gm.BalanceSheetAchievementMode = true;
			gm.muteSfx = muteSfx;
			if(gm.setTouch)
				gm.hidTouch = hidTouch;
			else
				hidTouch = gm.hidTouch;
			gm.muteSoundBackground = muteSoundBackground;

			//localisation
			
			i18n = I18nManager.Instance;
			i18n.LoadDbFile(localizatoinDB);

			if(i18n.Language == null || i18n.Language.Length == 0)
			{
				//first time set
				string lang = "";
				switch(language) 
				{
					case Languages.DE:
						lang = "DE";
						break;
					case Languages.DEP:
						lang = "DEP";
						break;
					case Languages.EN:
						lang = "EN";
						break;
					case Languages.NL:
						lang = "NL";
						break;
				}
			
				i18n.Language = lang;
			}
			else
			{
				//lang has already been set,
				//but better to set local game's language to avoid confusion
				switch(i18n.Language)
				{
					case "DE":
						language = Languages.DE;
						break;
					case "DEP":
						language = Languages.DEP;
						break;
					case "EN":
						language = Languages.EN;
						break;
					case "NL":
						language = Languages.NL;
						break;
				}
				
				I18nManager.Instance.SetLanguage(i18n.Language);
			}


			//Audio using localization db
			am = AudioManager.Instance;
			am.InitAudio();
			try
			{
				// Log.Debug("---------- am:"+am);
				Log.Debug("---------- audioResources:"+audioResources);
				am.LoadResources("AUDIO", audioResources);
				am.MuteMusic = muteSoundBackground;
				am.MuteSfx = muteSfx;
			}
			catch
			{
				//TODO only hide when in editor mode
				//we hide error for now due to editor
				Log.Error("---------- audioResources:"+audioResources);
			}

			// DialogGame can be seen as a MainGame and a requirement
			GMGame game = new GMGame(this.name);
			gm.Add(game);
			gm.DialogGame = game;



		}//Awake()

		// Start the game
		public virtual void Start()
		{
			Log.Debug("__________________________________ START _____________________________");
			Log.Debug("<color=green>STARTING GAME...</color>");


			if(useDBModule)
			{
				// StartCoroutine("xmlLoadRoutine");
				//initicalize dbmodule
				DBModule dbmodule = DBModuleManager.dbModule;
				LoadAppConfigurationXml();
			}	
		}//Start()

		public void OnEnable()
		{
			Log.Debug("__________________________________ ENABLE _____________________________");
			GameManager.Instance.RegisterEventHandler(
				"IPCONFIGED", ProcessIPConfiged);

			GameManager.Instance.RegisterEventHandler(
				"CONFIG", ProcessConfig);

			//start the game before Start()
			Enter();

			//dont start the game before other object's awake
			gm.Start(this.name);
		}

		public void OnDisable()
		{
			GameManager.Instance.RemoveEventHandler(
				"IPCONFIGED", ProcessIPConfiged);
			GameManager.Instance.RemoveEventHandler(
				"CONFIG", ProcessConfig);
			Exit();
		}

		/// <summary>
		/// TODO make them abstract later
		/// </summary>
		public virtual void Enter()
		{
			Log.Debug("Overwrite this plz");
		}
		public virtual void Exit()
		{
			Log.Debug("Overwrite this plz");
		}

//------------------------------------------------------------------------------

		//Update UI and game logic
		public virtual void Update()
		{

			//TODO works only once
			// use case for the retina display
			// all other MACs should work
			#if UNITY_STANDALONE_OSX
			if (Screen.fullScreen || m_fullscreensecond) {
				if (!m_fullscreenswitched && !m_fullscreensecond) {
					Screen.fullScreen = false;
					m_fullscreensecond = true;
				} else if (!m_fullscreenswitched && m_fullscreensecond) {
					Screen.fullScreen = true;
					m_fullscreenswitched = true;
				}
			}
			#endif

			if(gm != null)
				gm.Update();
			//else Log.Info ("Update MainGame");
			if(ui != null)
				ui.Update();

			if(!received && useDBModule)
			{
				///WTF????
				Application.ExternalEval(
					" UnityObject2.instances[0].getUnity().SendMessage('" 
					+ name + "', 'SetRequestParameters', userParameters);"
					);

				// , sessionToken
				string userName = "LarsEditor";
				string sessionToken = userParameters;
				#if UNITY_EDITOR
				SetRequestParameters(userParameters); // , sessionToken);
				#elif UNITY_STANDALONE || UNITY_IOS
				SetRequestParameters("?userName=user&sessionToken=-1");
				#endif
				//Log.Info("in here");
				//Log.Info("in here");

				//Emails.Instance.Add(new Mail(
				//	"TOTO", "evaluation process: "
				//	+userName+", "+sessionToken, "23. SEPTEMBER", "sender"));
			}
			
		}//Update()
		
//---------- Webpage interface to send and retrieve data ----------------------
		/*
	PreviewLabs.RequestParameters

	Public Domain
	
	To the extent possible under law, PreviewLabs has waived all copyright 
	and related or neighboring rights to this document. 
	This work is published from: Belgium.
	
	http://www.previewlabs.com
	
*/

		private void LoadAppConfigurationXml()
		{
			WebPlayerDebugManager.addOutput("Loading App Config...", 1);
			AppConfiguration  appConfig  = AppConfiguration.Instance;
			appConfig.Load();
		}
//------------------------------------------------------------------------------

		public bool HasKey(string key)
		{
			return parameters.ContainsKey (key);
		}
		
		// This can be called from Start(), but not earlier
		public string GetValue(string key)
		{
			return parameters[key];
		}

//------------------------------------------------------------------------------	
		public static string GetUniqueIdentifier()
		{
			System.Guid uid = System.Guid.NewGuid();
			return uid.ToString();	
		}

		public void SetRequestParameters(string parametersString) 
		{
			if(!useDBModule)
				return;

			Log.Debug(">>>>>>>>>>>>>>> SetRequestParameters "+parametersString);

			received = true;

			char[] parameterDelimiters = new char[]{'?', '&'};
			string[] ps = parametersString.Split (
							parameterDelimiters, 
							System.StringSplitOptions.RemoveEmptyEntries);
			
			
			char[] keyValueDelimiters = new char[]{'='};
			for (int i=0; i<ps.Length; ++i)
			{
				string[] keyValue = ps[i].Split (
					keyValueDelimiters, System.StringSplitOptions.None);
				Log.Info("<color=green>evaluation process </color>"
				         +keyValue[0]+", "+keyValue[1]);
				if (keyValue.Length >= 2)
				{
					if(parameters.ContainsKey(keyValue[0]))
					{
						parameters[keyValue[0]] = keyValue[1];
					}
					else
					{
						parameters.Add(
							WWW.UnEscapeURL(keyValue[0]), 
							WWW.UnEscapeURL(keyValue[1]));
					}
				}
				else if (keyValue.Length == 1)
				{
					if(parameters.ContainsKey(keyValue[0]))
					{
						parameters[keyValue[0]] = "";
					}
					else
					{
						parameters.Add(
							WWW.UnEscapeURL(keyValue[0]), "");
					}
				}	
			}
		}


		/// <summary>
		/// Helper to create a gameobject from a prefab.
		/// </summary>
		public static GameObject CreateGameObject(string path)
		{

			//create a new hud from prefab.
			//string path = hudPrefabs+prefab;
			Log.Info("Create GameObject from:"+path);

			GameObject obj = (GameObject)Resources.Load(path);
			if(!obj)
			{
				Log.Error("Can't find prefab to load:"+path);
			}

			return (GameObject)Instantiate(obj);
		
		}//CreateGameObject()


		private void ProcessConfig(string eventId, string data)
		{
			if (eventId == "APP_CONFIGURATION_LOADED")
			{
				WebPlayerDebugManager.addOutput("Event Call \n ipConfig Path: " + AppConfiguration.Instance.ipConfigPath +
				                                "\n Initialized: " + IPAppConfiguration.Instance.initialized, 1);
				if ((AppConfiguration.Instance.ipConfigPath != "") && (!IPAppConfiguration.Instance.initialized))
				{
					WebPlayerDebugManager.addOutput("Start Loading IP Config " + AppConfiguration.Instance.ipConfigPath, 1);
					IPAppConfiguration.Instance.Load (AppConfiguration.Instance.ipConfigPath);
				}
				else					
					GameManager.Instance.Event("CONFIG", "IPAPP_CONFIGURATION_LOADED" ,"");
			}
			else if (eventId == "IPAPP_CONFIGURATION_LOADED")
			{	
				WebPlayerDebugManager.addOutput(
					"Loading Game Config...", 1);
				if (!GameConfiguration.Instance.initialized)
				{
					GameConfiguration gameConfig = GameConfiguration.Instance;
					gameConfig.Load();
				}
				else
					GameManager.Instance.Event("CONFIG", "ScenarioLoad", "");
			}
		}

		// Processing Events
		private void ProcessIPConfiged(string eventId, string data)
		{

			Debug.Log(">>>>>>>>>>> ProcessIPConfiged >>>>>>>>>>>");
			networkLayer = null;
			GameObject obj1 = GameObject.Find("/Epigene/Networking");
			if ((obj1 != null) && (obj1.activeSelf != false))
			{
				networkLayer = obj1.GetComponent<NetworkLayer>();
			}
			if(networkLayer == null)
			{
				Debug.LogWarning("Missing networkLayer!!");
				return;
			}

			// bind our process function to network
			if(networkLayer)
			{
				//disconnect if connected
				networkLayer.gameObject.SetActive(false);
				//update values
				networkLayer.myIP 			= IPAppConfiguration.Instance.myIp;
				networkLayer.gameServerIP 	= IPAppConfiguration.Instance.serverIp;
				networkLayer.serverPort		= int.Parse(IPAppConfiguration.Instance.serverPort);
				networkLayer.listenerPort	= int.Parse(IPAppConfiguration.Instance.myPort);
				networkLayer.headerFlag		= bool.Parse(IPAppConfiguration.Instance.headerFlag);
				
				WebPlayerDebugManager.addOutput(
					"My IP: " + networkLayer.myIP +
					"Server: " + networkLayer.gameServerIP +
					"Port: " + networkLayer.serverPort, 1);
			}
			//reconnect
			networkLayer.gameObject.SetActive(true);
		}


		// // test function to notify us when screen changes
		// public void notifyScreenChanged(
		// 	UIScreen oldScreen, UIScreen newScreen)
		// {
		// 	string tmp = (oldScreen != null) ? oldScreen.Name : "NONE";
		// 	Log.Debug("Swtich screen <color=cyan>"
		// 	          +tmp+" -> "+newScreen.Name+"</color>");
		// }//notifyScreenChanged()

	}//class MainGame

}//namespace
//------------------------------------------------------------------------------
