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

using Epigene.UI;

using Epigene.AUDIO;
using Epigene.GAME;

namespace Epigene
{

	//TODO: 
	// - add onHover and onRelease
	// - resource name should be automatic from sprite

	//This cause trouble to update the slider's positions.. 
	//[ExecuteInEditMode]
	public class Button3D : MonoBehaviour 
	{
		//new
		public enum ButtonType {PUSH, TOGGLE};
		//old replace THIS later with line above
		//public bool toggle = false;

		public ButtonType type = ButtonType.PUSH;

		public string eventID;
		public GameObject handler;
		public string onClick;
		public string onPressed;
		public string onRelease;
		public string onOver;
		public string onOverExit;
		public string onActive;

		//sound effects
		public string ClickAudio;
		public string OverAudio;
		public string ReleaseAudio;

		public enum ButtonState {RELEASE = 0, PUSH, OVER, DISABLED, 
								ACTIVE_RELEASE, ACTIVE_PUSH, ACTIVE_OVER, ACTIVE_DISABLED};
		/// <summary>
		/// default sprite list
		/// </summary>
		public GameObject[] states = new GameObject[8];

		/// <summary>
		/// Name of the button
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
			set{ gameObject.name = value; }
		}

		/// <summary>
		/// get/set states of button
		/// </summary>
		public ButtonState State
		{
			set
			{
				state = value;				
				GameObject activeObj = null;
				for(int i =0; i < states.Length; ++i)
				{
					GameObject obj = states[i];
					if(obj != null)
					{
						if(i == (int)state)
						{
							obj.SetActive(true);
							activeObj = obj;
						}
						else if(obj != activeObj)
						{
							//only disabled if not the same
							obj.SetActive(false);
						}
					}
				}

				//if no object for state, use zero item
				if(activeObj == null && states[0] != null)
				{
					states[0].SetActive(true);
				}

			}
			get
			{
				return state;
			}
		}
		private ButtonState state;

		private AudioManager audioManager;

		private bool isToggled;

		void Awake()
		{
			//only add collider if not yet exist
			Collider c = gameObject.GetComponent<Collider>();
			if(c == null)
			{
				gameObject.AddComponent<SphereCollider>();
			}

			audioManager = AudioManager.Instance;

			//Log.Assert(states[0] != null, "Need to assign at least one state");

		}

		/// <summary>
		/// Enable this button - register it
		/// </summary>
		void OnEnable()
		{
			State = ButtonState.RELEASE;
		}	

		/// <summary>
		/// Disable this button (un register)
		/// </summary>
		void OnDisable()
		{
			//Log.Debug("Button OnDisable in "+gameObject.name);

		}
		 
		

		/// <summary>
		/// Button OnClick handler
		/// </summary>
		//public virtual void ButtonOnClick(UIButton button)
		public void OnMouseDown()
		{

			Log.Debug("OnMouseDown "+name);
	
			State = (type == ButtonType.TOGGLE && isToggled) ? ButtonState.ACTIVE_PUSH : ButtonState.PUSH;

			if(handler != null && onPressed != "")
			{
				handler.SendMessage(onPressed, gameObject.name);
			}
		}
	
		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void OnMouseUp()
		{
			Log.Debug("OnMouseUp "+name);

			//left click
			if(!Input.GetMouseButtonDown(0))
			{
				isToggled = !isToggled;

				Log.Debug("onClick - Button "+ gameObject.name);
				if(handler != null && onClick != "")
				{
					handler.SendMessage(onClick, gameObject.name);
					if(ClickAudio.Length > 0)
						audioManager.PlaySound(ClickAudio);

				}

				if(eventID != "")
				{
					GameManager.Instance.Event("BUTTON", eventID, "onClick");
				}
			}

			State = (type == ButtonType.TOGGLE && isToggled) ? ButtonState.ACTIVE_RELEASE : ButtonState.RELEASE;
			
			if(handler != null && onRelease != "")
			{
				handler.SendMessage(onRelease, gameObject.name);
				if(ReleaseAudio.Length > 0)
					audioManager.PlaySound(ReleaseAudio);
			}
			

		}
		
		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void OnMouseEnter()
		{

			State = (isToggled) ? ButtonState.ACTIVE_OVER : ButtonState.OVER;
			
			//Log.Info("onOver - Button");
			if(handler != null && onOver != "")
			{
				handler.SendMessage(onOver, gameObject.name);			
				if(OverAudio.Length > 0)
					audioManager.PlaySound(OverAudio);

			}

		}



		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void OnMouseExit()
		{

			State = (isToggled) ? ButtonState.ACTIVE_RELEASE : ButtonState.RELEASE;

			//Log.Info("onOverExit - Button");
			if(handler != null && onOverExit != "")
			{
				handler.SendMessage(onOverExit, gameObject.name);			
			}
		}

		/// <summary>
		/// helper to activate a button object
		/// </summary>
		public void SetActive(bool flag)
		{
			gameObject.SetActive(flag);
		}

		/// <summary>
		/// helper to set the button to active state (sprite)
		/// </summary>
		public void Toggle(bool flag = true)
		{
			isToggled = flag;
		}

	}//class

}//namespace