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
//-------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Epigene;
using Epigene.VIEW;
using Epigene.GAME;
using Epigene.UI;
using Epigene.MODEL;
//------------------------------------------------------------------------------
namespace TWISTBueBe.VIEW
{
	/// <summary>
	/// Helper class for navigation with buttons
	/// </summary>
	public class AchievementsLogShow : AchievementsLog
	{

		GameObject simObj;
		private Achievements logs;
		private int lastNumberShow;
		private int activeSlotShow;

		public override void Awake()
		{
			//Debug.Log ("You called me");
			base.Awake();

			logs = Achievements.Instance;
			lastNumberShow = 0;
			activeSlotShow = -1;


		}

		/// <summary>
		/// Enable the object will register event handler
		/// </summary>
		public void OnEnable()
		{
			//TODO not sure about the type, it was not defined!!
			GameManager.Instance.RegisterEventHandler("SCREEN", EventHandler);
		}
		
		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDisable()
		{
			//TODO not sure about the type, it was not defined!!
			GameManager.Instance.RemoveEventHandler("SCREEN", EventHandler);
		}

		// Update is called once per frame
		public void EventHandler(string eventId, string data)
		{
			if(eventId == "ENTER")
			{
				GameObject obj = GameObject.Find("Simulation");
				if(obj && hud.activeSelf)
					hud.SetActive(false);
				else if(!obj && !hud.activeSelf)
					hud.SetActive(true);
				ResetSlots(activePage); //is not public in AchievementLog.cs
			}
		}

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
			
			Log.Info("lastNumberShow:"+lastNumberShow);
			
			//trigger blinking if new items
			//if(items.Count > lastNumberShow)
			{
				
			}
			
			//slots[0].Show(true);
			
			activeSlotShow = -1;
			lastNumberShow = items.Count;
		}//ResetSlot()

	}//class ActivateObjects
}//namespace