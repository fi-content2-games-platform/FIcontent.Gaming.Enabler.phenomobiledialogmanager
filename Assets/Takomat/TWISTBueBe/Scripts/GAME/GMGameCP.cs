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

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//--------------------- Epigene
using Epigene;
using Epigene.GAME;
using Epigene.MODEL;
using Epigene.UI;

using TWISTBueBe;
using Epigene.IO;

namespace TWISTBueBe.GAME
{
	/// <summary>
	/// GMGameCP Class adds to GMGame
	/// virtual Game Time and specific handling 
	/// for the CP Game
	/// </summary>
	public class  GMGameCP : GMGame
	{
		public  DateTime	date; 			 // actual date
		public  float 		value    = 0.0f; // progress
		public  float		maxValue = 0.0f; // maxValue in seconds, defined by Duration and minutes per Week
		public  float 		timeStep = 0.1f; // seconds till next tick	
		private float		lastTime = 0.0f; // GameTime at last tick
		private float		actTime  = 0.0f; // GameTime at the tick
		private TimeLine    timeLine; 
		private string		timeLineText;
		
		private ScriptTrigger<int> 	triggerCPTime;
		private ACondition 			relTimeCondCP;
		
		// Virtual Game Time is setuped here: 
		//
		// We set a Time with startDate
		// e.g. startYear=2014, startMonth=05, StartDay=01.
		// Hours, minutes are assumed to be 0
		// Out of the virtual time the game runtime in seconds for the TimeSource
		// is calculated
		
		//TODO Get these Variables from GameScenario.xml (Jonas)
		private DateTime	startDate		= DateTime.Now;
		private float		duration 		= 42; // in days
		private float		minutesPerWeek 	= 5;	//5 minutes
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.GMGameCP"/> class.
		/// </summary>
		/// <param name="gameName">Name of the game, used as an id for reference</param>
		public GMGameCP (string name) : base (name)
		{
			//WebPlayerDebugManager.addOutput ("Game " + Name + " Constructed!", 1);
		}//ctor()
		
		public override DateTime Time ()
		{
			return date;
		}

		public override void OnPlay ()
		{
			lastTime = (float) TimeSource.Time;
			Speed = 1;

			timeLine.isActive = true;   
			timeLine.VisibleWhenInScreen = true;
			timeLine.gameObject.SetActive(true);
			//GameObject.SetActive (true);
		}
		
		public override void OnStop ()
		{
			//GameObject.SetActive (false);
		}
		
		public override void OnPause ()
		{
			Speed = 0;

			timeLine.isActive = false;   
			timeLine.VisibleWhenInScreen = false;
			timeLine.gameObject.SetActive(false);
			//GameObject.SetActive (false);
			
			/*
			GameManager.Instance.Event (GameEvent.TIMELINE,
			                            name,
			                            "Pause");
			GameManager.Instance.Event (GameEvent.TIMELINE,
			                            name,
			                            "Deactivate");
			WebPlayerDebugManager.addOutput("TimeSource State: " + timeSource.Pause.ToString(),1);*/
		}
		
