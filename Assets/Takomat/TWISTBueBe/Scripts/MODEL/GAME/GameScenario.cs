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
using System;
using Epigene.MODEL;
using Epigene.IO;
using Epigene;
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Epigene.GAME;

namespace TWISTBueBe
{
	/// <summary>
	/// This class handles the game scenarios saved 
	/// in a non volatile storage, e.g. xml, mySQL database ...
	/// It is the interface to the different storages
	/// put the data into the MODELs
	/// </summary>
	public sealed class GameScenario
	{
		// Game Mechanics
/*
		public P0000_GameParams gameParams;
		// public IND1000_GameIndicators   gameIndicators;
		public Z0000_GameObjectives  gameObjectives;
*/
/*
		public GameResourcesVO gameResources ;
		public var gameWeather:A0000_WeatherVO;
		public var gamePopulation:H0000_Population;
		public var powerNetwork:O0000_PowerNetwork;
		public var researchProjects:AllResearchProjects;
		public var inhabitants:Vector.<INonPlayerCharacterVO>;
		public var researchers:Vector.<INonPlayerCharacterVO>;
		public var campaigns:Vector.<J0000_CampaignVO>;
*/		
		// public F0000_Finance finances;
/*
		public var achievmentParams:AchievmentParams;
		public var dialogTriggerParams:DialogTriggerParamsVO;
		public var activeEvents:Vector.<GameEventVO>;
		public var gameEventParams:GameEventParamsVO;
		public var buildMenu:BuildMenuVO;
*/

		/*
		// Game Objects
		private var _simulationConfig:SimulationConstants;
		public var gameObjectConfig:PowerPlantConfig;
		public var gameObjects:Vector.<IGameObjectVO>;
		public var powerPlants:Vector.<IPowerPlantVO>;
		public var storages:Vector.<IStorageVO>;
		public var wasteSites:Vector.<IWasteSiteVO>;
		public var _atomicWasteSite_MengeProMonat:Number = 0;
		*/


		// time vars
/*
		private var _elapsedTime:Number = 0;
		public var currentYear:Number = 0;
		public var currentMonth:Number = 0;
		public var currentDay:Number = 0;
		public var displayYear:Number = 0;
		public var displayGameOverYear:Number = 0;
		public var displayMonth:Number = 0;
		public var displayDay:Number = 0;
		public var preSimulationMonths:Number = 0;
		public var isSimulationRunning:Boolean = false;
		*/

		GameConfiguration 		gameConfiguration 	= GameConfiguration.Instance;

		H0000_Population		population			= H0000_Population.Instance;
		
		// we use double here, because in epigene 
		// Flash version(ActionSctipt 3)  we use Number 
		// which is a data type representing 
		// an IEEE-754 double-precision floating-point number 
		// that might change to int, we will see.
		public int 		currentYear				= 2014;   	// 2014 A 
		public int 		currentMonth			= 5;  		// start 5		
		public int 		currentDay		 		= 1;    	// start 1
		public DateTime currentDate 			= new DateTime (1970, 1, 1);

		public double 	feces					= 150.0;	//g/d per Citizen
		public double 	fecesDensity			= 0.75;		//kg/l
		public double 	urine					= 1.5;	 	//l/d per Citizen
		public double 	flushingWater			= 21;		//l/d per Citizen
		public double 	greywaterSpecific		= 50.46;	//l/d per Citizen
		public double 	drinkingWaterSpecific	= 84.51;	//l/d per Citizen

		public double 	sewageCharge			= 2.0;		//EUR/m3
		public double 	waterTariff		= 3.0;		//EUR/m3
		public double 	currentWaterRate 		= 5.0;		//EUR/m3 (sewage + drinking

		public double 	minutesPerYear			= 1.0;		//minutes/year SIM
		public double 	minutesPerWeek			= 2.5;		//minutes/week CP
		public double 	P0006_balance			= 101.0;	//million Euro
		public double 	simulationSpeed			= 1.0;

		public int	  	numberOfPeople			= 500;		//people
		public int	  	criticalMassThreshold 	= 1500;		//people
		
		public double 	cpDurationInDays		= 42;		//days

		public int 		simStartYear			= 0;   	// 2010 A 
		public int 		simStartMonth			= 1;  		// start 1		
		public int 		simStartDay		 		= 1;    	// start 1
		public DateTime simStartDate 			= new DateTime (1970, 1, 1);
		public double 	simDurationInYears		= 16;		//years

		public int		numberOfHouses			= 80;		//houses

//------------------------------------------------------------------------------
		//Action
		private static readonly GameScenario instance = new GameScenario();
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static GameScenario Instance
		{
			get{ return instance;}
		}

