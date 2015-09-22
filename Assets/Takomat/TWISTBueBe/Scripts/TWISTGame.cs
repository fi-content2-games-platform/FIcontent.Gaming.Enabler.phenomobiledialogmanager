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
using Epigene.GAME;
using Epigene.MODEL;
using Epigene.UI;
using Epigene.VIEW;
using Epigene.AUDIO;


using TWISTBueBe.GAME;


//TODO: add enter, update, exit

//------------------------------------------------------------------------------
using Epigene.IO;

public static class EditorConfiguration
{
	public static string root = "TWISTBueBe";
}

namespace TWISTBueBe
{
	/// <summary>
	/// Start the game. Add each screens 
	/// to the UIManager and set the first one to active.
	/// Each screen have to manage their own logic.
	/// This class is only a start point for the game
	/// and does only the basic initialization.
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
	public class TWISTGame : MainGame
	{

		/// Design of simTime : 
		/// Simulation time is defined as the current Time,
		/// which is taken from the System API. But the 
		/// simTimeSteps are just defined concerning 
		/// the settings of the user and the performance 
		/// we have with different settings.
		/// The simTime can also be seen as a 0 to 100% relative 
		/// time to the overall time of the world.
		/// We map simTime to the current Date, if we need to.
		/// The mapping is now important for the calculation 
		/// of scores and also cost and income.
		/// That has to be according to the real world time.
		/// Interesting point: Mapping. First of all simple 
		/// mapping. than we refine that mapping later on.
		/// We can not show the current time because it is 
		/// a simulation into the future and that is not 
		/// so in the view of the current time we have to show 
		/// the mapped time.

		/*
		public float simTime     = 0.0f; // unit seconds
		public  float simTimeStep = 0.1f; // unit seconds
		public float cpTime     = 0.0f; // unit seconds
		public  float cpTimeStep = 0.1f; // unit seconds

		private TimeLine    goCP; 
		private TimeLine    goSIM;
		*/

		public string buildTime;
		
		//variables for muliple possibilities to play
		public static bool SIMMode
		{
			get { return simMode;}
			set { simMode = value;}
		}
		public static bool simMode = false;
		public static bool TWISTMode
		{
			get { return twistMode;}
			set { twistMode = value;}
		}
		public static bool twistMode = false;

		public static bool SIMOptions
		{
			get { return simOptions;}
			set { simOptions = value;}
		}
		public static bool simOptions = false;

		public bool SIM;
		public bool TWIST;
		public bool ActivateSimOptions;

		private DBModuleManager dbModuleManager;

		/// Tests the script condition.
		/// </summary>
		/// <param name="value">value to evaulate for condition</param>
		/// <returns>same as value</returns>
		public bool TestScriptConditionEvaulateBool(bool value)
		{
			Log.GameTimes ("TestScriptConditionEvaulateBool");
			
			return value;
		}//TestScriptConditionEvaulateBool()


//------------------------------------------------------------------------------

