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
using Epigene;
using Epigene.GAME;
using Epigene.UI;
using Epigene.AUDIO;

//------------------------------------------------------------------------------
namespace Epigene.MODEL
{
	
	/// <summary>
	/// Class for manage Scores
	/// This class represents game goals including 
	/// scores and acheivements
	/// the system and therefore for the player:
	/// The scores and 
	/// </summary>
	public sealed class Z0000_GameObjectives
	{
		//Scores
		//mZ0101 - mZ0104
		public  int maxScore = 100;

		// simulation part
		// score system for simulation
		// range : 0 to 300 every round
		public Score score;

		// Score Self-Evaluation(Selbstverortung)
		// player sets these values
		public Score scoreSelf;
		// solution set by the game AI : "Lösungsvorschlag"
		// This comes from the simulation solution.
		//
		// In the first version it is fixed defined by Daniel
		// Reset() method
		// 75% Social , 0% Economy, 25% Ecology
		public Score scoreSelfAI; // percentage
		// 
		public Score scoreSelfAIAbsolute; // absolute numbers 
		


		private Achievements achievements;
		private int			 achievementCount; //number of imported Achievements
		
//------------------------------------------------------------------------------
		
		// later we export this to a table, whic includes UUID and the semantic 
		// text for the achievemtn
		public string emailCPAcceptedUUID           	 = "550e8400-e29b-41d4-a716-446655440000";
		public string emailCPBudgetAcceptedUUID     	 = "c19186be-f58c-473b-94b0-72ddf6c368ac";
		public string emailConstructionDataUUID			 = "80d81e97-fd6c-4e11-b752-a3189092d964";
		public string emailCPNotAcceptedUUID			 = "511589ae-0edd-425e-8bd1-84862d28db6e";
		public string emailCPSolutionReceivedUUID		 = "a8c4f029-3064-4526-b652-4a1560b09b98";
		// List of all achievements
		//TODO Daniel: Are those complete?
		public string activeCitizenUUID             	 = "ef5b1ffa-52fb-4e8e-a5e9-3354838452c5";
		public string initParticipationUUID         	 = "e9b610ea-d377-4140-9d8b-92884c68601b";
		public string requestPlanUUID               	 = "d7374f33-0775-45b0-9a1f-7ad797d0d11e";
		public string initPostponementUUID          	 = "0863e949-c52e-4247-bdb3-2afb49a6e26c";
		public string cp_RequestToMayorUUID				 = "8696c180-57c4-4725-96b7-540bc2b62618";
		public string postponementRequestToMayerUUID	 = "b8be5e8d-7180-4bfe-93c2-977940f564a8";
		public string informHotspot_001UUID				 = "4e3f92d6-eed2-410f-a965-41925759ed74";
		public string raiseInterestHotSpot_001UUID		 = "98327894-9b03-42e9-810c-4348876df5a7";
		public string collectKnowledgeHotSpot_001UUID	 = "d659c0dc-171d-4e6f-994c-ac2e0b1f7501";
		public string activateCitizenHotSpot_001UUID	 = "e972b769-b109-4e9c-b508-984813ed713f";
		public string informHotspot_002UUID				 = "c0f1984d-ebbd-4c48-a58d-abe40643e686";
		public string raiseInterestHotSpot_002UUID		 = "ed4747de-4820-45c8-a270-a0cfa6808bb8";
		public string collectKnowledgeHotSpot_002UUID	 = "56a8771d-54bf-47ab-8429-0b049187b978";
		public string activateCitizenHotSpot_002UUID	 = "06ce6be1-24f5-42a3-be54-76a0cd838910";
		public string informHotspot_003UUID				 = "e1239dfb-388c-458f-9c6c-8b544fc826b2";
		public string raiseInterestHotSpot_003UUID		 = "6486b014-babb-4882-b98a-c34b5c5f0c77";
		public string collectKnowledgeHotSpot_003UUID	 = "e8ab004a-7a7a-47ab-a08a-9a0de6ff454d";
		public string activateCitizenHotSpot_003UUID	 = "eb6ef3b1-9683-4349-aa74-568f2b817f8f";
		public string mayorAllowsCivicParticipationUUID	 = "a09ba568-d9df-462b-a406-2b893d8c6242";
		public string engineerActivatedUUID				 = "77993904-81ee-4e60-8e12-87993050a4f1";
		public string constructionDataReceivedUUID		 = "9f92c47f-e542-4e29-a29c-56accc69f983";
		public string solutionSimulatedUUID				 = "713c4807-e3c8-4e13-b8eb-ff9394366daa";
		public string budgetReceivedUUID				 = "a34412da-2c04-42a1-9e89-290b51177b72";
		public string ownPRCampaignStartedUUID      	 = "2ebc815c-92ea-40d6-ad4c-bfc6400b96a7";
		public string ConsentReachedUUID				 = "83514784-bdfe-40d2-943f-b56476946bde";
		public string ParticipationCorrectlyExecutedUUID = "3a57c223-355b-4ab7-b821-80bf7c69cc52";
		public string SolutionConsideredUUID			 = "baba09fa-ef1a-4303-b049-208c2033f85d";
		public string criticalMassReachedUUID			 = "d075d977-23b7-47ce-a679-755539e10168";

