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
using Epigene.MODEL;
using Epigene.UI;
using Epigene.IO;
using Epigene.AUDIO;

namespace Epigene.GAME
{


	/// <summary>
	/// Delegate function definition for event handling
	/// </summary>
	//public delegate void EventFunction(GameEvent type, string eventId, string param);
	public delegate void EventFunction(string eventId, string param);
	
//------------------------------------------------------------------------------
	/// <summary>
	/// GMGame manager class to manage the game play via triggers.
	/// Current design is, that only one game is running.
	/// A Game state is not implemented. Instead we use messages
	/// going around informing all relevant objects about the changes in the 
	/// game. Plus every object has its own state.
	/// </summary>
	public sealed class GameManager
	{
		/// <summary>
		/// class to hold important info for each event
		/// </summary>
		class EventObject
		{
			public string type;
			public GameObject gameObject;
			public EventFunction function;

			public EventObject(GameObject obj, EventFunction fn, string t = "")
			{
				gameObject = obj;
				function = fn;
				type = t;
			}

		}

		/// <summary>
		/// debug mode switch
		/// use it for get additional info about the game
		/// like current story and dialog id in bubble.
		/// </summary>
		public bool debugMode = false;
		public bool BalanceSheetAchievementMode = false;
		public bool startAchievementsMode = false;
		public bool standaloneBetaMode = false;
		public bool standaloneSingleLevelMode = false;

		/// <summary>
		/// Muse music or sound for start
		/// </summary>
		public bool muteSfx = false; // SFX 
		public bool muteSoundBackground = false; // Background Sound loops
		
		/// <summary>
		/// if device has HID driver or not
		/// </summary>
		public bool hidTouch = false;

		public bool setTouch = true;

		//If the game is running offline
		public bool offline;


		/// <summary>
		/// Reference / link to the real game object
		/// which runs during the whole game.
		/// This is the entry point for the game.
		/// </summary>
		public MainGame mainGame;

		/// <summary>
		/// Reference / link to the DialogGame
		/// Currently we have only ONE Dialog handling Game
		///TODO Maybe later we have several.
		/// </summary>
		public GMGame DialogGame
		{
			get { return dialogGame;}
			set { dialogGame = value;}
		}		
		private GMGame dialogGame;


		/// <summary>
		/// list of games associated with it's name 
		/// </summary>
		private Hashtable gameList;


		/// <summary>
		/// Event receiver to add delegates
		/// </summary>
		//private EventFunction eventHandler;

		/// <summary>
		/// List of registered events.
		/// Key = hashcode of the event string
		/// Value = delegate holds references to assigned functions
		/// </summary>		
		//private Dictionary<int, EventFunction> eventList;
		private Dictionary<int, List<EventObject>> eventList;

		public int screenWidth;
		public int screenHeight;

//------------------------------------------------------------------------------
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static GameManager Instance
		{
			get{ return instance;}
		}
		private static readonly GameManager instance = new GameManager();