		// Use this for initialization
		//void Start () 
		private GameScenario()
		{
			currentDate 	 = DateTime.Now;
			currentYear  	 = currentDate.Year;
			currentMonth	 = currentDate.Month;
			currentDay	 	 = currentDate.Day;
		}//GameScenario()

//------------------------------------------------------------------------------
		// Use this for initialization
		//void Start () 
		public void DownloadStartScenario ()
		{
			WebPlayerDebugManager.addOutput("Load Start Scenario...",1);
			#if UNITY_EDITOR
			// For requested JSON with Editor
			DBModuleManager.Instance.Event
				("START_SCENARIO_LOAD",
                Application.dataPath,
                "/Resources/ExternalFiles");
			#elif UNITY_STANDALONE
			// For requested JSON with Standalone
			if (GameManager.Instance.standaloneBetaMode)
			{
				DBModuleManager.Instance.Event
					("START_SCENARIO_LOAD",
					Application.dataPath,
					"");
			}
			else
			{
				TextAsset download = Resources.Load("ExternalFiles/assets/startScenario") as TextAsset;
				
				LoadStartScenario (download.text);
			}
			#elif UNITY_WEBPLAYER
			// For requested JSON with web player
			WebPlayerDebugManager.addOutput ("Application Data Path: " + Application.dataPath, 1);
			
			DBModuleManager.Instance.Event
				("START_SCENARIO_LOAD",
				null,
				"/epigene_unity3d");
			#endif
		}


		public void LoadStartScenario (string startScenarioText)
		{
			Dictionary<string,object> startScenario = 
				MiniJSON.Json.Deserialize(startScenarioText) as Dictionary<string,object>;

			foreach(Dictionary<string,object> dict
			        in ((List<object>)startScenario["startScenario"]))
			{
				foreach(Dictionary<string,object> obj
				        in ((List<object>)dict["objects"]))
				{
					if(obj.ContainsKey("id"))
					{
						switch(obj["id"].ToString())
						{
						case "StartDate":
							if (obj.ContainsKey("startYear"))
								simStartYear = int.Parse(obj["startYear"].ToString());
							else
								simStartYear = currentYear;
							
							if (obj.ContainsKey("runTime"))
								simDurationInYears = int.Parse(obj["runTime"].ToString());
							break;
						case "FilterPlantLeuchtenthal":
						{
								//"position": "-0.35,3.31",
							string name = "FilterPlantLeuchtenthal";
							string type = "";
							if (obj.ContainsKey("type"))
								type = obj["type"].ToString();
							double simulationMonths=0;
							if (obj.ContainsKey("simulationMonths"))
								simulationMonths = double.Parse(obj["simulationMonths"].ToString());

							int subMonth = -1 * ((int)simulationMonths);


							break;
						}
						case "PipeSubpressure":
						{				
							//"position": "-0.35,3.31",
							string name = "PipeSubpressure";
							string type = "";
							if (obj.ContainsKey("type"))
								type = obj["type"].ToString();
							double simulationMonths=0;
							if (obj.ContainsKey("simulationMonths"))
								simulationMonths = double.Parse(obj["simulationMonths"].ToString());

							int subMonth = -1 * ((int)simulationMonths);

							break;
						}
						}
					}
				}
			}
			WebPlayerDebugManager.addOutput("StartScenario.txt loaded.", 0);
			GameManager.Instance.Event(
				"SIMULATION2",
				"SIM",
				"RestartFinished");
		}


