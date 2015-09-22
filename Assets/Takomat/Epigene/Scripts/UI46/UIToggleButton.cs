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

#if EPIGENE_UI_46

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Epigene.GAME;
using Epigene.AUDIO;


namespace Epigene.UI
{
	public class UIToggleButton : UIButton 
	{

		// public bool Toggle
		// {
		// 	set{ 
		// 		toggle = value;
		// 		State = UIButtonState.ACTIVE_RELEASE;
				
		// 	}
		// 	get{ return toggle;}
		// }
		// private bool toggle = false;

		// private spriteActiveOrder = new int[4]{	
		// 									(int)UIButtonState.ACTIVE_RELEASE,
		// 									(int)UIButtonState.ACTIVE_PUSH,
		// 									(int)UIButtonState.ACTIVE_OVER,
		// 									(int)UIButtonState.ACTIVE_DISABLED};

		public bool Active 
		{
			set
			{
				//LockSprite = false;
				if(value)
					State = UIButtonState.ACTIVE_RELEASE;
				else
					State = UIButtonState.RELEASE;
				//LockSprite = true;

			}
			get 
			{ 
				return (State >= UIButtonState.ACTIVE_RELEASE );

			}
		}


		public bool Pushed
		{
			get{ return (State == UIButtonState.PUSH);}
		}

		public UIToggleButton(GameObject obj, Sprite[] sprites)
			:base (obj, sprites)
		{
			//Pushed = false;
			this.type = UIType.ToggleButton;
		}//ctor()

		/// <summary>
		/// Constructor for create ui image from dicitionary.		
		/// </summary>
		public UIToggleButton(Dictionary<string,object> dict)
			:base(dict)
		{
			//this.pushed = false;
			this.type = UIType.ToggleButton;

		}//ctor()

		/// <summary>
		/// Set the button state and call the delegates for click.
		/// </summary>
		public override bool Click()
		{
			//Log.Info(gameObject.name+" Click():"+State+" visible="+Visible);

			blinkNumber = 1;
			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			
			if(State == UIButtonState.PUSH)
			{
				State = UIButtonState.ACTIVE_RELEASE;
			}
			else
			{
				State = UIButtonState.RELEASE;
			}


			if(onClick != null)
				onClick(this);

			if(eventId.Length > 0)
				GameManager.Instance.Event("SCREEN", eventId, gameObject.name);

			if(onClickSound != null && onClickSound.Length > 0)
				AudioManager.Instance.PlaySound(onClickSound);

			return true;
		}//Click()

		/// <summary>
		/// Set the button state and call the delegates for pressed down button.
		/// </summary>
		public override bool Push()
		{
			//Log.Info("Push():"+Name+" state:"+State);
			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			if(Active)
			{
				State = UIButtonState.ACTIVE_PUSH;
				if(onPush != null)
					onPush(this);
			}
			else
			{
				return base.Push();
			}

			return true;

		}//Pressed()

		/// <summary>
		/// Set the button state and call the delegates for relese button.
		/// </summary>
		public override bool Release()
		{
			//Log.Info("Release():"+State);

			// if (State == UIButtonState.DISABLED || !Visible)
			// 	return false;

			if(Active)
			{
				State = UIButtonState.ACTIVE_RELEASE;
				if(onRelease != null)
					onRelease(this);
			}
			else
			{
				return base.Release();
			}

			return true;


		}//Release()


		/// <summary>
		/// Set the button state and call the delegates for over button.
		/// </summary>
		public override bool Over()
		{
			//Log.Debug("Over():"+State + " gameObject:" + gameObject.name);

			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			if(Active)
			{
				State = UIButtonState.ACTIVE_OVER;
				if(onOver != null)
					onOver(this);
			}
			else
			{
				return base.Over();
			}

			return true;

		}//Over()

		/// <summary>
		/// Set the state when cursore leave the object
		/// </summary>
		public override bool OverExit()
		{
			//Log.Debug("Exit():"+this.state + " gameObject:" + gameObject.name);

			// if (State == UIButtonState.DISABLED || !Visible)
			// 	return false;

			if(Active)
			{
				State = UIButtonState.ACTIVE_RELEASE;
				if(onOverExit != null)
					OnOverExit(this);
			}
			else
			{

				return base.Over();
			}
			
			return true;
		}//OverExit()