		public string completeTechSpot_001UUID			 = "26b02e36-4fa0-4f0c-98f0-9915646483d1";
		public string completeTechSpot_002UUID			 = "b27a3ba6-745b-4797-a1a7-1e5a569ab510";
		public string completeEngineerUUID				 = "1f0f2c91-9a58-4bbc-af2c-5330f1ce0208";
		
		public string expiryDateRecognizedUUID      	 = "8b0c8af5-27d8-4daa-9158-fb8d013ce57c";
		

		public string[]     namesHotSpotArray;
		public List<string> activateCitizenHotspot;
		
		public string budgetReceived                	 = "ac87d6bf-021e-4655-b194-8b9c9033d5f8";
		public string mayorAllowsCivicParticipation 	 = "b8250f80-cc3a-4462-8c81-c36a2f11ab38";
		
		public string[]     namescollectKnowledgeHotspotArray;
		public List<string> collectKnowledgeHotspot;
		
		public string[]     namesraiseInterestHotspotArray;
		public List<string> raiseInterestHotspot;
		
		public int ActiveCitizen
		{
			get { return activeCitizen;}
			set 
			{ 
				activeCitizen = value;
			}
		}
		public int activeCitizen;
		
		public int InformCitizen
		{
			get { return informCitizen;}
			set 
			{ 
				informCitizen = value;
			}
		}
		public int informCitizen;
		
		public int ActivateCitizen
		{
			get { return activateCitizen;}
			set 
			{ 
				activateCitizen = value;
			}
		}
		private int activateCitizen;

		public int Total
		{
			get { 
				total  = scoreSelfAIAbsolute.Social;
				total += scoreSelfAIAbsolute.Ecology;
				total += scoreSelfAIAbsolute.Economy;
				total += activeCitizen+informCitizen+activateCitizen;
				return total;}
		}
		// this is the total score for the high score 
		// It combines CP and SIM scores
		private int total;


		public int CriticalMassThreshold
		{
			get { return criticalMassThreshold;}
			set 
			{ 
				criticalMassThreshold = value;
			}
		}
		private int criticalMassThreshold = 0;
		
		
		//------------------------------------------------------------------------------
		// score system for simulation
		// range : 0 to 300 every round
		// public  int mZ0101_scoreSocial  = 0;
		// public  int mZ0102_scoreEconomy = 0;
		// public  int mZ0103_scoreEcology = 0;
		// total scores
		// public  int score.Total   = 0;
		
