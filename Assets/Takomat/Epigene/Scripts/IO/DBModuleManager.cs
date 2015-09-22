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

namespace Epigene.IO
{

	public sealed class DBModuleManager
	{
		public static DBModule dbModule 
		{
			get 
			{
				// Log.Debug("----------------------------------------------------- GEt DBModule");
				if (module == null)
				{
					FindOrInitializeDBModule();
				}
				return module;
			}
		}
		private static DBModule module;
		private static GameObject moduleGO = null;

		/// <summary>
		/// Delegate function definition for event handling
		/// </summary>
		public delegate void EventFunction(string eventId, string param);
		
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

		private Dictionary<int, List<EventObject>> eventList;

//------------------------------------------------------------------------------

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static DBModuleManager Instance
		{
			get
			{
				//if (module == null) findOrInitializeDBModule();
				return instance;
			}
		}
		private static readonly DBModuleManager instance = new DBModuleManager();


	/// <summary>
		/// ctor
		/// </summary>
		private DBModuleManager()
		{
			eventList = new Dictionary<int, List<EventObject>>();

			// FindOrInitializeDBModule();	
		}//DBModuleManager()

		private static void FindOrInitializeDBModule()
		{
			//Debug.Log("DBModule not set. Searching...");
			
			if(moduleGO == null )
				moduleGO = new GameObject();

			GameObject go = GameObject.FindGameObjectWithTag("DBModule");
			if (go == null)
			{
				Log.Debug("DBModule not found. Creating...");
				
				moduleGO.AddComponent<DBModule>();
				moduleGO.name = "DBModule";
				moduleGO.tag = "DBModule";
				module = moduleGO.GetComponent<DBModule>();
				
				Log.Debug("DBModule created and set. Returning...");
			}
			else
			{
				moduleGO = go;
				module = moduleGO.GetComponent<DBModule>();
				Log.Debug("DBModule found. Returning...");
			}
		}

//---------- basic unity API MonoBehavior class methods ------------------------
/*
		/// <summary>
		/// Process triggers
		/// </summary>
		public void Update()
		{
			// Debug.Log("dbm update()");
			//DBModule.Update();
		}//Update()
/**/
//---------- Interface ---------------------------------------------------------

		/// <summary>
		/// Delegate events to registred functions
		/// </summary>
		public void Event(string type, string eventId, string param = "")
		{
			Log.Info("<color=yellow>Event("+type+") fired: </color>" + eventId+", "+param);
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
				
				Log.Info("number of events:"+eList.Count);
				
				foreach(EventObject ev in eList)
				{
					EventFunction fn = ev.function;

					// Debug.Log("fn:"+ev.function+" go:"+ev.gameObject);//+ " act:");

					if(fn != null
					   && ( ev.gameObject == null 
					    || (ev.gameObject != null && ev.gameObject.active)))
					{
						Debug.Log("EVENT found and fired!");
						fn(eventId, param);
					}
				}
			}
			else
			{
				Log.Debug("Sent event ("+eventId+"), but no listener found for type: "+type);
			}
			
		}//Event()

		/// <summary>
		/// Register a funtion handler to an event.
		/// </summary>
		public void RegisterEventHandler(string eventType, EventFunction func, GameObject handler = null)
		{
			//TODO add funct to dict if eventType exist.

			int id = eventType.GetHashCode();
			
			if(eventList.ContainsKey(id))
			{
				//Log.Info("RegisterEventHandler as existing:"+ eventType);
				List<EventObject> eList = eventList[id];
				eList.Add(new EventObject(handler, func));				
				
			}
			else
			{
				//Log.Info("RegisterEventHandler as new:"+ eventType);
				//add to list
				List<EventObject> eList = new List<EventObject>();
				eList.Add(new EventObject(handler, func));
				eventList.Add(id, eList);
				
			}
		}
		
		public void RemoveEventHandler(string eventType, EventFunction func)
		{
			Log.Info("RemoveEventHandler:"+ eventType);
			
			int id = eventType.GetHashCode();
			if(eventList.ContainsKey(id))
			{
				List<EventObject> eList = eventList[id];
				foreach(EventObject e in eList)
				{
					if(e.function == func)
					{
						Log.Info("REMOVED EVENT");
						//TODO might need another dropList here
						eList.Remove(e);
						return;						
					}
				}
			}			
		}

		public void Clear()
		{
			Debug.Log("Clear DBModuleManager");

			module = null;
			moduleGO = null;
			eventList.Clear();
			
		}

	}//class GameManager

}//namespace GameManager