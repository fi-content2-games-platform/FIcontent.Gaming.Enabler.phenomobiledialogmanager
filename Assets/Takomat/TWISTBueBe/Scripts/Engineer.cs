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
//using Epigene.AUDIO;
using Epigene.VIEW;

// TODO Julia:	is actually exactly like Announcement.cs
// 				only Announcement has two more Buttons
namespace TWISTBueBe
{
	/// <summary>
	/// CityHall is the class handling all the situations 
	/// in the CityHall.
	/// e.g. : CP1_3_x
	/// Apply for participation.
	/// </summary>
	public class Engineer : UIScreen	 
	{
		
		/// <summary>
		/// Type of the end
		/// </summary>
		public enum Type {SUCCESS, OVER};
		
		/// <summary>
		/// Set the type
		/// </summary>
		public Type type;
		
		/// <summary>
		/// list of story ids
		/// </summary>
		private List<string> storyList;
		
		// variables for scoring part of MultipleChoice
		// see ProcessAnswer
		//private string[]     namesActiveCitizenArray;
		private List<string> listOfActiveCitizen;
		
		//private NPC npcCoordLady;
		private UICharacter engineer;
		private UICharacter player;

		private string rememberStoryID = "";
		
//------------------------------------------------------------------------------
		private void HandleDialogDependantLogic()
		{
			dialogView = DialogManager.Instance.DialogView;
			string storyId = dialogView.StoryId;
			// TODO: move this to update?
			// 1_10_4
			// logic to evaluate scores
			// Check current screen
			// Log.GameTimes(this.Name+" : dialogView.StoryId : "+
			//              storyId);

			if(DialogManager.Instance.DialogView.ActivateLastDialogV) return;
			
			if ((storyId == "CP1_8_7") ||
			    (storyId==null))
			{
				DialogManager.Instance.LoadStory("CP1_9_3", ProcessDialogAnswer);
			}
			else if(storyId == "CP1_9_3" || storyId == "CP1_9_7" || storyId == "CP1_9_8" || storyId == "CP1_8_7")
			{
				rememberStoryID = storyId;
				DialogManager.Instance.LoadStory("CP0_1_2", ProcessDialogAnswer);
			}
			else if(storyId == "CP1_9_9")
			{
				DialogManager.Instance.LoadStory("CP1_9_5", ProcessDialogAnswer);
			}
		}
		
//------------------------------------------------------------------------------
		/// <summary>
		/// Handles default visibility. So the intersection set of  
		/// visibility of all situations handled with this 
		/// this screen
		/// </summary>
		private void HandleVisibilityOnEnter()
		{
			
			GameObject obj = UIManager.Instance.GetPopup("DecisionPopup");
			if(obj)
				obj.SetActive(true);

			if(engineer.Emotion != EmotionType.NEUTRAL)
				engineer.Emotion = EmotionType.NEUTRAL;
			
			player = (UICharacter)GetObject("Player");
			if(player.Emotion != EmotionType.NEUTRAL)
				player.Emotion = EmotionType.NEUTRAL;
		}
		
//------------------------------------------------------------------------------
		/// <summary>
		/// Enter into this screen		
		/// </summary>
		public override void Enter()
		{
			Log.Info("1 ENTER "+Name);
			
			engineer  = (UICharacter)GetObject("Engineer");
			dialogView = DialogManager.Instance.DialogView;
			dialogView.uiCharacter = engineer;

			HandleVisibilityOnEnter();

			HandleDialogDependantLogic();
			
			Log.Debug("NPC ="+engineer);

			//connect event handlers
			GameManager.Instance.RegisterEventHandler("SCREEN", ProcessScreenEvent, gameObject);
			GameManager.Instance.RegisterEventHandler("DIALOG", ProcessDialogEvent, gameObject);
			GameManager.Instance.RegisterEventHandler("POPUP_DECISION", ProcessPopupEvent, gameObject);

		}//Enter()
		
		/// <summary>
		/// Exit from this screen
		/// </summary>
		public override void Exit()
		{
			Log.Info("EXIT "+Name);


			GameManager.Instance.RemoveEventHandler("SCREEN", ProcessScreenEvent);
			GameManager.Instance.RemoveEventHandler("DIALOG", ProcessDialogEvent);
			GameManager.Instance.RemoveEventHandler("POPUP_DECISION", ProcessPopupEvent);
			
		}//Exit()
		
		
		//------------------------------------------------------------------------------
		/// <summary>
		/// Runs only once
		/// </summary>
		void Start()
		{
			Log.Info("Start "+Name);
			
		}//Start()
		