		// highscoreObj.socialPoints = objectives.z0101_socialPoints.spielSumme;
		// highscoreObj.oekologiePoints = objectives.z0103_ecologyPoints.spielSumme;
		// highscoreObj.oekonomiePoints = objectives.z0102_economyPoints.spielSumme;
		// highscoreObj.totalPoints = objectives.z0104_totalScore.currentValue;
		
		
		// player sets these values
		// public int mZ0105_scoreSelfPositioningSocial   = 0;
		// public int mZ0106_scoreSelfPositioningEconomy  = 0;
		// public int mZ0107_scoreSelfPositioningEcology  = 0;
		// public int mZ0108_scoreSelfPositioningTotal    = 0;
		
		// solution set by the game AI : "Lösungsvorschlag"
		// public int mZ0109_scoreSelfPositioningAISocial   = 0;
		// public int mZ0110_scoreSelfPositioningAIEconomy  = 0;
		// public int mZ0111_scoreSelfPositioningAIEcology  = 0;
		// public int mZ0112_scoreSelfPositioningAITotal    = 0;
		
		// Design value : Quantitative "D" to Quality 0, 25, 50, 75, 100 procent total
		// mapping mZ0109_scoreSelfPositioningAISocialD to mZ0109_scoreSelfPositioningAISocial is 
		// approximately with dramaturgy ;-)
		
		// High Score and quantitative numbers : Simulation
		// public double mZ0109_scoreSelfPositioningAISocialD   = 0;
		// public double mZ0110_scoreSelfPositioningAIEconomyD  = 0;
		// public double mZ0111_scoreSelfPositioningAIEcologyD  = 0;
		// public double mZ0112_scoreSelfPositioningAITotalD    = 0;
		
		//------------------------------------------------------------------------------
		//self participitation Action
		public bool talk  = false;
		public bool vote  = false;
		public bool party = false;
		
		private static readonly Z0000_GameObjectives instance = 
			new Z0000_GameObjectives();
		
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static Z0000_GameObjectives Instance
		{
			get{ return instance;}
		}
		
		//------------------------------------------------------------------------------
		/// <summmary>
		/// Reset scores
		/// </summary>
		public void ResetScores(Score.ScoreMode type)
		{

			
			if(type!=Score.ScoreMode.AI)
			{
				scoreSelf.Social = 0;
				scoreSelf.Economy = 0;
				scoreSelf.Ecology = 0;				
				
			} else
			{
				scoreSelfAIAbsolute.Social  = 0;
				scoreSelfAIAbsolute.Economy = 0;
				scoreSelfAIAbsolute.Ecology = 0;
				//CalculateScoreSelfAIPercentage();
			}
			
			activeCitizen   = 0;
			informCitizen   = 0;
			activateCitizen = 0;
			total = 0;
			
		}

		/// <summmary>
		/// Reset scores
		/// </summary>
		public void ResetScores()
		{
			score.Social  = 0;
			score.Economy = 0;
			score.Ecology = 0;

			ResetScores(Score.ScoreMode.AI);
			ResetScores(Score.ScoreMode.SELF);
		}
		
		/// <summmary>
		/// Reset Actions
		/// </summary>
		public void ResetActions()
		{
			talk  = false;
			vote  = false;
			party = false;
		}
		
		/// <summmary>
		/// Reset scores
		/// </summary>
		public void Reset()
		{
			ResetScores();
			// Solution finding AI result is currently 
			// defined by Daniel in a fix way
			// Later this will dynamically 
			// set by the civic aware solution finding
			// process 
			SetScore (75, 0, 25, Score.ScoreMode.AI);
			
			ResetActions();
			achievements.Reset ();
		}//ResetAction()