		public void Load()
		{
			Log.Info("<color=cyan>GameScenario Load</color> initialized.");

			// start to load config scenario data 

			//TODO Check wether or not there's saved data and if so, use the savedData DateTime
			currentDate = DateTime.Now;
			currentYear  	 = currentDate.Year;
			currentMonth	 = currentDate.Month;
			currentDay	 	 = currentDate.Day;

			/*
			else
			{
				currentYear  	 = int.Parse (gameConfiguration.GetValue("currentYear"));
				currentMonth 	 = int.Parse (gameConfiguration.GetValue("currentMonth"));
				currentDay 	 	 = int.Parse (gameConfiguration.GetValue("currentDay"));
				currentDate	 	 = new DateTime(currentYear, currentMonth, currentDay);
			}
			*/

			if (gameConfiguration.HasKey ("feces"))
				feces						= double.Parse (gameConfiguration.GetValue("feces"));
			if (gameConfiguration.HasKey ("fecesDensity"))
				fecesDensity				= double.Parse (gameConfiguration.GetValue("fecesDensity"));
			if (gameConfiguration.HasKey ("urine"))
				urine						= double.Parse (gameConfiguration.GetValue("urine"));
			if (gameConfiguration.HasKey ("flushingWater"))
				flushingWater				= double.Parse (gameConfiguration.GetValue("flushingWater"));
			if (gameConfiguration.HasKey ("greywaterSpecific"))
				greywaterSpecific			= double.Parse (gameConfiguration.GetValue("greywaterSpecific"));
			if (gameConfiguration.HasKey ("drinkingWaterSpecific"))
				drinkingWaterSpecific		= double.Parse (gameConfiguration.GetValue("drinkingWaterSpecific"));

			if (gameConfiguration.HasKey ("sewageCharge"))
				sewageCharge				= double.Parse (gameConfiguration.GetValue("sewageCharge"));
			if (gameConfiguration.HasKey ("waterTariff"))
				waterTariff			= double.Parse (gameConfiguration.GetValue("waterTariff"));
			currentWaterRate				= sewageCharge + waterTariff;

			if (gameConfiguration.HasKey ("minutesPerYear"))
				minutesPerYear				= double.Parse (gameConfiguration.GetValue("minutesPerYear"));
			if (gameConfiguration.HasKey ("minutesPerWeek"))
				minutesPerWeek				= double.Parse (gameConfiguration.GetValue("minutesPerWeek"));
			if (gameConfiguration.HasKey ("P0006_balance"))
				P0006_balance				= double.Parse (gameConfiguration.GetValue("P0006_balance"));
			if (gameConfiguration.HasKey ("simulationSpeed"))
				simulationSpeed				= double.Parse (gameConfiguration.GetValue("simulationSpeed"));

			if (gameConfiguration.HasKey ("numberOfPeople"))
				numberOfPeople				= int.Parse    (gameConfiguration.GetValue("numberOfPeople"));
			if (gameConfiguration.HasKey ("criticalMassThreshold"))
			{
				criticalMassThreshold		= int.Parse    (gameConfiguration.GetValue("criticalMassThreshold"));
				Z0000_GameObjectives.Instance.CriticalMassThreshold = criticalMassThreshold;
			}

			if (gameConfiguration.HasKey ("cpDurationInDays"))
				cpDurationInDays			= double.Parse (gameConfiguration.GetValue("cpDurationInDays"));
			/*if (gameConfiguration.HasKey ("simDurationInYears"))
				simDurationInYears			= double.Parse (gameConfiguration.GetValue("simDurationInYears"));*/

			if (gameConfiguration.HasKey ("numberOfHouses"))
				numberOfHouses				= int.Parse    (gameConfiguration.GetValue("numberOfHouses"));

			//WebPlayerDebugManager.addOutput("People change: " + numberOfPeople, 4);
			population.NumberOfPeople	= numberOfPeople;
			//finance.CurrentWaterRate	= currentWaterRate;
			//finance.SewageCharge		= sewageCharge;
			//finance.WaterTariff = waterTariff;
			// In general we have inhabitans per house on a general parameter
			// or specific to each house area 
			// manually set

			// between complex and easy implementation

			// what values I essentially need

			// Map material is loaded in LoadSHPFiles in Map Class

			/*

			Trinkwasser	Ausgangswert:  2 EUR/m3 
			
				Abwasser (ungeklärt)	Ausgangswert: 3  EUR/m3 
				(Keine Unterscheidung zwischen Direkt und TOK-Einleiter)
					Abwasser (vorgeklärt)	Ausgangswert: 1,62 EUR/m3 
					(Keine Unterscheidung zwischen Direkt und TOK-Einleiter)
					
					Regenwasserabgabe in Abhängigkeit von der Grundstücksfläche 
					
					Ausgangswert: 0 EUR/m2
					Brauchwasserabgabe	Ausgangswert: 0 EUR/m3
					

			*/

			// taken from the doucment Wohlsborn 
			//P0000_GameParams.Instance.P0006_Balance = 100.0;
			// the unit should be also forwarded here

			/*
			Flows within the house: 
				Input:
					•	Grauwasser: 100 l/EW/d
					•	Elektrische Energie: 10 W/EW/d
					Output:
					•	Wärme-Energie: 15 W/EW/d
					•	Brauchwasser: 100 l/EW/d
					•	Schlamm:  50g/EW/d
			*/
			/*
			•	500 Einwohner
				o	Produzieren pro Person und Tag
					•	100 g Fäzes
					•	1,5 l Urin
					•	35 l Spülwasser
					•	50 l Grauwasser
					o	Konsumieren pro Person und Tag
					•	87 l Trinkwasser (aus der zentralen Wasserversorgung) davon:
					27% für Toilettenspülung; 36% für baden/Duschen/Körperpflege/12% 
					Wäschewaschen/15% Auto/Garten/Raumpflege; 5% Geschirrspülen; 5 Essen/Trinken
				•	Hauswasserversorgung nicht gemessen

			*/

			/*
			// time vars
			private var _elapsedTime:Number = 0;
			public var displayYear:Number = 0;
			public var displayGameOverYear:Number = 0;
			public var displayMonth:Number = 0;
			public var displayDay:Number = 0;
			public var preSimulationMonths:Number = 0;
			public var isSimulationRunning:Boolean = false;
			*/





			// Flash engine: 
			// _p0003_MIN_PER_YEAR = 3; three minutes are one year
			// _ONE_YEAR  = (_p0003_MIN_PER_YEAR * 60 * 1000)/2; one year in milliseconds, why divided by 2?

			// a year three minutes -> one month is than 15 seconds 

			// calculations are done in month and in day ticks


			/*
			 * origionall ythsi WaitForSeconds iPhoneUtils into 
				as byte ArrayList in faash .

					this will Behaviour the same here int UnityEditor AndroidJNI also the 
					fiormat will Behaviour the same 

the database interface will be the same as the flash interface is 
already and ih have to talk with our guys from endertech about 
that 

Byte array in falsh should be handled in the same way but i think the content 
could be different 

			*/
			/* now for the Timelines */
			/* TimelineCP */
            /* The CP runs as follows:

               CP1_2_2 : Here the time is shown 15.3.2014, that the village council 
                         has decided to decide about a planned solution on the 
                         1.5.2014. It is set, that player has now the date 15.4..
                         So the first date is 4 weeks ago and the second date 
                         is in two weeks. The two dates will be set automatic
                         to the real date the player has, e.g. 27.7.2014 is current
                         date. So the first date is set to the 29.6.2014 and the 
                         second date is set to 10.8.2014
               CP1_6_3 : Do you want a posponement, because only 2 weeks to go.

               CP1_8_5 : Posponement accepted . So the decision date of the council 
			 			 is now 8.9.2014(calculated 4weeks+1 
			 			 day Submission time(Submission on the 7.9.)
			 			 So the participitation solution from the player is 
                         has been submitted on the 7.9. and is taken into account
               CP1_8_7 : Show TimelineCP and set it to run: The TimelineCP
                         runs now in the "Simulation" situation    
               CP      : The TimelineCP is ticking now from 27.7. to 7.9. until CP3_3_1 the 
                         submission of the solution

			   From CP1_8_7 to CP3_3_1 the time ticks. But only in situation,
			   in which the timeline occurs, 
			   which is only in the Simulation/Map Overview. So time is measured 
			   how long the player needs to select a HotSpot or Clicks 
			   on the Mail button. 
			   Additional time will be added as follows: 
			   1. visiting the HotSpots+TecSpots+CityHallOffice+Engineer: 
				  Each time add a day to the TimelineCP. Around 9 days should 
                  be added in a normal walktrough than. 

            */



		}

