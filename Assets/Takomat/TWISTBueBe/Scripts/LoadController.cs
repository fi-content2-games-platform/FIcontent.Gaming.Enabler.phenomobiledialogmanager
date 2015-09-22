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
using System;
using Epigene.MODEL;
using Epigene;
using Epigene.GAME;
using System.Collections.Generic;
using UnityEngine;
using Epigene.Network;
using Epigene.IO;

// ______________________________

namespace TWISTBueBe
{
	/// <summary>
	/// LoadController . This class responsible to manage all load events
	/// and directing information to the correct classes.
	/// Disabling this script will disable any loading processes.
	/// </summary>
	public class LoadController : MonoBehaviour
	{
		public bool AutoReplayPlayerActions
		{
			set { autoReplayPlayerActions = value; }
			get { return autoReplayPlayerActions; }
		}
		private bool autoReplayPlayerActions = false;

		public bool ResetSave
		{
			set { resetSave = value; }
			get { return resetSave; }
		}
		private bool resetSave = false;

		public Dictionary<string, string>  ReplayInfo
		{
			set { replayInfo = value; WebPlayerDebugManager.addOutput(" replayInfo set: "+replayInfo.Count, 2); }
			get { return replayInfo; }
		}
		private Dictionary<string, string>  replayInfo = new Dictionary<string, string>();

		private string tempInfoStorage = "";
		private int countOutgoingEvents = 0;
		private GameManager gameManager = GameManager.Instance;
		private GameScenario gameScenario = GameScenario.Instance;

		public LoadController() {
			gameManager.RegisterEventHandler(
				"GAME_LOAD_COMPLETE", ProcessLoadEvent);

			tempInfoStorage = "";
			countOutgoingEvents = 0;
			gameManager = GameManager.Instance;
			gameScenario = GameScenario.Instance;
		}
		
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		private static LoadController instance;
		public static LoadController Instance
		{
			get
			{
				if (instance == null){
					GameObject obj = new GameObject();
					instance = obj.AddComponent<LoadController>();
				}
				return instance;
			}
		}
		
		/// <summary>
		/// Destroy this object
		/// </summary>
		public void Exit() 
		{
			gameManager.RemoveEventHandler(
				"GAME_LOAD_COMPLETE", ProcessLoadEvent);
		}//Exit()

		/// <summary>
		/// Event handlers
		/// </summary>
		public void ProcessLoadEvent(string eventId, string param)
		{
			// eventId has to be load and loadingId (e.g. load3)
			if(!eventId.StartsWith("load"))
				return;

			WebPlayerDebugManager.addOutput("autoReplayPlayerActions = "+autoReplayPlayerActions,2);
			
			// nextEventId is saved as: AdressedClass&NecessaryFunction
			string nextEventId = "";
			
			// if new load is initialized, count has to be reset
			if(eventId.Replace("load", "") == "0")
				countOutgoingEvents = 0;
			
			int value = 0;

			if(resetSave == true)
			{
				Epigene.IO.DBModuleManager.Instance.Event(
					"GAME_SAVE",
					"",
					param);
			}
			// if it is a normal load of a game status
			else if(autoReplayPlayerActions == false)
			{
				// e.g.: PIPE&238.9609&301.9727&0;
				if(int.TryParse(param.Split('&')[0].Replace("E", ""), out value) == false && !param.Contains("Epigene"))
				{
					nextEventId = "Simulation&BuildConstruction&";
					param += "&load";
					WebPlayerDebugManager.addOutput("Simulation information found: "+param, 1);
				}
				// e.g.: E0001&020151203123456789&&0;
				else if(param.StartsWith("E") && !param.Contains("Epigene"))
				{
					nextEventId = "FinanceManager&addConstruction&";
					WebPlayerDebugManager.addOutput("FinanceManager information found: "+param, 1);
				}

				WebPlayerDebugManager.addOutput(nextEventId+countOutgoingEvents+" - "+param, 1);

				if (nextEventId != "") {
					gameManager.Event(
						"GAME_LOAD_COMPLETE", 
						nextEventId+countOutgoingEvents, 
						param);
					
					countOutgoingEvents++;
				}
			}
			// if the Game should be automatic replay of all Player actions
			else if(autoReplayPlayerActions == true)
			{
				// e.g.: PIPE&238.9609&301.9727&0;
				if(int.TryParse(param.Split('&')[0].Replace("E", ""), out value) == false && !param.Contains("Epigene"))
				{
					nextEventId = "FinanceManager&storeInfo&";
					WebPlayerDebugManager.addOutput("Simulation information found: "+param, 1);
				}
				// e.g.: E0001&20151209123456789&&0;
				else if(param.StartsWith("E") && !param.Contains("Epigene"))
				{
					tempInfoStorage = param;
					WebPlayerDebugManager.addOutput("FinanceManager information found: "+param, 1);
				}
				
				if (nextEventId != "") {
					gameManager.Event(
						"GAME_LOAD_COMPLETE", 
						nextEventId+countOutgoingEvents, 
						tempInfoStorage+"&"+param);
					
					countOutgoingEvents++;
				}
			}
		}
		
		
		/// <summary>
		/// Updates
		/// </summary>
		public void Update()
		{
			// only activate when replayInfo is not empty & replay activated
			if(replayInfo.Count > 0 && AutoReplayPlayerActions == true)
			{
				string possibleKey = gameManager.Get("SIM").Time().ToString("yyyyMMdd");
				string value = "";
				//WebPlayerDebugManager.addOutput("Update: possibleKey = "+possibleKey, 2);
				
				// check if DateTime overlaps
				if(replayInfo.TryGetValue(possibleKey, out value) == true)
				{
					WebPlayerDebugManager.addOutput("Update: "+replayInfo[possibleKey], 2);
					string allParam = replayInfo[possibleKey];
					string[] arrParam = allParam.Split('&');
					gameManager.Event(
						"GAME_LOAD_COMPLETE", 
						"FinanceManager&addConstruction&"+replayInfo.Count, 
						arrParam[0]+"&"+arrParam[1]+"&"+arrParam[2]+"&"+arrParam[3]);
					gameManager.Event(
						"GAME_LOAD_COMPLETE", 
						"Simulation&BuildConstruction&"+(replayInfo.Count+1), 
						arrParam[4]+"&"+arrParam[5]+"&"+arrParam[6]+"&"+arrParam[7]+"&");
					
					replayInfo.Remove(possibleKey);
				}
			}
		}
	}
}