		// Use this for initialization
		//void Start () 
		private Z0000_GameObjectives()
		{
			Log.Debug("Z0000_GameObjectives initialized.");
			score       = new Score();
			scoreSelf   = new Score();
			scoreSelfAI = new Score();
			scoreSelfAIAbsolute = new Score();
			scoreSelfAIAbsolute.rangeMax =  100000;
			scoreSelfAIAbsolute.rangeMin = -100000;

			namesHotSpotArray = new string[] {	
				"aafa106b-0655-4b2d-9557-16c3ab0272f0",
				"4d8bd261-0c94-4613-8aa7-6dcdcf9f8dee",
				"3c5b8ea6-311b-4c31-9443-83e1afef63ca"
			};
			activateCitizenHotspot  
				= new List<string>(namesHotSpotArray);
			
			
			namescollectKnowledgeHotspotArray = new string[] {	
				"50293b47-36bc-47eb-92b8-ce2e79e4c716",
				"801228da-a84d-46d2-bc17-ffd2e5c7b753",
				"dcb53ec7-7cb4-453d-b54c-bb17587fe6b3"
			};
			collectKnowledgeHotspot 
				= new List<string>(namescollectKnowledgeHotspotArray);
			
			
			namesraiseInterestHotspotArray = new string[] {	
				"a6f0c811-4c27-449d-ae8a-75334a220bc6",
				"3045bb58-67b8-4360-8f66-1a497cb4c0ff",
				"665c0462-6686-401a-888f-c5f14919041f"
			};
			raiseInterestHotspot 
				= new List<string>(namesraiseInterestHotspotArray);

			GameManager.Instance.RegisterEventHandler("ACHIEVEMENT", AchievementEventHandler);
			GameManager.Instance.RegisterEventHandler("RESET", ResetEventHandler);

			achievements = Achievements.Instance;
			if (achievements.Items.Count < 1) GetAchievements();

			// at start init all the values equal to a Game restart/reset
			Reset();

		}
		
		//------------------------------------------------------------------------------
		
		/// <summmary>
		/// Update scores
		/// </summary>
		public void UpdateScore(
			int social, int economy, int ecology, Score.ScoreMode type)
		{
			if(type!=Score.ScoreMode.AI)
			{
				// the system is a mapping 
				// 
				if(scoreSelf.Total < maxScore)
				{
					scoreSelf.Social += social;
					scoreSelf.Economy += economy;
					scoreSelf.Ecology += ecology;
				}
				// Debug.Log ("---------------- TOTO"+scoreSelf.Social;
			} else
			{
				if(scoreSelfAI.Total < maxScore)
				{
					scoreSelfAIAbsolute.Social  += social;
					scoreSelfAIAbsolute.Economy += economy;
					scoreSelfAIAbsolute.Ecology += ecology;	
					CalculateScoreSelfAIPercentage();
				}
				Log.GameTimes ("SCORE "+  scoreSelfAI.Social + " " + scoreSelfAI.Economy + " " +
				               scoreSelfAI.Ecology);
			}
			/*
			Debug.Log ("Total : "+ score.Social 
			           + " " + score.Economy 
			           + " " + mZ0103_scoreEcology
			           + " " + score.Total);
			 */          
			
		}//UpdateScore()
		
		/// <summmary>
		/// Update scores in Percentage way
		/// </summary>
		public void SetScore(int social, int economy, int ecology, Score.ScoreMode type)
		{
			if(type!=Score.ScoreMode.AI)
			{
				// the system is a mapping 
				// 
				scoreSelf.Social = social;
				scoreSelf.Economy = economy;
				scoreSelf.Ecology = ecology;
			
				// Debug.Log ("---------------- TOTO"+scoreSelf.Social;
			} else
			{
				scoreSelfAIAbsolute.Social  = social*3;
				scoreSelfAIAbsolute.Economy = economy*3;
				scoreSelfAIAbsolute.Ecology = ecology*3;	
				// Percentage, e.g. 25,50,25

				scoreSelfAI.Social  = social;
				scoreSelfAI.Economy = economy;
				scoreSelfAI.Ecology = ecology;	

				//CalculateScoreSelfAIPercentage();
			}
			/*
			Debug.Log ("Total : "+ score.Social 
			           + " " + score.Economy 
			           + " " + mZ0103_scoreEcology
			           + " " + score.Total);
			 */          
			
		}//UpdateScore()
		
		// we need this methods for mapping from absolute scores to relative one
		