		/// <summary>
		/// process all type of event which related to this screen.
		/// </summary>
		public void ProcessScreenEvent(string eventId, string data)
		{
			
			if(eventId == "NextScreen")
			{
				GameManager.Instance.Event("DIALOG", "Restart", "fire");
				NextScreen();
			}
			else if(eventId == "ScreenAgain")
			{
				dialogView = DialogManager.Instance.DialogView;
				string storyId = dialogView.StoryId;

				if(storyId == "CP1_9_3")
				{
					DialogManager.Instance.LoadStory("CP1_8_7", "D100", ProcessDialogAnswer);
				}
				else if(storyId == "CP1_9_5" || storyId == "CP1_9_6")
				{
					DialogManager.Instance.LoadStory("CP1_9_9", ProcessDialogAnswer);
				}
				else if(storyId == "CP0_1_2")
				{
					DialogManager.Instance.LoadStory(rememberStoryID, "D100", ProcessDialogAnswer);
				}
				BackScreen();
			}
			else if(eventId == "Settings")
			{
				UpdateSettings();
				
			}
		}
		
		//------------------------------------------------------------------------------
		/// <summary>
		/// Logic for process events from dialog system
		/// </summary>
		void ProcessDialogEvent(string eventId, string data)
		{
			// Event processing
			Log.Debug("event: "+eventId+" data:"+data);
			
			//when a dialog finished it sends a hide event.
			//We capture it here and set the next story or switch screen
			if(data == "hide")
			{
				//process stories
				if(eventId == "CP0_1_2.D004")
				{
					Log.GameTimes("_______________ "+rememberStoryID);
					DialogManager.Instance.LoadStory(rememberStoryID, "D100", ProcessDialogAnswer);
					uiManager.ActivateScreen("Simulation");
				}
				else if(eventId == "CP1_9_3.D005")
				{
					engineer.Emotion = EmotionType.POSITIVE;
				}
				else if(eventId == "CP1_9_3.D007")
				{
					engineer.Emotion = EmotionType.NEGATIVE;
				}
				else if(eventId == "CP1_9_3.D008")
				{
					engineer.Emotion = EmotionType.NEUTRAL;
				}
				else if(eventId == "CP1_9_3.D010")
				{
					engineer.Emotion = EmotionType.POSITIVE;
				}
				else if(eventId == "CP1_9_3.D012")
				{
					engineer.Emotion = EmotionType.NEUTRAL;
				}
				else if(eventId == "CP1_9_3.D017")
				{
					engineer.Emotion = EmotionType.POSITIVE;
				}
				else if(eventId == "CP1_9_3.D0191")
				{
					engineer.Emotion = EmotionType.NEUTRAL;
				}
				else if(eventId == "CP1_9_3.D023")
				{
					engineer.Emotion = EmotionType.POSITIVE;
				}
				else if(eventId == "CP1_9_3.D025")
				{
					engineer.Emotion = EmotionType.NEUTRAL;
				}
				else if(eventId == "CP1_9_3.D026")
				{
					engineer.Emotion = EmotionType.POSITIVE;
				}
				else if(eventId == "CP1_9_3.D031")
				{
					if(Achievements.Instance.Get(Z0000_GameObjectives.Instance.completeEngineerUUID).completed == false)
						GameManager.Instance.Event("ACHIEVEMENT", 
					                           Z0000_GameObjectives.Instance.completeEngineerUUID, 
					                           "completed");
					UIManager.Instance.ActivateScreen("Simulation");
				}
				else if(eventId == "CP1_9_5.D015")
				{
					gameManager.Event("POPUP_DECISION", "005", "show");
				}
				else if(eventId == "CP1_9_5.D016")
				{
					dialogView.ActivateDialog("D006");
				}
				else if(eventId == "CP1_9_5.D018")
				{
					DialogManager.Instance.LoadStory("CP1_9_6", ProcessDialogAnswer);
				}
				else if(eventId == "CP1_9_6.D004")
				{
					UIManager.Instance.ActivateScreen("Simulation");
				}
				
			}
			
		}//ProcessDialogEvent()
		
		//------------------------------------------------------------------------------
		/// <summary>
		/// Logic for process event from popup system
		/// </summary>
		void ProcessPopupEvent(string eventId, string data)
		{
			if(data == "yes" && eventId == "005")
			{
				DialogManager.Instance.LoadStory("CP1_9_5", "D016", ProcessDialogAnswer);
			}
			else if( data == "no" && eventId == "005")
			{
				DialogManager.Instance.LoadStory("CP1_9_5", "D017", ProcessDialogAnswer);
			}
		}//ProcessPopupEvent()
		
		/// <summary>
		/// Process multi choice answer.
		/// TODO this is just an example for now, we can later improve this
		/// score Logic
		/// </summary>
		public void ProcessDialogAnswer(UIDialog dialog, EmotionType answer)
		{
			if(dialog.Id == "D002")
			{
				//ProcessQuestion1(answer);
			}
			else if(dialog.Id == "D006")
			{
				
				// ProcessQuestion2(answer);
			}
		}//ProcessAnswer()
		
	}//class Announcement
	
}//namespace
//------------------------------------------------------------------------------

