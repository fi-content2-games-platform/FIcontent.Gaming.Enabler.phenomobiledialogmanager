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
	public class Button : MonoBehaviour 
	{
		//new
		//public enum ButtonType {Push, Toggle};
		//old replace THIS later with line above
		public bool toggle = false;

		//public ButtonType type = ButtonType.Push;

		public string resourcePath;
		/// <summary>
		/// default sprite list
		/// </summary>
		public int[] spriteOrder = new int[8]{0,1,2,3,4,5,6,7};

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


		public Color upColor;
		public Color downColor;
		public Color overColor;
		public Color disabledColor;
		public Color activeColor;


		//public GameObject text;
		public UIText text;
		protected UIType uiType = UIType.Button;

		/// <summary>
		/// Get the uiButton object
		/// </summary>
		public UIButton UIButton
		{
			get
			{
				//if(uiButton == null)
					//Init();

				return uiButton;
			}
		}
		private UIButton uiButton;

		/// <summary>
		/// Name of the button
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
			set{ gameObject.name = value; }
		}

		private AudioManager audioManager;

//-------------------------------- basic unity API MonoBehavior class methods ---
//
		void Start()
		{
			
			//UIManager.ScreenChanged = ScreenChanged;
			


		}

		/// <summary>
		/// Enable this button - register it
		/// </summary>
		void OnEnable()
		{
			//Log.Debug("Button OnEnable in "+gameObject.name);
			
			audioManager = AudioManager.Instance;
			Init();

			//UIManager.ScreenChanged = ScreenChanged;
		}

		/// <summary>
		/// Disable this button (un register)
		/// </summary>
		void OnDisable()
		{
			//Log.Debug("Button OnDisable in "+gameObject.name);
			//unregister the button
			 UIManager.Instance.Remove(gameObject.GetInstanceID());			 


		}
		 
		/// <summary>
		/// Initialize the button and register to ui
		/// </summary>
		public void Init()
		{
			Log.Debug(gameObject.name+ " Init");

			UIManager uiManager = UIManager.Instance;

			//only add collider if not yet exist
			Collider2D c = gameObject.GetComponent<Collider2D>();
			if(c == null)
			{
				//gameObject.AddComponent<PolygonCollider2D>();
				gameObject.AddComponent<BoxCollider2D>();
			}

			//Log.Debug(gameObject.name+" ============== uiType:"+uiType);
			if(uiType == UIType.Button && toggle)
				uiType = UIType.ToggleButton;

			//TODO FIXME this should NOT be here
			// if((gameObject.name.EndsWith("btn_SoundEffects")) || 
			//    (gameObject.name.EndsWith("btn_Sound")))
			// 	uiType = UIType.SoundButton;

			// if((gameObject.name.EndsWith("btn_CityHall")) ||
			//    (gameObject.name.EndsWith("btn_Engineer")) ||
			//    (gameObject.name.EndsWith("btn_Settings")) ||
			//    (gameObject.name.EndsWith("btn_LeaveScreen")) ||
			//    (gameObject.name.EndsWith("btn_MessageHead")) ||
			//    (gameObject.name.EndsWith("btn_Emails")) ||
			//    (gameObject.name.EndsWith("btn_Play")))
			// 	uiType = UIType.SettingsButton;
			
			// if(gameObject.name.EndsWith("btn_Earth") || 
			//    gameObject.name.EndsWith("btn_Future") || 
			//    gameObject.name.EndsWith("btn_Sun") || 
			//    gameObject.name.EndsWith("btn_Water") || 
			//    gameObject.name.EndsWith("btn_WhatIsEnergy") || 
			//    gameObject.name.EndsWith("btn_Wind"))
			// 	uiType = UIType.TouchButton;


			// TODO fix this for more cases, localization and
			//use resourcePath only if given
			if(resourcePath.Length > 0)
			{
				// Log.Info("add res:"+resourcePath);
				uiButton = (UIButton)uiManager.Add(uiType, gameObject, resourcePath);
			}			
			else
			{
				////check if we have sprite to use instead
				SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
				if(sr != null && sr.sprite != null)
				{
					//TODO fix localization and find a way to load the rest of the sprites..
					Log.Info("We have a sprite:"+gameObject.name);
					uiButton = (UIButton)uiManager.Add(uiType, gameObject, "", sr.sprite);
				}
				else
				{
					Log.Debug("No resource nor sprite set in button: "+gameObject.name);
					uiButton = (UIButton)uiManager.Add(uiType, gameObject, "");
					
				}
			}

			uiButton.SetSpriteOrder(spriteOrder);
			Register(uiButton);

			if(text != null)
			{			
				uiButton.SetTextColors(
					upColor, downColor, overColor, disabledColor, activeColor);
				uiButton.SetText(text);
			}

		}//Init()

		/// <summary>
		/// Register the button functions
		/// </summary>
		public virtual void Register(UIButton b)
		{
			b.Register(UIEvent.Pressed, ButtonOnPressed);
			b.Register(UIEvent.Release, ButtonOnRelease);
			b.Register(UIEvent.Click, ButtonOnClick);
			b.Register(UIEvent.Over, ButtonOnOver);
			b.Register(UIEvent.OverExit, ButtonOnOverExit);
		
		}//Register()

		/// <summary>
		/// Button OnClick handler
		/// </summary>
		public virtual void ButtonOnClick(UIButton button)
		{
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


		}//buttonOnClick()

		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void ButtonOnPressed(UIButton button)
		{
			//Log.Info("onClick - Button");
			if(handler != null && onPressed != "")
			{
				handler.SendMessage(onPressed, gameObject.name);
			}

		}//buttonOnClick()
	
		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void ButtonOnRelease(UIButton button)
		{

			//Log.Info("onRelease - Button");
			if(handler != null && onRelease != "")
			{
				handler.SendMessage(onRelease, gameObject.name);
				if(ReleaseAudio.Length > 0)
					audioManager.PlaySound(ReleaseAudio);
			}

		}//ButtonOnRelease()
		
		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void ButtonOnOver(UIButton button)
		{
			//Log.Info("onOver - Button");
			if(handler != null && onOver != "")
			{
				handler.SendMessage(onOver, gameObject.name);			
				if(OverAudio.Length > 0)
					audioManager.PlaySound(OverAudio);

			}

		}//ButtonOnOver()

		/// <summary>
		/// Button handler
		/// </summary>
		public virtual void ButtonOnOverExit(UIButton button)
		{
			//Log.Info("onOverExit - Button");
			if(handler != null && onOverExit != "")
			{
				handler.SendMessage(onOverExit, gameObject.name);			
			}
		}//ButtonOnExit()

		/// <summary>
		/// helper to activate a button object
		/// </summary>
		public void SetActive(bool flag)
		{
			gameObject.SetActive(flag);
		}//SetActive()

		/// <summary>
		/// helper to "release" a button
		/// </summary>
		public void Up()
		{
			uiButton.LockSprite = false;
			uiButton.State = UIButtonState.RELEASE;
		}

		/// <summary>
		/// helper to "push" a button
		/// </summary>
		public void Down()
		{
			uiButton.LockSprite = false;
			uiButton.State = UIButtonState.PUSH;
			uiButton.LockSprite = true;
		}

		/// <summary>
		/// helper to set the button to active state (sprite)
		/// </summary>
		public void Toggle(bool flag = true)
		{
			//only if toggle button
			if(toggle)
			{
				((UIToggleButton)uiButton).Active = flag;
			}
		}

	}//class Button

}//namespace