		// Use this for initialization
		public void Awake() 
		{
			WebPlayerDebugManager.addOutput ("Version build: " + buildTime, 1);
			dbModuleManager = DBModuleManager.Instance;
			if(useDBModule)
				StartCoroutine("addServiceURL");

			//Debug.Log ("TWISTGame Awake");

			base.Awake();

			//------- TODO WORKAROUND -------
			GameScenario gs = GameScenario.Instance;
			gs.currentDate = System.DateTime.Now;
			
			//Edit StartPoint
			System.DateTime startDate = GameScenario.Instance.currentDate;
			//WebPlayerDebugManager.addOutput("Get Current Date: " + startDate, 3);
			startDate = startDate.AddDays (-28);
			string text;
			text = i18n.Get("CP1_2_2", "004").Replace(
				"15.3.2014", 
			    startDate.Day + "." + startDate.Month + "." + startDate.Year
				);
			i18n.Set("CP1_2_2", "004", text);
			
			//Edit EndPoint
			startDate = GameScenario.Instance.currentDate;
			startDate = startDate.AddDays (14);
			text = i18n.Get ("CP1_2_2", "004").Replace(
				"1.5.2014",
				startDate.Day + "." + startDate.Month + "." + startDate.Year
				);
			i18n.Set ("CP1_2_2", "004", text);
			text = i18n.Get ("CP1_5_5", "001").Replace(
				"1.5.2014",
				startDate.Day + "." + startDate.Month + "." + startDate.Year
				);
			i18n.Set ("CP1_5_5", "001", text);
			text = i18n.Get ("CP1_6_1", "001").Replace(
				"1.5.2014",
				startDate.Day + "." + startDate.Month + "." + startDate.Year
				);
			i18n.Set ("CP1_6_1", "001", text);

			//SIMMode = false;
			//TWISTMode = true;
			SIMMode = SIM;
			TWISTMode = TWIST;
			SIMOptions = ActivateSimOptions;
			
			//------- /WORKAROUND -------

			Log.Info("Starting TWISTBueBe...");
		

			// We have two sub game games to the main game 
			GMGameCP gameCP = new GMGameCP("CP");
			gm.Add(gameCP);

			GMGameSIM gameSIM = new GMGameSIM("SIM");
			gm.Add(gameSIM);
			/*GameObject objSIM = UIManager.Instance.GetHud("TimeLineSIM");			
			goSIM = objSIM.GetComponent<TimeLine>();
			
			/*gm.Start ("CP");
			gm.Get("CP").Pause();*/
			//gm.Start ("SIM");
			//gm.Get("SIM").Pause();
			//gm.Start ("CP");


			/*
			// get the view object
			GameObject objCP = UIManager.Instance.GetHud("TimeLineCP");			
			goCP = objCP.GetComponent<TimeLine>();
			*/


			//create conditions
			ACondition falseCond = new FalseCondition();
			ACondition trueCond  = new TrueCondition();
			ScriptCondition<bool> scriptCond =  
				new ScriptCondition<bool>(
					TestScriptConditionEvaulateBool, true);


			//Log.Info("Number of screens:"+ui.NumberOfScreens);
			//Log.Assert(ui.NumberOfScreens > 0, 
			//	"No screen found! Did you tag any screen as UIScreen?");

			// Log.Assert(startScreen, 
			//            "Missing start screen: '"
			//            +startScreenName+"' for "
			//            +gameObject.name);
			// //ui.ActivateScreen(startScreenName);
			// //startScreen.SetActive(true);

		}

		private IEnumerator addServiceURL()
		{
			yield return new WaitForSeconds(0.1f);
			dbModuleManager.Event(
				"ADD_SERVICEURL",
				"development",
				"takomat-bb-dev.stage.endertech.net");
			dbModuleManager.Event(
				"ADD_SERVICEURL",
				"staging",
				"takomat-bb.stage.endertech.net");
			dbModuleManager.Event(
				"ADD_SERVICEURL",
				"production",
				"www.buergerbeteiligungsspiel.de");
			dbModuleManager.Event(
				"ADD_SERVICEURL",
				"production_twist",
				"www.twist-water.com");
			yield return new WaitForSeconds(0.5f);

		}

		public override void Enter()
		{
			Log.Info("ENTER "+gameObject.name);
			AudioManager.Instance.MuteMusic = true;

			
			//TODO: add startScreenName
			//ui.AddScreenFromFile(configResources+"/Settings");
			//switch to start screen
			ui.InitUI(gameObject);			
			ui.UIParent.transform.parent = gameObject.transform;
			ui.ActivateScreen(startScreenName);

			GameManager.Instance.RegisterEventHandler("CONFIG", ProcessConfigEvent, gameObject);
		}

		public override void Exit()
		{
			Log.Info("EXIT "+gameObject.name);
			GameManager.Instance.RemoveEventHandler("CONFIG", ProcessConfigEvent);
		}
	
		// public void CreateScreen(string name)
		// {
		// 	string script = configResources+"/"+name+".txt";

		// }


		public void ProcessConfigEvent(string eventId, string data)
		{
			if (eventId == "ScenarioLoad")
			{
				GameScenario gameScenario = GameScenario.Instance;
				gameScenario.Load ();
				gameScenario.DownloadStartScenario();
			}
			else if (eventId == "StartScenarioLoad")
			{
				GameScenario gameScenario = GameScenario.Instance;
				gameScenario.LoadStartScenario(data);
			}
		}//EventHandler()

	}//class StartGame

}//namespace
//------------------------------------------------------------------------------
