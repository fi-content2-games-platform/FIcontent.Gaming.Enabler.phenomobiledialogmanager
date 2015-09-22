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

using Epigene;
using Epigene.UI;
using Epigene.GAME;

namespace TWISTBueBe
{
	/// <summary>
	/// Save simulation popup to manage saving of simulations.
	/// Occurs first in CP1_9 
	/// </summary>
	public class Invite : MonoBehaviour 
	{

		/// <summary>
		/// holder
		/// </summary>
		public GameObject panel;
		/// <summary>
		/// holder
		/// </summary>
		public GameObject panel2;
		/// <summary>
		/// id of current message
		/// </summary>
		public Button[] slots;
		
		/// <summary>
		/// which slots should be used to save
		/// </summary>
		private int selectedSlot;
		
		// Use this for initialization
		void Awake () 
		{
			Log.Assert(panel, "Please assign the panel in "+gameObject.name);
			Log.Assert(panel2, "Please assign the panel2 in "+gameObject.name);
			
			Log.Assert(slots != null, "Please assing slots in "+gameObject.name);
			Log.Assert(slots.Length == 3, "Please assing 5 slots in "+gameObject.name);

			Show();

		}//Awake()

		/// <summary>
		/// Enable this panel will trigger to initialize
		/// </summary>
		void OnEnable()
		{
			Show();

			GameManager.Instance.RegisterEventHandler("INVITE", EventHandler, gameObject);
		}
		
		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDisable()
		{
			GameManager.Instance.RemoveEventHandler("INVITE", EventHandler);
		}

		
		/// <summary>
		/// Show the panel
		/// </summary>
		public void Show()
		{
			//TODO check already savInvite
			panel.SetActive(true);
			panel2.SetActive(false);

		}//Show()

		/// <summary>
		/// Hide Invite
		/// </summary>
		public void Hide()
		{
			panel.SetActive(false);	
			panel2.SetActive(false);
		}//Hide()

		/// <summary>
		/// Handle buttons
		/// </summary>
		public void OnClick(string button)
		{
			if(button == "btn_Open")
			{
				
				//TODO animate later
				panel2.SetActive(true);
				
			}
			else if( button == "btn_Close")
			{
				
				//TODO animate later
				panel2.SetActive(false);
			}
			else
			{
				//check if any of the slot button is pressed
				for(int i = 0; i < slots.Length; i++)
				{
					//TODO set and LOCK sprite 
					if(button == slots[i].Name)
					{					
						selectedSlot = i + 1;
						slots[i].UIButton.Sprite = 1;
						slots[i].UIButton.LockSprite = true;
					}
					else
					{
						slots[i].UIButton.LockSprite = false;
						slots[i].UIButton.Sprite = 0;
					}
				}
			}

			Log.Debug("selectedSlot ="+selectedSlot);

		}//OnClick()

		/// <summary>
		/// Function to check new events.
		/// </summary>
		public void EventHandler(string eventId, string param)
		{

			//add new mail
			if(param == "show")
			{
				Show();
			}
			else if(param == "hide")
			{
				Hide();
			}

		}//EventHandler()

	}//class Invite
}//namespace
