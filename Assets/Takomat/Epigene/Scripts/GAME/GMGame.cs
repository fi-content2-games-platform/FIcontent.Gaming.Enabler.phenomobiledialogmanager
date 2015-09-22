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
using System.Collections.Generic;
using System.Linq;

namespace Epigene.GAME
{
	/// <summary>
	/// GMGame Class encapsulate a game state and f
	/// controls the associated triggers execution.
	/// Every GMGame object represents one game, with a set of triggers.
	/// GMGame acts as a normal Audio Player concerning the states
	/// Start, Stop, Pause, Play
	/// </summary>
	public class GMGame
	{
		/// <summary>
		/// Gets or sets the real-time TimeSource
		/// </summary>
		/// <value>The TimeSource</value>
		public TimeSource TimeSource
		{
			get {return timeSource;}
			set {timeSource = value;}
		}
		private TimeSource timeSource;
		public bool pause = false;
		/// <summary>
		/// Name of this game.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get {return name;}
		}
		private string name;

		//Standard: Today
		public virtual DateTime Time()
		{
			return DateTime.Now;
		}

		/// <summary>
		/// Gets or sets the speed of the Game
		/// , which defines how much faster or slower
		/// than the real-time Timesource
		/// the game is running.
		/// </summary>
		/// <value>The speed.</value>
		public double Speed
		{
			get {return speed;}
			set {speed = value;
				//if(timeSource!=null) timeSource.Speed = speed;
			}
		}
		private double speed;

		public int MinPriority
		{
			get {return minPriority;}
			set {minPriority = value;}
		}
		private int minPriority;
		
		public int MaxPriority
		{
			get {return maxPriority;}
			set {maxPriority = value;}
		}
		private int maxPriority;

		public bool IsRunning
		{
			get {return isRunning;}
		}

		private bool isRunning;

		public bool IsStarted
		{
			get {return isStarted;}
		}
		
		private bool isStarted;

		private int myCounter;

		private List<ATrigger> triggerList;
		private List<ATrigger> triggerListToRemove;
		private List<ATrigger> triggerListToAdd;		

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.GMGame"/> class.
		/// </summary>
		/// <param name="gameName">Name of the game, used as an id for reference</param>
		public GMGame (string gameName)
		{
			minPriority = 0;
			maxPriority = 10;
			Speed = 1.0f;
			isRunning = false;
			name = gameName;

			timeSource = new TimeSource ();
			triggerList = new List<ATrigger>();
			triggerListToRemove = new List<ATrigger>();
			triggerListToAdd = new List<ATrigger>();
			myCounter = 0;
		}//ctor()


		~GMGame()
		{
			// garbage collection is done by another thread
			// so nothing to do here
			// everything is garbage collected
		}


		/// <summary>
		/// All Triggers of the game by default/start
		/// </summary>
		/// <returns>true if game started</returns>
		public virtual void StartSetup()
		{
		}
		/// <summary>
		/// Start the game will start to process associated triggers.
		/// </summary>
		/// <returns>true if game started</returns>
		public virtual bool Start()
		{
			//// Log.GameTimes(name + " STARTED started..");
			timeSource.Start();
			isRunning = true;
			isStarted = true;
			StartSetup(); // all standard setups, e.g. game triggers
			return isRunning;
		}//Start()

		/// <summary>
		/// Stop this game. The game won't process any triggers.
		/// </summary>
		public void Stop()
		{
			// Debug.Log (this.name + "Trigger Counter : " + myCounter);
			Speed = 1.0f;
			timeSource.Stop();

			isRunning = false;
			isStarted = false;
			// only generally all DIALOG triggers are removed 
			// other trigger can
			RemoveTriggers();

			List<ATrigger> dropList = new List<ATrigger>();

			//iterate over each active one
			foreach(ATrigger trigger in triggerList)
			{
				if(triggerListToRemove.IndexOf(trigger)!=-1)
				{
					triggerListToRemove.Remove(trigger);
					dropList.Add (trigger);
				}
			}

			//remove dropped triggers
			myCounter -= dropList.Count;
			triggerList.RemoveAll (x => dropList.Contains(x));


			//Debug.Log (this.name + "Trigger Counter : " + myCounter);
			//Debug.Log (this.name + "Trigger Counter : " + triggerList.Count);
			//Debug.Log (this.name + "Trigger Counter : " + triggerListToAdd.Count);
			//Debug.Log (this.name + "Trigger Counter : " + triggerListToRemove.Count);

			// send out information about stop
			GameManager.Instance.Event ("TIMELINE",
			                            name,
			                            "Deactivate");
			GameManager.Instance.Event ("TIMELINE",
			                            name,
			                            "Stop");

			// Log.GameTimes(name + " stopped..");
			OnStop ();
		}//Stop()