		/*

			//======================================================================
			public static function readFromByteArray(bArray:ByteArray):GameScenarioVO
			{
				registerClasses();
				
				bArray.position = 0;
				var scenario:GameScenarioVO = bArray.readObject();
				
				if (scenario.terrainBaseMap){
					scenario.terrainBaseMap.texture = 
						ImageProvider.getClassResourceByName(String(scenario.terrainBaseMap.texture));
					scenario.terrainBaseMap.rendererInst = new MainTerrainRenderer();
				}
				for (var i:int=0; i<scenario.gameObjects.length; i++){
					var go:IGameObjectVO = scenario.gameObjects[i];
					go.texture = ImageProvider.getClassResourceByName(String(go.texture));
					go.rendererInst = new DefaultGameObjectRenderer();
				}
				for (var j:int=0; j<scenario.terrainTiles.length; j++){
					var tile:ITerrainTileVO = scenario.terrainTiles[j];
					scenario.addToTerrainDictionary(tile);
				}
				
				G0200_InterimWasteSite.MENGE_PRO_MONAT = scenario._atomicWasteSite_MengeProMonat;
				trace("read from scenario: G0200_InterimWasteSite.MENGE_PRO_MONAT: "+G0200_InterimWasteSite.MENGE_PRO_MONAT);
				
				scenario.refreshPowerPlants();
				scenario.refreshStorages();
				scenario.refreshWasteSites();
				
				return scenario;
			}

		*/

		// Achievements
		// a list of possible Achievements
		// ends up to get these Achievements

		// score definition currently 
		// Lösungsvorschlag(Solution proposal) : 75% Social , 0% Economy, 25 % Ecology
		// CP(BueBe) part indicator scores on each of three scores equivalent
		// TWIST : Influence matrix calculation
		// 2014.07.04.0553_InfluenceMechanic


	}//GameScenario
}//namespace