		public void SubScoreAISocial(int score)
		{
			scoreSelfAIAbsolute.Social -= score;
		}
		
		public void AddScoreAISocial(int score)
		{
			scoreSelfAIAbsolute.Social += score;
		}

		public void ShowScores()
		{
			string pointsAccount = "Show  Points account: ";
			pointsAccount += "Social("+scoreSelfAI.Social+") ";
			pointsAccount += "Economy("+scoreSelfAI.Economy+") ";
			pointsAccount += "Ecology("+scoreSelfAI.Ecology+")";
			Log.Score (pointsAccount);
		}
		
		// ----------------- Indicator Interface ------
		private void ShowPointsAccount()
		{
			string pointsAccount = "Show  Points account: ";
			pointsAccount += "activeCitizen("+activeCitizen+") ";
			pointsAccount += "informCitizen("+informCitizen+") ";
			pointsAccount += "activateCitizen("+activateCitizen+")";
			Log.Score (pointsAccount);
		}

		public void SubActiveCitizen(int score)
		{
			//WebPlayerDebugManager.addOutput (activeCitizen + "-=" + score, 1);
			activeCitizen -= score;
			ShowPointsAccount();
		}
		
		public void AddActiveCitizen(int score)
		{
			//WebPlayerDebugManager.addOutput (activeCitizen + "+=" + score, 1);

			activeCitizen += score;
			Log.GameTimes ("AddActiveCitizen");
			ShowPointsAccount();
		}
		
		public void SubInformCitizen(int score)
		{

			informCitizen -= score;
			Log.GameTimes ("SubInformCitizen");
			ShowPointsAccount();
		}
		
		public void AddInformCitizen(int score)
		{

			informCitizen += score;
			Log.GameTimes ("AddInformCitizen");
			ShowPointsAccount();
		}
		
		
		
		public void SubActivateCitizen(int score)
		{

			activateCitizen -= score;
			Log.GameTimes ("SubActivateCitizen");
			ShowPointsAccount();
		}
		
		public void AddActivateCitizen(int score)
		{
			activateCitizen += score;
			Log.GameTimes ("AddActivateCitizen");
			ShowPointsAccount();
		}
		