		//Additional Playfunction for inherited classes
		public virtual void OnStop()
		{
			return;
		}

		public void Play()
		{
			if (!pause)	return;
			WebPlayerDebugManager.addOutput("Play(" + name + ")",1);
			pause = false;
			//timeSource.Pause = pause;
			isRunning = true;		
			GameManager.Instance.Event ("TIMELINE",
			                            name,
			                            "Activate");
			GameManager.Instance.Event ("TIMELINE",
			                            name,
			                            "Unpause");

			WebPlayerDebugManager.addOutput("TimeSource State: " + timeSource.Pause.ToString(),1);

			OnPlay ();
		}

		//Additional Playfunction for inherited classes
		public virtual void OnPlay()
		{
		}

		public void Pause()
		{ 
			if(pause) return;
			WebPlayerDebugManager.addOutput("Pause(" + name + ")",1);
	  	    pause = true;
			//timeSource.Pause = pause;	
			isRunning = false;
			OnPause ();
		}

		//Additional Pausefunction for inherited classes
		public virtual void OnPause()
		{
		}

		public void Update()
		{
			if(!isRunning) return;
			//// Log.GameTimes(name + " update..");
			//add new triggers
			foreach(ATrigger t in triggerListToAdd)
			{ //// Log.GameTimes(name + " update..Add");
				triggerList.Add(t);
				myCounter++;
			}
			triggerListToAdd.Clear();

			//// Log.GameTimes(name + " update..Count"+ triggerList.Count);
		
			List<ATrigger> dropList = new List<ATrigger>();
			
			//order list by priority
			triggerList = 
				(from t in triggerList orderby t.Priority descending select t).ToList();

			//iterate over each active one
			foreach(ATrigger trigger in triggerList)
			{
				if(triggerListToRemove.IndexOf(trigger)!=-1)
				{
					triggerListToRemove.Remove(trigger);
					dropList.Add (trigger);
					trigger.ConnectEventHandler(false);
				}
				//only use triggers which are at minimum priority level
				else if(trigger.Priority >= minPriority && trigger.Priority <= maxPriority)
				{
					//fire the trigger.. 
					//Debug.Log (" - fire trigger prio:"+trigger.Priority);
					trigger.Fire();			
					//remove it if needed
					if( trigger.FireDelete() )
						dropList.Add (trigger);
				}
			}
			//remove dropped triggers
			myCounter -= dropList.Count;
			triggerList.RemoveAll (x => dropList.Contains(x));
		}//Update()

		/// <summary>
		/// Add one trigger.
		/// </summary>
		/// <param name="newTrigger">New trigger.</param>
		public void AddTrigger(ATrigger newTrigger)
		{

			//dont use triggerList directly, because this could cause
			//problems when a trigger try to add another trigger.
			triggerListToAdd.Add(newTrigger);
			//sort it by priority
			//triggerList = (from t in triggerList orderby t.Priority descending select t).ToList();
			
		}//AddTrigger()

		/// <summary>
		/// Removes the trigger.
		/// </summary>
		/// <param name="oldTrigger">Old trigger.</param>
		public void RemoveTrigger(ATrigger oldTrigger)
		{
			triggerListToRemove.Add(oldTrigger);
		}//RemoveTrigger()

		/// <summary>
		/// Remove all triggers
		/// </summary>
		public void RemoveTriggers()
		{
			triggerListToAdd.Clear();
			triggerListToRemove.AddRange(triggerList);
			
		}//RemoveTriggers()

		/// <summary>
		/// Remove all triggers with given type
		/// </summary>
		public void RemoveTriggers(TriggerType type)
		{

			//Log.Info("REMOVE ALL TRIGGERS BY TYPE:"+type+" count:"+triggerList.Count);

			//add existing triggers to remove list,
			//this is safe even when a trigger call this function.			
			foreach(ATrigger t in triggerList)
			{
				//Log.Info("t="+t+" type:"+t.Type);

				if(t.Type == type)
				{
					triggerListToRemove.Add(t);
					//Log.Info("Remove trigger:"+t);
				}
			}
			
			//remove triggers from "add" list
			List<ATrigger> temp = new List<ATrigger>();
			foreach(ATrigger t in triggerListToAdd)
			{
				if(t.Type == type)
				{
					temp.Add(t);
				}
			}

			foreach(ATrigger t in temp)

			{
				triggerListToAdd.Remove(t);
				myCounter--;
			}

			System.GC.Collect();
			
			//triggerListToAdd.Clear();
			//triggerListToRemove.AddRange(triggerList);

		}//RemoveAllTrigger()



	}//class GMGame
}//namespace GameManager