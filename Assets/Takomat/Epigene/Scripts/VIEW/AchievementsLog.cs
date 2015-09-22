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
using Epigene.MODEL;


using Epigene.UI;
//------------------------------------------------------------------------------
namespace Epigene.VIEW
{
	/// <summary>
	/// Achievement log.
	//TODO Later we have an acheivement list with uuid and the semantics generated 
	/// from the code here.
	/// </summary>
	public class AchievementsLog : MonoBehaviour 
	{	
		/// <summary>
		/// Main button to activate/hide the hud
		/// </summary>
		//public UIButton btnNote;
		//public GameObject btnNoteObj;
		public Button btnNote;
		/// <summary>
		/// the main hud
		/// </summary>
		public GameObject hud;

		public List<AchievementNpc> slots;

		/// <summary>
		/// currently active slot
		/// or -1 if none
		/// </summary>
		private int activeSlot;

		/// <summary>
		/// navigation arrow for characters
		/// </summary>
		public Button rightArrow;
		/// <summary>
		/// navigation arrow for characters
		/// </summary>
		public Button leftArrow;
		/// <summary>
		/// log panel
		/// </summary>
		public GameObject logPanel;		
		/// <summary>
		/// text of the activeated achievement
		/// </summary>
		public UIText logText;
		/// <summary>
		/// time of the log
		/// </summary>
		public UIText logTime;

		/// <summary>
		/// ui manager
		/// </summary>
		private UIManager uiManager;
		/// <summary>
		/// achievements model
		/// </summary>
		private Achievements logs;

		/// <summary>
		/// store the last number of items
		/// </summary>
		private int 	 lastNumber;

		private string   gBadge = "HUD/btn_Badge_AchievementSlot";

		public Sprite[]  achievementSprite;

		public int 		 logPages;
		public int 		 activePage;
		public int 	     maxAmountOfAchievementsVisible = 4;
		
		//------------------------------------------------------------------------------

		/// <summary>
		/// Initialize the components
		/// </summary>
		public virtual void Awake () 
		{
			//Debug.Log ("AchievementsLogs Awake");
			GameObject obj = null;

			uiManager = UIManager.Instance;

			hud = GetChildObject("HUD");
			logPanel = GetChildObject("HUD/popup_Achievements");			

			obj = GetChildObject("HUD/popup_Achievements/txt_Note");
			logText = obj.GetComponent<UIText>();
			logText.wrapSize = 40;
			obj = GetChildObject("HUD/popup_Achievements/txt_Time");
			logTime = obj.GetComponent<UIText>();

			//use Slots class here?
			slots = new List<AchievementNpc>();
			slots.Add(GetChildObject(gBadge+"1").GetComponent<AchievementNpc>());
			slots.Add(GetChildObject(gBadge+"2").GetComponent<AchievementNpc>());
			slots.Add(GetChildObject(gBadge+"3").GetComponent<AchievementNpc>());
			slots.Add(GetChildObject(gBadge+"4").GetComponent<AchievementNpc>());

			obj  	   = GetChildObject ("HUD/btn_Arrow_Achievements_left");
			leftArrow  = obj.GetComponent<Button> ();
			obj		   = GetChildObject ("HUD/btn_Arrow_Achievements_right");
			rightArrow = obj.GetComponent<Button> ();
			logs = Achievements.Instance;

			activeSlot = -1;

			logPages = 0;
			activePage = 0;

			//TODO : The GameManager.Remove(EventHandler); is missing
			// Where to set?
			// As the AchievementsLog is only once set in a game it should 
			// not be a serious issue
			GameManager.Instance.RegisterEventHandler(
				"ACHIEVEMENT", ProcessAchievementEvent, gameObject);

			Show(false);

			achievementSprite = UIManager.LoadSprite (
				"Game/Sprites/Achievement/badge_Achievements");
			ResetSlots (0);
			
		}//Awake()
//------------------------------------------------------------------------------