		/// <summary>
		///  not needed currently
		/// </summary>
		public void CalculateScoreSelfAIPercentage()
		{
			double percentageSocial  = 0.0f;
			double percentageEconomy = 0.0f;
			double percentageEcology = 0.0f;
			
			scoreSelfAI.Social = 0;
			scoreSelfAI.Economy = 0;
			scoreSelfAI.Ecology = 0;



			// does this work
			int total = scoreSelfAIAbsolute.Social + scoreSelfAIAbsolute.Economy + scoreSelfAIAbsolute.Ecology;
			percentageSocial = total;
			percentageSocial = scoreSelfAIAbsolute.Social/percentageSocial;  // 0.41
			
			percentageEconomy = total;
			percentageEconomy =	scoreSelfAIAbsolute.Economy/percentageEconomy; // 0.17
			
			percentageEcology = total;
			percentageEcology =	scoreSelfAIAbsolute.Ecology/percentageEcology; // 0.41

			//WebPlayerDebugManager.addOutput("Calculated Percentage: Social(" + percentageSocial + "), Economy("+ percentageEconomy + "), Ecology ("+percentageEcology+")", 1);


			//Top Down: First Absolut Scores (100% SOCIAL/ECONOMY/ECOLOGY)
			if (percentageSocial > 0.75f)
			{
				scoreSelfAI.Social = 100;
			} 
			else if (percentageEconomy > 0.75f)
			{
				scoreSelfAI.Economy = 100;
			} 
			else if (percentageEcology > 0.75f)
			{
				scoreSelfAI.Ecology = 100;
			}

			//Then one bigger than the other two combined
			else if(percentageSocial > 0.50f)
			{
				scoreSelfAI.Social = 75;
				if (percentageEcology >= percentageEconomy)
					scoreSelfAI.Ecology = 25;
				else
					scoreSelfAI.Economy = 25;	
			}
			else if(percentageEconomy > 0.50f)
			{
				scoreSelfAI.Economy = 75;
				if (percentageEcology >= percentageSocial)
					scoreSelfAI.Ecology = 25;
				else
					scoreSelfAI.Social = 25;	
			} 
			else if(percentageEcology > 0.50f)
			{
				scoreSelfAI.Ecology = 75;
				if (percentageSocial >= percentageEconomy)
					scoreSelfAI.Social = 25;
				else
					scoreSelfAI.Economy = 25;	
			}

			//One is bigger than the others
			else if ((percentageSocial >= percentageEconomy) &&
			         (percentageSocial >= percentageEcology))
			{
				scoreSelfAI.Social = 50;

				if (percentageEcology < 0.25f) 
					scoreSelfAI.Economy = 50;
				else if (percentageEconomy < 0.25f)
					scoreSelfAI.Ecology = 50;
				else 
				{
					scoreSelfAI.Economy = 25;
					scoreSelfAI.Ecology = 25;
				}
			} 
			else if ((percentageEconomy >= percentageSocial) &&
			         (percentageEconomy >= percentageEcology))
			{
				scoreSelfAI.Economy = 50;
				
				if (percentageEcology < 0.25f) 
					scoreSelfAI.Social = 50;
				else if (percentageSocial < 0.25f)
					scoreSelfAI.Ecology = 50;
				else 
				{
					scoreSelfAI.Social = 25;
					scoreSelfAI.Ecology = 25;
				}
			} 
			else if ((percentageEcology >= percentageSocial) &&
			         (percentageEcology >= percentageEconomy))
			{
				scoreSelfAI.Ecology = 50;
				
				if (percentageEconomy < 0.25f) 
					scoreSelfAI.Social = 50;
				else if (percentageSocial < 0.25f)
					scoreSelfAI.Economy = 50;
				else 
				{
					scoreSelfAI.Social = 25;
					scoreSelfAI.Economy = 25;
				}
			}

			//Last case: All three are equal (as if this would ever happen)
			else
			{
				scoreSelfAI.Social  = 25;
				scoreSelfAI.Economy = 25;
				scoreSelfAI.Ecology = 25;
			}

			/*
			if (percentageSocial > 0.75f)
			{
				scoreSelfAI.Social = 100;
			} 
			else if(percentageSocial > 0.50f)
			{
				scoreSelfAI.Social = 75;
				if (percentageEcology >= percentageEconomy)
					scoreSelfAI.Ecology = 25;
				else
					scoreSelfAI.Economy = 25;	
			} 
			else if(percentageSocial > 0.25f)
			{
				scoreSelfAI.Social = 50;
				if       (percentageEcology > 0.25f)
				{
					scoreSelfAI.Ecology = 50;
				} else if       (percentageEcology > 0.0f)
				{
					scoreSelfAI.Ecology = 25;
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 25;
				}
			} else if(percentageSocial >= 0.25f)
			{   scoreSelfAI.Social = 25;
				if       (percentageEcology > 0.50f)
				{
					scoreSelfAI.Ecology = 75;
				} else if(percentageEcology > 0.25f)
				{
					scoreSelfAI.Ecology = 50;
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 25;
				} else if(percentageEcology >= 0.25f)
				{
					scoreSelfAI.Ecology = 25;
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 50;
				} else
				{
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 75;
				}
				
			} else
			{   // scoreSelfAI.Social == 0
				if       (percentageEcology > 0.75f)
				{
					scoreSelfAI.Ecology = 100;
				} else if(percentageEcology > 0.50f)
				{
					scoreSelfAI.Ecology = 75;
					if(percentageEconomy > 0.01f)  scoreSelfAI.Economy = 25;
				} else if(percentageEcology > 0.25f)
				{
					scoreSelfAI.Ecology = 50;
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 50;
				} else 
				{
					if(percentageEconomy > 0.01f) scoreSelfAI.Economy = 100;
				} 
			}*/
			
			Log.GameTimes ("SCORE Calc1 " + percentageSocial+ 
			               " "      + percentageEconomy +
			               " "      + percentageEcology);
			
			Log.GameTimes ("SCORE Calc2 " + scoreSelfAI.Social+ 
			               " "      + scoreSelfAI.Economy +
			               " "      + scoreSelfAI.Ecology);
		}

