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
using Epigene.VIEW;


namespace TWISTBueBe.GAME
{
	/// <summary>
	/// GMGameSIM Class adds to GMGame
	/// virtual Game Time and specific handling 
	/// for the SIM Game
	/// </summary>
	public class GMGameSIM : GMGame
	{
		public  DateTime	date;			 // actual date
		public  float 		value    = 0.0f; // progress
		public  float		maxValue = 0.0f; // maxValue in seconds, defined by Duration and minutes per Week
		public  float 		timeStep = 0.1f; // seconds till next tick	
		private float		lastTime = 0.0f; // GameTime at last tick
		private float		actTime  = 0.0f; // GameTime at the tick
		private double		oldSpeed = 0; 	 //memorize old Speed when Paused
		private TimeLine    timeLine; 
		private string		timeLineText;
		private GameObject	simMenu;
		private GameObject	scoreBoard;

		private ScriptTrigger<int> 	triggerSIMTime;

		//TODO Get these Variables from GameScenario.xml (Jonas)
		private DateTime	startDate		= DateTime.Now;
		private float		duration 		= 1*365;	//in days
		private float		minutesPerYear 	= 2;// 15;	//15 minutes
		

		public override DateTime Time ()
		{
			return date;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.GMGameSIM"/> class.
		/// </summary>
		/// <param name="gameName">Name of the game, used as an id for reference</param>
		public GMGameSIM (string name) : base (name)
		{
			//WebPlayerDebugManager.addOutput ("Game " + Name + " Constructed!", 1);
		}//ctor()
		
		/// <summary>
		/// All Triggers of the game by default/start
		/// </summary>
		/// <returns>true if game started</returns>
		public override void StartSetup() 
		{


			//Initiates Variables
			if (GameScenario.Instance.simStartYear == 0)
				startDate		= GameScenario.Instance.currentDate;
			else
				startDate		= new DateTime(GameScenario.Instance.simStartYear,
				                          	   GameScenario.Instance.simStartMonth,
				                          	   GameScenario.Instance.simStartDay);
			TimeSpan span 	= startDate.AddYears ((int)GameScenario.Instance.simDurationInYears) - startDate;
			duration 		= (float) span.TotalDays;	//in days
			//WebPlayerDebugManager.addOutput (duration + " Days from " + startDate + " till " + startDate.AddYears((int)GameScenario.Instance.simDurationInYears), 1);
			minutesPerYear 	= (float)P0000_GameParams.Instance.MinutesPerYear;

			GameObject obj = UIManager.Instance.GetPopup("SaveSimPopup");
			if(obj)
			{
				obj.SetActive(false);
			}

			GameManager.Instance.RegisterEventHandler("TIMELINE", ProcessTimelineEvent);

			simMenu = UIManager.Instance.GetHud ("SimulationMenu");
			simMenu.SetActive (true);
			scoreBoard = UIManager.Instance.GetHud ("ScoreBoard");
			scoreBoard.SetActive (true);
			date = startDate;

			GameObject objSIM = UIManager.Instance.GetHud("TimeLineSIM");			
			timeLine = objSIM.GetComponent<TimeLine>();

			maxValue = duration * (minutesPerYear * 60 / 365);
			timeLine.SetTimeBar(0,0,maxValue);

			//Edit TimelineSIM Dates
			//TODO When shall this start?
			string text = "";
			text = I18nManager.Instance.Get ("Timeline", "003").Replace("SIMTAG1", startDate.Month + "." + startDate.Year);
			I18nManager.Instance.Set ("Timeline", "003", text);
			text = I18nManager.Instance.Get ("Timeline", "004").Replace("SIMBAU", startDate.AddDays (duration).Month + "." + startDate.AddDays (duration).Year);
			I18nManager.Instance.Set ("Timeline", "004", text);

			//Starts the Timeline
			GameManager.Instance.Event ("TIMELINE", "SIM", "Activate");
			GameManager.Instance.Event ("TIMELINE", "SIM", "Start");
		}

		public override void OnPlay ()
		{
			if (simMenu == null)
				return;
			Speed = oldSpeed;
			lastTime = (float) TimeSource.Time;
			simMenu.SetActive (true);
			scoreBoard.SetActive (true);

			timeLine.isActive = true;   
			timeLine.VisibleWhenInScreen = true;
			timeLine.gameObject.SetActive(true);
		}

		public override void OnStop ()
		{
			if (simMenu != null)
				simMenu.SetActive (false);
			if (scoreBoard != null)
				scoreBoard.SetActive (false);
		}

		public override void OnPause ()
		{
			if (simMenu == null)
				return;

			oldSpeed = Speed;
			Speed = 0;
			simMenu.SetActive (false);
			scoreBoard.SetActive (false);

			timeLine.isActive = false;   
			timeLine.VisibleWhenInScreen = false;
			timeLine.gameObject.SetActive(false);

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
		/// UpdateSIMTime :
		/// </summary>
		public void UpdateSIMTime(int param)
		{
			//WebPlayerDebugManager.addOutput("TICK " + Speed, 1);

			if ((value > maxValue) && (value != 0))
			{
				WebPlayerDebugManager.addOutput("Show BalanceSheet",1);
				GameObject obj = UIManager.Instance.GetPopup("BalanceSheetPopup");
				if(obj)
					obj.SetActive(true);
				
				GameManager.Instance.Event("POPUP_BALANCE", "001", "show");



				//GameManager.Instance.Event("POPUP_SAVESIM", "", "show");
				RemoveTrigger(triggerSIMTime);
				timeLine.isActive = false; 
				timeLine.VisibleWhenInScreen = false; 
				timeLine.gameObject.SetActive(false);
				return;
			}

			int lastYear  = date.Year;
			int lastMonth = date.Month;
 
			actTime = (float) TimeSource.Time;
			value  += (float) Speed * (actTime - lastTime);
			lastTime = actTime;

			date = startDate.AddDays (value / maxValue * duration);
			timeLineText = date.Day.ToString("D2") + "." + date.Month.ToString("D2") + "." + date.Year;
			timeLine.TimeValue = value;
			timeLine.Text = timeLineText;

			if (date.Month != lastMonth)
			{
				//WebPlayerDebugManager.addOutput("NEW MONTH", 1);
				GameManager.Instance.Event("TIMELINE",
				                           "SIM",
				                           "NewMonth");
			}
			if (date.Year != lastYear)
			{
				//WebPlayerDebugManager.addOutput("NEW YEAR", 1);
				GameManager.Instance.Event("TIMELINE", "SIM", "NewYear");
			}

		}//UpdateSIMTime()
		
		public void ProcessTimelineEvent(string eventId, string param)
		{
			if (eventId != "SIM") return;

			//WebPlayerDebugManager.addOutput(eventId + " " + param, 2);

			switch (param)
			{
			case "Pause":	   	if (Speed != 0) oldSpeed = Speed;
								Speed = 0;	
								timeLine.PauseSlider();
								break;
			case "Unpause":	   	Speed = oldSpeed;
								timeLine.UnpauseSlider();
								simMenu.SetActive (true);
								scoreBoard.SetActive (true);
								break;	
			case "Activate":   	timeLine.isActive = true;   
								timeLine.VisibleWhenInScreen = true;
								timeLine.gameObject.SetActive(true);
								break;
			case "Start":	   	{
									ACondition relTimeCondSIM = 
										new TimerCondition(TimerType.Relative,
										                   timeStep, TimeSource);  //true after 1sec
									
									triggerSIMTime = 
										new ScriptTrigger<int>(1, -1, 
										                       relTimeCondSIM.Check,
										                       null, UpdateSIMTime, 1); 
									
									AddTrigger(triggerSIMTime);
									
									//WebPlayerDebugManager.addOutput ("Trigger added!", 1);
									Speed = 1;
									timeLine.StartSlider();
								}
								break;
			case "Restart":	   	{
									WebPlayerDebugManager.addOutput("restarting timeline SIM", 2);
									value = 0;

									ACondition relTimeCondSIM = 
										new TimerCondition(TimerType.Relative,
										                   timeStep, TimeSource);  //true after 1sec
									
									triggerSIMTime = 
										new ScriptTrigger<int>(1, -1, 
										                       relTimeCondSIM.Check,
										                       null, UpdateSIMTime, 1); 
									
									AddTrigger(triggerSIMTime);
									
									//WebPlayerDebugManager.addOutput ("Trigger added!", 1);

									oldSpeed = 1;
									date = startDate;

									Speed = 0;	
									timeLine.PauseSlider();

									actTime = (float) TimeSource.Time;
									lastTime = actTime;

								}
								break;
			case "Deactivate": 	timeLine.isActive = false; 
								timeLine.VisibleWhenInScreen = false; 
								timeLine.gameObject.SetActive(false);
								break;
			case "Stop":	   	if (timeLine.isActive) 
									timeLine.StopSlider();
								RemoveTrigger(triggerSIMTime);
								break;
			case "Speed_1":	   	Speed = 1;
								break;
			case "Speed_2":	   	Speed = 2;
								break;
			case "Speed_4":	   	Speed = 4;
									break;
			}
		}
		
	}//class GMGameSIM
}//namespace GameManager