		/// <summary>
		/// Function to check new events.
		/// Here the game logic is implemented.
		/// For different games it should work different.
		/// </summary>
		public void ProcessAchievementEvent(string _eventId, string _param)
		{
			if(_param == "completed")
			{
				if((_eventId == "26b02e36-4fa0-4f0c-98f0-9915646483d1") ||
				   (_eventId == "b27a3ba6-745b-4797-a1a7-1e5a569ab510") ||
				   (_eventId == "1f0f2c91-9a58-4bbc-af2c-5330f1ce0208"))
				{
					// skip
				}
				else 
				{
					ResetSlots (activePage);
					uiManager.BlinkImage(btnNote.UIButton, 3, 500);
					logPages = (Achievements.Instance.achievementsDoneOrStarted.Count-1);
					logPages /= maxAmountOfAchievementsVisible;
					activePage = logPages;
					ResetSlots(activePage);
				}
			}
		}

		/// <summary>
		/// Helper to get a child object 
		/// and report errors if needed
		/// </summary>
		GameObject GetChildObject(string name)
		{
			GameObject obj = GameObject.Find(gameObject.name+"/"+name);
			Log.Assert(obj, "Could not find object for:"+name);
			return obj;

		}//GetChildObject();
		
		/// <summary>
		/// Enable this log
		/// </summary>
		void OnEnable()
		{
			//btnNote.State = UIButtonState.DISABLED;
			//
			Log.Info("------------------- Enabled LOG");
			Show(false);

			//UIManager.ScreenChanged = ScreenChanged;

		}//OnEnable()

		void OnDisable()
		{
			//Show(false);
		}