		public void GetAchievements()
		{
			getAchievement (activeCitizenUUID);					 //Z1001
			getAchievement (initParticipationUUID);				 //Z1002
			getAchievement (requestPlanUUID);					 //Z1003
			getAchievement (initPostponementUUID);				 //Z1004
			getAchievement (cp_RequestToMayorUUID);				 //Z1006
			getAchievement (postponementRequestToMayerUUID);	 //Z1007
			getAchievement (informHotspot_001UUID);				 //Z1008
			getAchievement (raiseInterestHotSpot_001UUID);		 //Z1009
			getAchievement (collectKnowledgeHotSpot_001UUID);	 //Z1010
			getAchievement (activateCitizenHotSpot_001UUID);	 //Z1011
			getAchievement (informHotspot_002UUID);				 //Z1012
			getAchievement (raiseInterestHotSpot_002UUID);		 //Z1013
			getAchievement (collectKnowledgeHotSpot_002UUID);	 //Z1014
			getAchievement (activateCitizenHotSpot_002UUID);	 //Z1015
			getAchievement (informHotspot_003UUID);				 //Z1016
			getAchievement (raiseInterestHotSpot_003UUID);		 //Z1017
			getAchievement (collectKnowledgeHotSpot_003UUID);	 //Z1018
			getAchievement (activateCitizenHotSpot_003UUID);	 //Z1019
			getAchievement (mayorAllowsCivicParticipationUUID);	 //Z1020
			getAchievement (engineerActivatedUUID);				 //Z1021
			getAchievement (constructionDataReceivedUUID);		 //Z1022
			getAchievement (solutionSimulatedUUID);				 //Z1023
			getAchievement (budgetReceivedUUID);				 //Z1024
			getAchievement (ownPRCampaignStartedUUID);			 //Z1025
			getAchievement (criticalMassReachedUUID);			 // inbetween
			getAchievement (ConsentReachedUUID);				 //Z1026
			getAchievement (ParticipationCorrectlyExecutedUUID); //Z1027
			getAchievement (SolutionConsideredUUID);			 //Z1028
			getAchievement (completeTechSpot_001UUID);			 //no actual achievement; TechSpots
			getAchievement (completeTechSpot_002UUID);			 //no actual achievement; TechSpots
			getAchievement (completeEngineerUUID);				 //no actual achievement; TechSpots
		}

		public void getAchievement(string uuid)
		{
			I18nManager i18n = I18nManager.Instance;
			Achievement ach;

			string id  = i18n.Get (uuid, "id");
			ach 	   = achievements.Add(id);
			string npc = i18n.Get (uuid, "npc");
			switch (npc)
			{
				case "MIRIAM": 	 ach.npc = Achievement.Character.LADY; 		break;
				case "DAD": 	 ach.npc = Achievement.Character.DAD; 	 	break;
				case "BOY": 	 ach.npc = Achievement.Character.BOY; 	 	break;
				case "GIRL": 	 ach.npc = Achievement.Character.GIRL; 		break;
				case "GRANDPA":  ach.npc = Achievement.Character.GRANDPA;	break;
				case "LOGO": 	 ach.npc = Achievement.Character.LOGO; 		break;
				case "ENGINEER": ach.npc = Achievement.Character.ENGINEER;	break;
				case "":		 ach.npc = Achievement.Character.LOGO;		break; // only as dummy for TechSpots
			}
			string text 		= i18n.Get (uuid, "text");
			ach.text 			= text;
			string infotext 	= i18n.Get (uuid, "infotext");
			ach.infotext 		= infotext;
			ach.completed 		= false;
			ach.started			= false;
			achievementCount++;


			//WebPlayerDebugManager.addOutput ("Added new Achievement #" + achievementCount, 1); 


		}

