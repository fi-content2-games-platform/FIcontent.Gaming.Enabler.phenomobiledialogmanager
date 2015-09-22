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
using System.Collections;
using System.Collections.Generic;
using Epigene;


//------------------------------------------------------------------------------
//namespace TWISTBueBe
using Epigene.GAME;


namespace TWISTBueBe
{
	public class WebPlayerDebugManager : Epigene.WebPlayerDebugManager
	{		


		override protected void sendHighscore(string totalscore) {

		}

		void check()
		{
			Epigene.IO.DBModuleManager.Instance.Event(
				"CHECK_FOR_SAVED_GAME", 
				"", 
				"");
		}

		void load()
		{
			Epigene.IO.DBModuleManager.Instance.Event(
				"GAME_LOAD", 
				"", 
				"");
		}

		void achieve() {
			Epigene.MODEL.Z0000_GameObjectives mZ0000_GameObjectives = Epigene.MODEL.Z0000_GameObjectives.Instance;
			
			string uuid = "";
			for (int i=1; i<28; i+=1)
			{
				switch (i)
				{
				case  1: uuid = mZ0000_GameObjectives.activeCitizenUUID; break;				 	 //Z1001
				case  2: uuid = mZ0000_GameObjectives.initParticipationUUID; break;			 	 //Z1002
				case  3: uuid = mZ0000_GameObjectives.requestPlanUUID; break;				 	 //Z1003
				case  4: uuid = mZ0000_GameObjectives.initPostponementUUID; break;			  	 //Z1004
				case  5: uuid = mZ0000_GameObjectives.cp_RequestToMayorUUID; break;			 	 //Z1006
				case  6: uuid = mZ0000_GameObjectives.postponementRequestToMayerUUID; break; 	 //Z1007
				case  7: uuid = mZ0000_GameObjectives.informHotspot_001UUID; break;			 	 //Z1008
				case  8: uuid = mZ0000_GameObjectives.raiseInterestHotSpot_001UUID; break;	 	 //Z1009
				case  9: uuid = mZ0000_GameObjectives.collectKnowledgeHotSpot_001UUID; break;	 //Z1010
				case 10: uuid = mZ0000_GameObjectives.activateCitizenHotSpot_001UUID; break; 	 //Z1011
				case 11: uuid = mZ0000_GameObjectives.informHotspot_002UUID; break;			 	 //Z1012
				case 12: uuid = mZ0000_GameObjectives.raiseInterestHotSpot_002UUID; break;	 	 //Z1013
				case 13: uuid = mZ0000_GameObjectives.collectKnowledgeHotSpot_002UUID; break; 	 //Z1014
				case 14: uuid = mZ0000_GameObjectives.activateCitizenHotSpot_002UUID; break; 	 //Z1015
				case 15: uuid = mZ0000_GameObjectives.informHotspot_003UUID; break;			 	 //Z1016
				case 16: uuid = mZ0000_GameObjectives.raiseInterestHotSpot_003UUID; break;		 //Z1017
				case 17: uuid = mZ0000_GameObjectives.collectKnowledgeHotSpot_003UUID; break;	 //Z1018
				case 18: uuid = mZ0000_GameObjectives.activateCitizenHotSpot_003UUID; break;	 //Z1019
				case 19: uuid = mZ0000_GameObjectives.mayorAllowsCivicParticipationUUID; break;  //Z1020
				case 20: uuid = mZ0000_GameObjectives.engineerActivatedUUID; break;			 	 //Z1021
				case 21: uuid = mZ0000_GameObjectives.constructionDataReceivedUUID; break;		 //Z1022
				case 22: uuid = mZ0000_GameObjectives.solutionSimulatedUUID; break;				 //Z1023
				case 23: uuid = mZ0000_GameObjectives.budgetReceivedUUID; break;				 //Z1024
				case 24: uuid = mZ0000_GameObjectives.ownPRCampaignStartedUUID; break;			 //Z1025
				case 25: uuid = mZ0000_GameObjectives.ConsentReachedUUID; break;				 //Z1026
				case 26: uuid = mZ0000_GameObjectives.ParticipationCorrectlyExecutedUUID; break; //Z1027
				case 27: uuid = mZ0000_GameObjectives.SolutionConsideredUUID; break;			 //Z1028
				}
				
				Epigene.GAME.GameManager.Instance.Event("ACHIEVEMENT", uuid, "completed");
			}
		}

		protected void toEndScreen(string screen) {
			Epigene.UI.UIManager.Instance.ActivateScreen(screen);
		}

		void imageButton()
		{

		}

		void OnGUI() {

			base.OnGUI();

			if (showDebug == true) {
				// Make a background box
				GUI.Box (new Rect (10, 210, 320, 70), "");

				//Debug Buttons
				// Make the GetAchievements button.
				if (GUI.Button (new Rect (20, 220, 70, 20), "Achieve")) {
					achieve();
				}

				// Make the GoToEndScreen button.
				if (GUI.Button (new Rect (95, 220, 70, 20), title)) {
					if (title == "Win")
					{
						toEndScreen("GameSuccess");
						title = "Lose";
					}
					else if (title == "Lose")
					{
						toEndScreen("GameOver");
						title = "Win";
					}
				}

				// Make the Check For Saved Game button.
				if (GUI.Button (new Rect (166, 220, 78, 20), "CheckSave")) {
					check();
				}

				highscore = GUI.TextField (new Rect (20, 250, 70, 20), highscore, 10);

				
				// Make the SendHighscore button.
				if (GUI.Button (new Rect (95, 250, 70, 20), "SendScore")) {
					sendHighscore(highscore);
				}

				// Make the Load Saved Game button.
				if (GUI.Button (new Rect (170, 250, 70, 20), "Load")) {
					LoadController.Instance.ResetSave = false;
					LoadController.Instance.AutoReplayPlayerActions = false;
					load();
				}
				
				// Make the Check For Saved Game button.
				if (GUI.Button (new Rect (245, 220, 78, 20), "ResetSave")) {
					LoadController.Instance.ResetSave = true;
					GameManager.Instance.Event(
						"GAME_LOAD_COMPLETE", 
						"load", 
						"");
				}
				
				// Make the Replay Saved Game button.
				if (GUI.Button (new Rect (245, 250, 70, 20), "Replay")) {
					LoadController.Instance.ResetSave = false;
					LoadController.Instance.AutoReplayPlayerActions = true;
					//test
					/*GameManager.Instance.Event(
						"GAME_LOAD_COMPLETE", 
						"load0", 
						"Epigene.MODEL.E0006_SmallFilterPlant:5");*/
					load();
				}
			}
		}
	}
}