		/// <summary>
		/// All Triggers of the game by default/start
		/// </summary>
		/// <returns>true if game started</returns>
		public override void StartSetup() 
		{
			//create trigger CPTime
			startDate		= GameScenario.Instance.currentDate.AddDays(2);
			duration 		= (float) GameScenario.Instance.cpDurationInDays;//float.Parse(GameConfiguration.Instance.GetValue("cpDurationInDays")); // in days
			minutesPerWeek 	= (float) P0000_GameParams.Instance.MinutesPerWeek;	//5 minutes

			GameManager.Instance.RegisterEventHandler("TIMELINE", ProcessTimelineEvent);
			
			date = startDate;
			
			GameObject objCP = UIManager.Instance.GetHud("TimeLineCP");			
			timeLine = objCP.GetComponent<TimeLine>();
			Log.Assert(timeLine, "Missing TimeLine");
			
			maxValue = duration * (minutesPerWeek * 60 / 7);

			timeLine.SetTimeBar(0,0,maxValue);

			
			//Edit TimelineCP Dates
			string text = "";
			text = I18nManager.Instance.Get ("Timeline", "001").Replace("TAG1", startDate.Day + "." + startDate.Month);
			I18nManager.Instance.Set ("Timeline", "001", text);
			text = I18nManager.Instance.Get ("Timeline", "002").Replace("BAU", startDate.AddDays(duration).Day + "." + startDate.AddDays(duration).Month);
			I18nManager.Instance.Set ("Timeline", "002", text);
			
			//UpdateCPTime (0);

			
			GameManager.Instance.Event ("TIMELINE", "CP", "Activate");
			GameManager.Instance.Event ("TIMELINE", "CP", "Start");
			
		}
		
		
		/// <summary>
		/// UpdateCPTime :
		/// </summary>
		public void UpdateCPTime(int param)
		{
			if ((value > maxValue) && (value != 0))
			{
				WebPlayerDebugManager.addOutput("UPDATE CP TIME:\n" + " value: " + value + "\n maxValue: " + maxValue, 1);
				GameManager.Instance.Event("TIMELINE", "CP", "Stop");
				GameManager.Instance.Event("TIMELINE", "CP", "Deactivate");
				GameManager.Instance.Event("TIMELINE", "CP", "Finished");
				return;
			}

			actTime = (float) TimeSource.Time;
			value  += (float) Speed * (actTime - lastTime);
			lastTime = actTime;
			date = startDate.AddDays (value / maxValue * duration);
			timeLineText = date.Day.ToString("D2") + "." + date.Month.ToString("D2");

			timeLine.Text = timeLineText;
			timeLine.TimeValue = value;

			/*
			Log.Info ("Update CPTime: " + time);
			WebPlayerDebugManager.addOutput ("Update CPTime: " + time,1);
			if ((value != 0) && (timeLine.isActive))
				time += 1;
			timeLineText = startDate.AddDays(time).Day + "." + startDate.AddDays(time).Month;
			slider.ValueText = timeLineText;
			
			timeLine.UpdateSlider(time);
			*/
		}//UpdateCPTime()
		
		public void ProcessTimelineEvent(string eventId, string param)
		{

			if (eventId != "CP") return;
			
			//WebPlayerDebugManager.addOutput(eventId + " " + param, 2);
			
			switch (param)
			{
			case "Pause":	Speed = 0;
				timeLine.PauseSlider();
				break;
			case "Unpause":	Speed = 1;
				timeLine.UnpauseSlider();
				break;	
			case "Activate":   	timeLine.isActive = true;   
				timeLine.VisibleWhenInScreen = true;
				timeLine.gameObject.SetActive(true);
				timeLine.Show(true);
				break;
			case "Start":	  
				
				relTimeCondCP = 
					new TimerCondition(TimerType.Relative,
					                   timeStep, TimeSource);  //true after timestep

				triggerCPTime = 
					new ScriptTrigger<int>(1, -1, 
					                       relTimeCondCP.Check,
					                       null, UpdateCPTime, 1); 
				AddTrigger(triggerCPTime);

				lastTime = (float) TimeSource.Time;
				timeLine.StartSlider();
				break;
			case "Deactivate": 	timeLine.isActive = false; 
				timeLine.VisibleWhenInScreen = false; 
				timeLine.gameObject.SetActive(false);
				timeLine.Show(false);
				break;
			case "Stop":	   	RemoveTrigger(triggerCPTime);
				timeLine.StopSlider();
				break;
			}
		}

	}//class GMGameCP 
}//namespace GameManager