		void handleToggle(string gameObjectName, bool val)
		{
			UnityEngine.UI.Toggle t = this.gameObject.GetComponent<UnityEngine.UI.Toggle>();
			UnityEngine.UI.Image i = this.gameObject.GetComponent<UnityEngine.UI.Image>();
			
			UnityEngine.UI.SpriteState state = t.spriteState;
			if (sprites != null && sprites.Length >= 4)
			{
				if (val)
				{
					i.sprite = sprites[spriteOrder[(int)UIButtonState.RELEASE]];
					state.pressedSprite = sprites[spriteOrder[(int)UIButtonState.PUSH]];
					state.highlightedSprite = sprites[spriteOrder[(int)UIButtonState.OVER]];
					state.disabledSprite = sprites[spriteOrder[(int)UIButtonState.DISABLED]];
				}
				else
				{
					i.sprite = sprites[spriteOrder[(int)UIButtonState.ACTIVE_RELEASE]];
					state.pressedSprite = sprites[spriteOrder[(int)UIButtonState.ACTIVE_PUSH]];
					state.highlightedSprite = sprites[spriteOrder[(int)UIButtonState.ACTIVE_OVER]];
					state.disabledSprite = sprites[spriteOrder[(int)UIButtonState.ACTIVE_DISABLED]];
				}
			}
			t.spriteState = state;
			i.SetNativeSize();
		}

		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameters
		/// </summary>	
		protected override void Parse(Dictionary<string,object> dict)
		{
			//Log.Info("<color=green>PARSE!!!!!!!!!!!!!!!!!!!!!!!!!</color>");
			Debug.Log("<color=green>PARSE!!!!!!!!!!!!!!!!!!!!!!!!!</color>");
		
			// this.gameObject.transform.parent = GameObject.Find("Canvas").transform;

			UnityEngine.UI.Toggle t = this.gameObject.AddComponent<UnityEngine.UI.Toggle>();

			//parse base stuff
			base.Parse(dict);

			//TODO parse button handlers and functions, button spec.
			
			//"collider": "CircleCollider"
			t.onValueChanged.AddListener ( (value) => handleToggle(this.gameObject.name, value));

			if(dict.ContainsKey("event"))
			{
				this.eventId = dict["event"].ToString();
				// Log.Debug("event:"+this.eventId);
			}

			//parse on click sound
			if(dict.ContainsKey("sound"))
			{
				onClickSound = dict["sound"].ToString();
			}

			if(dict.ContainsKey("spriteOrder"))
			{
				string[] items = dict["spriteOrder"].ToString().Split(',');				

				//Log.Info("size:"+items.Length+" 0:"+items[0]);
				Log.Assert(items.Length == 4 || items.Length == 8, "spriteOrder must be 4 or 8 long with comma separated int list!");
				
				//reset
				for(int i = 0; i < 8; ++i)
				{
					spriteOrder[i] = i;
				}
				//set values
				for(int i = 0; i < items.Length; ++i)
				{

					spriteOrder[i] = Convert.ToInt32(items[i]);
				}
			}

			t.transition = UnityEngine.UI.Selectable.Transition.SpriteSwap;
			Debug.Log("<color=orange>image: " + this.GameObject.name + ", sprites.Length:" +sprites.Length +"</color>");
			t.targetGraphic = GameObject.GetComponent<UnityEngine.UI.Image>();

			UnityEngine.UI.SpriteState state = t.spriteState;
			if (sprites != null && sprites.Length >= 4)
			{
				state.pressedSprite = sprites[spriteOrder[(int)UIButtonState.PUSH]];
				state.highlightedSprite = sprites[spriteOrder[(int)UIButtonState.OVER]];
				state.disabledSprite = sprites[spriteOrder[(int)UIButtonState.DISABLED]];
			}
			t.spriteState = state;

			UnityEngine.UI.Navigation nav = t.navigation;
			nav.mode = UnityEngine.UI.Navigation.Mode.None;
			t.navigation = nav;
		}//Parse()


	}//class UIToggleButton
}//namespace

#endif //EPIGENE_UI_46