		void Show(bool flag)
		{	
			Log.Debug(gameObject.name+" Show:"+flag);
			hud.SetActive(flag);
			logPanel.SetActive(false);

			ResetSlots(activePage);

		}
//------------------------------------------------------------------------------
		/// <summary>
		/// Handle buttons OnClick events
		/// </summary>
		public void OnClick(string button)
		{
			Log.Debug("Click:"+button);

			if(button == "btn_Note")
			{
				Log.GameTimes ("Click:"+gameObject.name);
				Show(!hud.active);
				ResetSlots(activePage);
				//Comment in to save Achievement Images
				/*
				if (hud.active)
					saveAchievementImagesToPNG ();
				*/
			}
			//slots
			else if(button.EndsWith("Slot1"))
			{
				ToggleSlot(0);
			}
			else if(button.EndsWith("Slot2"))
			{
				ToggleSlot(1);
			}
			else if(button.EndsWith("Slot3"))
			{
				ToggleSlot(2);
			}
			else if(button.EndsWith("Slot4"))
			{
				ToggleSlot(3);
			}
			else if(button.EndsWith("left"))
			{
				activePage -= 1;
				ResetSlots(activePage);
				logPanel.SetActive(false);
			}
			else if(button.EndsWith("right"))
			{
				activePage += 1;
				ResetSlots(activePage);
				logPanel.SetActive(false);
			}
		}

//------------------------------------------------------------------------------
		/// <summary>
		/// Reset the slots based on Achievements
		/// </summary>
		void ResetSlots(int page)
		{
			logPages = (Achievements.Instance.achievementsDoneOrStarted.Count-1) / 4;

			int start = page * 4;
			List<Achievement> items = logs.Items;
			// Log.GameTimes("Reset AchievementLog items.count:"+items.Count);

			if (leftArrow != null)
			{
				if (page > 0) 	 	 leftArrow.SetActive(true); 	//leftArrow.UIButton.State  = UIButtonState.ACTIVE;
				else 				 leftArrow.SetActive(false); 	//leftArrow.UIButton.State  = UIButtonState.DISABLED;
			}
			if (rightArrow != null)
			{
				if (page < logPages) rightArrow.SetActive(true);	//rightArrow.UIButton.State = UIButtonState.ACTIVE;
				else 				 rightArrow.SetActive(false); 	//rightArrow.UIButton.State = UIButtonState.DISABLED;
			}

			if(items.Count > 0)
			{
				// Log.GameTimes("todo:");
				for(int i = 0; i < slots.Count; i++)
				{
					// Log.GameTimes("for:"+i);
					if (start < Achievements.Instance.achievementsDoneOrStarted.Count)
					{
						if (i+start < Achievements.Instance.achievementsDoneOrStarted.Count)
						{
							//set item	
							// Log.GameTimes("show:"+i);
							slots[i].Show(true);
							slots[i].Check 	= items[Achievements.Instance.achievementsDoneOrStarted[i+start]].completed;
							slots[i].Npc 	= items[Achievements.Instance.achievementsDoneOrStarted[i+start]].npc;
							logText.Text 	= items[Achievements.Instance.achievementsDoneOrStarted[i+start]].text;

							//TODO 
							//Second Version : Set the correct time. But this needs Design. 
							// In the CP1_7 now real time can be given.
							logTime.Text = "";
							// Second version, if LogTime is correctly set : items[i].LogTime;
						}
						else
						{
							//no more item
							//if(slots[i]!=null)
							{
								// Log.GameTimes("dont show:"+i);
								slots[i].Show(false);
							}
						}

					}
					else
					{
							slots[i].Show(false);
					}
				}
				

			}

			else
			{
				hud.SetActive(false);
			}

			Log.Info("lastNumber:"+lastNumber);

			//trigger blinking if new items
			//if(items.Count > lastNumber)
			{

			}

			//slots[0].Show(true);

			activeSlot = -1;
			lastNumber = items.Count;
		}//ResetSlot()

//------------------------------------------------------------------------------
		/// <summary>
		/// Open the corresponding slot with 
		/// the right text in it
		/// </summary>
		void ToggleSlot(int id)
		{
			List<Achievement> items = logs.Items;
			bool flag = !(id == activeSlot);
			Log.Debug("toggleSlot("+id+"):"+flag);

			//hide others
			if(flag)
			{
				foreach(AchievementNpc slot in slots)
					slot.SetActive(false);
			}

			
			if (items[Achievements.Instance.achievementsDoneOrStarted[id+(activePage*4)]].completed)
				logText.Text = items[Achievements.Instance.achievementsDoneOrStarted[id+(activePage*4)]].infotext;
			else 
				logText.Text = "Achievement '" + items[Achievements.Instance.achievementsDoneOrStarted[id+(activePage*4)]].text + "' wurde gestartet!";

			logPanel.SetActive(flag);
			slots[id].SetActive(flag);

			activeSlot = (flag) ? id : -1;

		}//ToggleSlot()

		/// <summary>
		/// Reset the slots after every screen change,
		/// This will make sure we have a valid state
		/// </summary>
		public void ScreenChanged(UIScreen oldScreeen, UIScreen newScreen)
		{

			if(gameObject.active && newScreen != null)
			{
				ResetSlots(activePage);
			}
		

		}//ScreenChanged()

		public void saveAchievementImagesToPNG()
		{
			string 			name		= "btn_Badge_AchievementSlot1";
			GameObject 		obj 		= GameObject.Find(hud.name+"/"+name);
			GameObject		npc			= GameObject.Find(obj.name+"/"+"npc");
			SpriteRenderer 	sprites 	= npc.GetComponent<SpriteRenderer> ();
			Sprite 			oldSprite	= sprites.sprite;

			for (int i = 0; i < achievementSprite.Length; i++)
			{
				sprites.sprite =  achievementSprite[i];
				Texture2D  	achImage = UIManager.CreateTexture(obj, new Rect (0, 0, 25, 25), 0.25f);
				UIManager.SaveTextureToFile (achImage, "achievemntImage_0" + i + ".png");
			}

			sprites.sprite = oldSprite;
		}

	}//class AchievementLog

}//namespace
//------------------------------------------------------------------------------