/*
namespace TWISTBueBe.GAME
{
	/// <summary>
	/// GMGameCP Class adds to GMGame
	/// virtual Game Time and specific handling 
	/// for the CP Game
	/// </summary>
	public class  GMGameCP : GMGame
	{
		public  float 		time     = 0.0f; // ticks till started
		public  float 		timeStep = 1f;   // seconds till tick			
		private TimeLine    timeLine; 
		private Slider	    slider;
		private string		timeLineText;

		private ScriptTrigger<int> 	triggerCPTime;
		private ACondition 			relTimeCondCP;

		// Virtual Game Time is setuped here: 
		//
		// We set a Time with startDate
		// e.g. startYear=2014, startMonth=05, StartDay=01.
		// Hours, minutes are assumed to be 0
		// Out of the virtual time the game runtime in seconds for the TimeSource
		// is calculated

		//TODO Get these Variables from GameScenario.xml (Jonas)
		private DateTime	startDate		= GameScenario.Instance.currentDate;
		private float		duration 		= 40; // in days
		private float		minutesPerWeek 	= 0.8f;	//5 minutes


		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.GMGameCP"/> class.
		/// </summary>
		/// <param name="gameName">Name of the game, used as an id for reference</param>
		public GMGameCP (string name) : base (name)
		{
			//WebPlayerDebugManager.addOutput ("Game " + Name + " Constructed!", 1);
		}//ctor()

		public override float Time ()
		{
			return time;
		}

		/// <summary>
		/// All Triggers of the game by default/start
		/// </summary>
		/// <returns>true if game started</returns>
		public override void StartSetup() 
		{
			//create trigger CPTime
			//TODO enable this
			GameManager.Instance.AddEventHandler(EventHandler);

			time = 0.0f;
			
			GameObject objCP = UIManager.Instance.GetHud("TimeLineCP");			
			timeLine = objCP.GetComponent<TimeLine>();

			slider = timeLine.slider.GetComponent<Slider> ();
			slider.minValue = 0;
			GameObject bar  = GameObject.Find("bar_Passing_Timeline_CP");	
			if (bar != null)
			{
				WebPlayerDebugManager.addOutput (bar.renderer.bounds.size.x.ToString(), 1);
				//TODO Calculate this with the vertical lines bar
				slider.width	= bar.renderer.bounds.size.x - 0.66f; //We're not starting at the very edge
			}
			slider.MaxValue = duration;
			slider.timelineType = TimeLine.TimelineType.CP;

			//Edit TimelineCP Dates
			string text = "";
			startDate = startDate.AddDays (2);	//2 days needed for talking with citizens
			text = I18nManager.Instance.Get ("Timeline", "001").Replace("TAG1", startDate.Day + "." + startDate.Month);
			I18nManager.Instance.Set ("Timeline", "001", text);
			text = I18nManager.Instance.Get ("Timeline", "002").Replace("BAU", startDate.AddDays(duration).Day + "." + startDate.AddDays(duration).Month);
			I18nManager.Instance.Set ("Timeline", "002", text);
			
			UpdateCPTime (0);

			GameManager.Instance.Event (GameEvent.TIMELINE, "CP", "Activate");
			GameManager.Instance.Event (GameEvent.TIMELINE, "CP", "Start");

		}


		/// <summary>
		/// UpdateCPTime :
		/// </summary>
		public void UpdateCPTime(int value)
		{
			Log.Info ("Update CPTime: " + time);
			WebPlayerDebugManager.addOutput ("Update CPTime: " + time,1);
			if ((value != 0) && (timeLine.isActive))
				time += 1;
			timeLineText = startDate.AddDays(time).Day + "." + startDate.AddDays(time).Month;
			slider.ValueText = timeLineText;

			timeLine.UpdateSlider(time);
		}//UpdateCPTime()

		public void EventHandler(GameEvent eventType,
		                         string eventId,
		                         string param)
		{
			if(eventType == GameEvent.TIMELINE)
			{
				if (eventId != "CP") return;

				WebPlayerDebugManager.addOutput(eventId + " " + param, 2);

				switch (param)
				{
					case "Pause":	   	timeLine.PauseSlider();
									   	TimeSource.Pause = true;
										WebPlayerDebugManager.addOutput(triggerCPTime.Repeat.ToString(), 1);
									   	break;
					case "Unpause":	   	timeLine.UnpauseSlider(); 
									   	TimeSource.Pause = false;
										WebPlayerDebugManager.addOutput(triggerCPTime.Repeat.ToString(), 1);
									   	break;	
					case "Activate":   	timeLine.isActive = true;   
									   	timeLine.VisibleWhenInScreen = true;
									   	timeLine.gameObject.SetActive(true);
									   	break;
					case "Start":	   	timeStep = minutesPerWeek / 7 * 60;

										relTimeCondCP = 
											new TimerCondition(TimerType.Relative,
											                   timeStep, TimeSource);  //true after timestep

										//TODO enable an endless mode
										triggerCPTime = 
											new ScriptTrigger<int>(1, -1, 
											                       relTimeCondCP.Check,
											                       null, UpdateCPTime, 1); 
										AddTrigger(triggerCPTime);

										WebPlayerDebugManager.addOutput ("time source state: " + TimeSource.Pause.ToString(), 1);
										timeLine.StartSlider();
									   	break;
					case "Deactivate": 	timeLine.isActive = false; 
									   	timeLine.VisibleWhenInScreen = false; 
									   	timeLine.gameObject.SetActive(false);
									   	break;
					case "Stop":	   	RemoveTrigger(triggerCPTime);
										timeLine.StopSlider();
									   	break;
				}
			}
		}		
	}//class GMGameCP 
}//namespace GameManager
*/