		/// <summary>
		/// ctor
		/// </summary>
		private GameManager()
		{
			Log.Debug("GameManager initialized.");
			AUDIO.AudioManager.Instance.MuteMusic = true;


			gameList = new Hashtable();
			eventList = new Dictionary<int, List<EventObject>>();

			LoadEpigeneProperties();

			// also receive Events
			RegisterEventHandler("RESET", ResetEventHandler);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other 
		/// cleanup operations before the <see cref="GameManager"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~GameManager()
		{
			//TODO anything else to do with games here?
			gameList.Clear();
		}

//-------------------------------- basic unity API MonoBehavior class methods ---

		/// <summary>
		/// Process triggers
		/// </summary>
		public void Update()
		{
			ICollection games = gameList.Values;

			foreach( GMGame g in games)
			{
				if(g.IsRunning) 
				{//// Log.GameTimes("gm update()"+ g.Name);
					g.Update();
				}
			}
		
			// If OS Application : 
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
			
		}//Update()

		/// <summary>
		/// Adds the trigger to the current game.
		/// For smoothly "Remove trigger" execute "ATrigger.Repeat = 0;"
		/// Because that is a Remove, which can occur within trigger 
		/// execution. Otherwise you get an error.
		/// </summary>
		/// <param name="newTrigger">New trigger.</param>
		public void Add(ATrigger trigger)
		{
			Add(DialogGame.Name, trigger);
		}//Add(Trigger)

		/// <summary>
		/// Adds the trigger. 
		/// For smoothly "Remove trigger" execute "ATrigger.Repeat = 0;"
		/// Because that is a Remove, which can occur within trigger 
		/// execution. Otherwise you get an error.
		/// </summary>
		/// <param name="newTrigger">New trigger.</param>
		public void Add(string gameName, ATrigger trigger)
		{
			Log.Info("-------------- Add trigger to game:" + gameName);
			
			//if a gamename not specified use the current active one
			GMGame game = (GMGame)gameList[gameName];

			if(game == null)
			{
				Log.Error("Game not exist:"+gameName);
				Log.Error("Games :"+gameList.Count);
				for(int i =0; i < gameList.Count; i++)
					Log.Info("i:"+gameList[i]);
				
			}

			game.AddTrigger(trigger);
		}//Add(gameName, Trigger)
		
		/// <summary>
		/// Adds the trigger.
		/// </summary>
		/// <param name="newTrigger">New trigger.</param>
		public void Add(GMGame newGame)
		{
			Log.Debug("Add game '"+newGame.Name+"' to GameManager.");
			//add new element
			try
			{
				if(newGame == null)
					Log.Error("You must pass a valid game object!");
				
				gameList.Add(newGame.Name, newGame);
			}
			catch
			{
				
				string name = (newGame != null) ? newGame.Name : "";
				Log.Warning("GMGame already exist:" + name);
			}
			
		}//Add()

		/// <summary>
		/// Get an instance of the game if its exist
		/// </summary>
		/// <param name="name">Name of game</param>
		public GMGame Get(string name)
		{
			if (!gameList.ContainsKey(name))
				return null;
			else
				return (GMGame)gameList[name];
		}//Get

		/// <summary>
		/// Removes the game specified by the name.
		/// If the name is "*" it will remove all games.
		/// </summary>
		/// <param name="name">Name.</param>
		public void Remove(string name)
		{
			if(name == "*")
			{
				gameList.Clear();
			}
			else
			{
				if(gameList.Contains(name))
				{
					gameList.Remove(name);
					Log.Debug("GMGame '"+name+"' has been removed!");
				}
				else
					Log.Warning("No game to remove:"+name);
			}
		}

//--------------- Interface like an Audio Player --------------------

		/// <summary>
		/// Start the game. The next update will process triggers for this game only.
		/// If the game not exist, it will log warning and return false.
		/// If another game already started, it will NOT send stop(),
		/// but pause it, while set the new one as current game.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <returns>true if game(s) started successfully</returns>
		public bool Start(string name)
		{
			bool success = false;
			if(name == "*")
			{
				ICollection games = gameList.Values;
				if(games.Count == 0 )
				{
					Log.Warning("There is no games to start!");
					return false;
				}
				foreach( GMGame g in games)
					g.Start();
				success = true;
			} 
			else if(gameList.Contains(name))
			{
				success = ((GMGame)gameList[name]).Start();
			}
			else
			{
				Log.Error("Cannot start game which does not exist:"+name);
				
			}
			return success;
			
		}//Start()

		/// <summary>
		/// Stop the specified name.
		/// If the name is empty string, it will stop the current game.
		/// If the name is "*" it will stop all games.
		/// If the name is specify one game, it will stop only that one.
		/// Errors will be logged as warning.
		/// </summary>
		/// <param name="name">Name of the game</param>
		/// <returns>true if ok, false in case of any error.</returns>
		public bool Stop(string name)
		{
			if(name == "*")
			{
				ICollection games = gameList.Values;
				if(games.Count == 0 )
				{
					Log.Warning("There is no games to stop!");
					return false;
				}
				foreach( GMGame g in games)
					g.Stop();
			}
			else
			{
				//stop game by name
				if(gameList.Contains(name))
				{
					((GMGame)gameList[name]).Stop();
					return true;
				}
				else
					Log.Warning("There is no game to stop:"+name);

			}

			return false;
		}//Stop()

		/// <summary>
		/// Pause or resume the current game.
		/// </summary>
		/// <param name="flag">If true, 
		/// it will resume the game, if false it will  pause it</param>
		public void Pause(string name)
		{
			if(gameList.Contains(name))
			{
				((GMGame)gameList[name]).Pause();
			}
		}

		/// <summary>
		/// Pause or resume the current game.
		/// </summary>
		/// <param name="flag">If true, 
		/// it will resume the game, if false it will  pause it</param>
		public void Play(string name)
		{
			if(gameList.Contains(name))
			{
				((GMGame)gameList[name]).Play();
			}
		}

//----------------------------------- 

		/// <summary>
		/// Load a game
		/// </summary>
		public bool Load()
		{
			//TODO implement
			Log.Warning("Load not implemented!");
			return false;

		}//Load()

		/// <summary>
		/// Save a game with it's state and triggers
		/// </summary>
		public bool Save()		
		{
			//TODO implement
			Log.Warning("Save not implemented!");

			return false;
		}//Save()

		/// <summary>
		/// Load the specified fileName from the Resource directory as string
		/// </summary>
		/// <param name="fileName">File name and path realtive to Resource directory.</param>
		public static string LoadText(string fileName)
		{
			TextAsset txt = Resources.Load<TextAsset>(fileName);
			if(txt == null)
			{
				throw new System.Exception("No file to load:"+fileName);
			}
			else
			{
				Log.Debug("Load text file: <color=yellow>"+fileName+"</color>");
				return txt.ToString();
			}

		}//LoadText()

		/// <summary>
		/// Add DialogTriggers from json file
		/// </summary>
		public void AddDialogFromFile(string fileName)
		{

			// release old one
			RemoveTriggers(TriggerType.DIALOG);
			
			
			// string jsonString = LoadText(fileName);
			// Dictionary<string,object> dict = 
			// 	MiniJSON.Json.Deserialize(jsonString) as Dictionary<string,object>;
			Dictionary<string, object> dict = LoadJSon(fileName);

			//create dialog triggers for each dialogs			
			foreach(Dictionary<string,object> d in ((List<object>)dict["dialogs"]))
			{
				DialogTrigger dt = new DialogTrigger(d);
				dt.ConnectEventHandler(true);
				Add(dt);
			}
		}//AddDialogFromFile()

		// /// <summary>
		// /// Remove all trigger with given type
		// /// </summary>
		public void RemoveTriggers(TriggerType type)
		{

			foreach( DictionaryEntry data in gameList)
			{
				GMGame g = (GMGame)data.Value;								
				g.RemoveTriggers(TriggerType.DIALOG);			
			}
		}

		/// <summary>
		/// Remove all triggers from active game.
		/// </summary>
		public void RemoveTriggers()
		{
			foreach( DictionaryEntry data in gameList)
			{
				GMGame g = (GMGame)data.Value;				
				g.RemoveTriggers();
			}
		}
		/// <summary>
		/// Load settings from JSon file
		/// </summary>
		public static Dictionary<string, object> LoadJSon(string fileName)
		{
			string jsonString = LoadText(fileName);
			Dictionary<string,object> dict = 
				MiniJSON.Json.Deserialize(jsonString) as Dictionary<string,object>;

			return  dict;
		}//LoadJSon();

		/// <summary>
		/// Save settings to JSon file
		/// Should be limitted for editor mode only. (file access)
		/// </summary>
		public static void SaveJSon(Dictionary<string, object> data, string fileName)
		{
			//TODO: add save
			//MiniJSON.JSon.Serialize(data);
			

		}//SaveJSon();

		/// <summary>
		/// Delegate events to registred functions
		/// </summary>
		public void Event(string type, string eventId, string param = "")
		{
			// Log.Info("<color=yellow>Event("+type+") fired: </color>" + eventId+", "+param);
			//if(eventHandler != null && eventId != null)
			//	eventHandler(type, eventId, param);
			
			int id = type.GetHashCode();
			if(eventList.ContainsKey(id))
			{
				//call each events within the same type
				List<EventObject> origList = eventList[id];

				List<EventObject> eList = new List<EventObject>(origList);
				//origList.CopyTo(eList);
				//eList.CopyFrom(origList);

				//Log.Info("number of events:"+eList.Count);

				foreach(EventObject ev in eList)
				{
					EventFunction fn = ev.function;
					if(fn != null
						&& ( ev.gameObject == null 
						  || (ev.gameObject != null && ev.gameObject.active)))
					{
						fn(eventId, param);
					}
				}
			}
			else
			{
				Log.Debug("Sent event ("+eventId+"), but no listener found for type: "+type);
			}
			
		}//Event()

		//TODO not sure if we need this!!
		//
		// /// <summary>
		// /// Register a funtion handler to an event.
		// /// </summary>
		// public void RegisterEvent(string eventType)
		// {
		// 	//TODO add funct to dict if eventType exist.
		// 	//
			
		// 	int id = eventType.GetHashCode();
		// 	//EventFunction fn = null;
		// 	eventList.Add(id, null);
		// }

		/// <summary>
		/// Register a funtion handler to an event.
		/// </summary>
		public void RegisterEventHandler(string eventType, EventFunction func, GameObject handler = null)
		{
			//TODO add funct to dict if eventType exist.
			
			int id = eventType.GetHashCode();

			if(eventList.ContainsKey(id))
			{
				Log.Debug("RegisterEventHandler as existing:"+ eventType);
				List<EventObject> eList = eventList[id];
				eList.Add(new EventObject(handler, func));
				// eList[id] = new EventObject(handler, func);

			}
			else
			{
				Log.Debug("RegisterEventHandler as new:"+ eventType);
				//add to list
				List<EventObject> eList = new List<EventObject>();
				eList.Add(new EventObject(handler, func));
				eventList.Add(id, eList);
				
			}
		}

		public void RemoveEventHandler(string eventType, EventFunction func)
		{
			Log.Debug("RemoveEventHandler:"+ eventType);

			int id = eventType.GetHashCode();
			if(eventList.ContainsKey(id))
			{
				List<EventObject> eList = eventList[id];
				foreach(EventObject e in eList)
				{
					if(e.function == func)
					{
						//Log.Debug("REMOVED EVENT");
						//TODO might need another dropList here
						eList.Remove(e);
						return;						
					}
				}
			}
			
		}

		public void ResetEventHandler(string eventId, string data)
		{
			/*// Log.GameTimes("GameManager " + eventType+ ": EVENT("+eventType+"): " 
			              + eventId+","+data);
			*/
			
			if(eventId=="Game")
			{
				AUDIO.AudioManager.Instance.MuteMusic = true;
				//Application.LoadLevel(Application.loadedLevelName);
				Stop("*");
				GameManager.Instance.Event("RESET", "Models", "");
				// Reset all model, which have not owner
				Emails.Instance.Mails.Clear();
				UIManager.Instance.Clear();

				MessagePopups.Instance.XClick = false;

				// start main game, other games are started when needed;
				Start(DialogGame.Name);
				UIManager.Instance.ActivateScreen(mainGame.startScreenName);
			}
		
		}

		public void ClearEvents()
		{
			eventList.Clear();
		}

		/// <summary>
		/// Loads the scene.
		/// </summary>
		/// <param name="level">Level.</param>
		/// <param name="_setTouch">Optional Parameter, set it true if you need it true. else leave it blank</param>
		public void LoadScene(string level, bool _setTouch = false)
		{
			Debug.Log("******************************** Load scene:"+level+" **********************************");

			// UIManager.Instance.ClearScreens();
			//force to drop everything
			eventList.Clear();
			// RemoveTriggers();
			UIManager.Instance.Clear(true);
			DBModuleManager.Instance.Clear();
			
			Application.LoadLevel(level);			

			Debug.Log("Level loaded..");
			RegisterEventHandler("RESET", ResetEventHandler);

			setTouch = _setTouch;
		}


		private void LoadEpigeneProperties()
		{
			//default values
			screenWidth = 1008;
			screenHeight = 756;

//TODO this needs to be fixed later, because each game screen resolution is specific
#if !UNITY_WEBPLAYER
			string propFile = System.IO.Directory.GetCurrentDirectory() + "/Assets/epigene-properties.json";
			
			Dictionary<string, object> properties = new Dictionary<string, object>();
			if (System.IO.File.Exists(propFile))
			{
				string txt = System.IO.File.ReadAllText(propFile);
				if(txt != null)
				{
	 				properties = MiniJSON.Json.Deserialize(txt) as Dictionary<string, object>;
				}
			}

			if (properties.ContainsKey("screenWidth"))
			{
				screenWidth = int.Parse(properties["screenWidth"].ToString());
			}
			if (properties.ContainsKey("screenHeight"))
			{
				screenHeight = int.Parse(properties["screenHeight"].ToString());
			}
#endif
		}

	}//class GameManager

}//namespace GameManager