		private bool achieved(string uuid)
		{
			Achievement ach;
			ach = achievements.Get (uuid);
			if (ach != null)
			{
				ach.completed = true;
				//WebPlayerDebugManager.addOutput("Achievement " + ach.text + " completed!", 2);
				achievements.logAchievements();
				return true;
			}
			else
				return false;
		}

		private bool started(string uuid)
		{
			Achievement ach;
			ach = achievements.Get (uuid);
			if (ach != null)
			{
				ach.started = true;
				//WebPlayerDebugManager.addOutput("Achievement " + ach.text + " started!", 2);
				achievements.logAchievements();
				return true;
			}
			else
				return false;
		}

		public void ResetEventHandler(string eventId, string data)
		{
			/*// Log.GameTimes("Z0000_GameObejctives " + eventType+ ": EVENT("+eventType+"): " 
							+ eventId+","+data);
			*/			
			if(eventId=="Models")
			{
				Reset();
			}

		}

		public void AchievementEventHandler(string eventId, string data)
		{
			/*// Log.GameTimes("Z0000_GameObejctives " + eventType+ ": EVENT("+eventType+"): " 
							+ eventId+","+data);
			*/
			//GameManager.Instance.Event(GameEvent.RESET, "Game", "");

			bool done = false;
			if (data == "completed")
			{
				done = achieved (eventId);

				AudioManager.Instance.PlaySound("AUDIO.SFX_E", 0.5f);

				if ((eventId == activateCitizenHotSpot_001UUID) ||
				    (eventId == activateCitizenHotSpot_002UUID) ||
				    (eventId == activateCitizenHotSpot_003UUID))
				{
					if (allHotSpotsActivated())
					{
						//WebPlayerDebugManager.addOutput("send", 2);
						GameObject obj = UIManager.Instance.GetHud("Email");
						obj.SetActive(true);
						GameManager.Instance.Event("EMAIL", emailCPAcceptedUUID , "add");
						obj.SetActive(false);

						//TODO LOAD CP1_8_1
					}
				}
				
				if ((eventId == completeTechSpot_001UUID) ||
				    (eventId == completeTechSpot_002UUID) ||
				    (eventId == completeEngineerUUID))
				{
					if (allTechSpotsActivated())
					{
						GameObject obj = UIManager.Instance.GetHud("Email");
						obj.SetActive(true);
						GameManager.Instance.Event("EMAIL", emailCPBudgetAcceptedUUID , "add");
					}
				}
			}
			else if ((data == "started") && (GameManager.Instance.startAchievementsMode == true))
			{
				done = started (eventId);
			if (done == false)
				WebPlayerDebugManager.addOutput("No Achievement " + eventId + " found!", 3);
			}

		}

		private bool allHotSpotsActivated()
		{
			Achievement a1 = Achievements.Instance.Get(activateCitizenHotSpot_001UUID);
			if(a1==null) return false;
			Achievement a2 = Achievements.Instance.Get(activateCitizenHotSpot_002UUID);
			if(a2==null) return false;
			Achievement a3 = Achievements.Instance.Get(activateCitizenHotSpot_003UUID);
			if(a3==null) return false;
			// Activated = TRUE for all?
			if(a1.completed&&a2.completed&&a3.completed) return true;
			return false;
		}
		
		private bool allTechSpotsActivated()
		{
			Achievement a1 = Achievements.Instance.Get(completeTechSpot_001UUID);
			if(a1==null) return false;
			Achievement a2 = Achievements.Instance.Get(completeTechSpot_002UUID);
			if(a2==null) return false;
			Achievement a3 = Achievements.Instance.Get(completeEngineerUUID);
			if(a3==null) return false;
			// Activated = TRUE for all?
			if(a1.completed&&a2.completed&&a3.completed) return true;
			return false;
		}


	}
}//namespace
//------------------------------------------------------------------------------