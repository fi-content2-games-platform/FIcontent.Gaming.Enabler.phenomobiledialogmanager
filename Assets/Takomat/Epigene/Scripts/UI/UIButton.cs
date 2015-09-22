#if !EPIGENE_UI_46
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
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//------------------------------------------------------------------------------

using Epigene.GAME;
using Epigene.AUDIO;



namespace Epigene.UI
{
	/// <summary>
	/// Available states of a UIButton
	/// Convention over Configuration : 
	/// Our convention is here, that the button sprite 
	/// has all the state sprites in order of UIButtonState.
	/// This reduces the configuration.
	/// 
	/// possible configurations of Buttonstates through sprites:
	/// 	For every GameObject with the component <Button> it is 
	/// 	possible to set the size in Sprite Order to 4 or 8 which 
	/// 	correspond to the first 4 or all 8 UIButtonStates defined 
	/// 	here. Further specific sprites can be sorted to the elements 
	/// 	in Sprite Order. For example if the Button should be invisible 
	/// 	when DISABLED, the fourth Element (Element3) should be set to 
	/// 	a sprite that has no solid content. (Refer to btn_Arrow_left
	/// 	in EEZA project.) Also if the sprite order should not be correct 
	/// 	for some reason it can easily be corrected there without changing 
	/// 	the sprite-naming.
	/// 
	/// hidTouch configurations:
	/// 	If hidTouch is enabled it is crucial to lookout for the 
	/// 	RELEASE ButtonState if another button (or the same button 
	/// 	after being disabled re)appears at the same position 
	/// 	afterwards because it will trigger this button as well. 
	/// 	This Button has to be set to the OVER state because this 
	/// 	state will ignore the next click and unwanted activating
	/// 	can be avoided.
	/// </summary>
	public enum UIButtonState {RELEASE = 0, PUSH, OVER, DISABLED, 
								ACTIVE_RELEASE, ACTIVE_PUSH, ACTIVE_OVER, ACTIVE_DISABLED};

	/// <summary>
	/// Delegate functions definition for buttons
	/// </summary>
	public delegate void UIButtonFunction(UIButton button);

	/// <summary>
	/// User interface button.
	/// </summary>
	public class UIButton : UIImage
	{
		// TODO:  Use UIImage instead of sprites and/or UIBase as base class

		/// <summary>
		/// sprite order
		/// </summary>
		public int[] spriteOrder = new int[8]{	(int)UIButtonState.RELEASE,
												(int)UIButtonState.PUSH,
												(int)UIButtonState.OVER,
												(int)UIButtonState.DISABLED,
												(int)UIButtonState.ACTIVE_RELEASE,
												(int)UIButtonState.ACTIVE_PUSH,
												(int)UIButtonState.ACTIVE_OVER,
												(int)UIButtonState.ACTIVE_DISABLED};

		/// <summary>
		/// Event ID to send on action
		/// </summary>
		public string eventId ="";

		/// <summary>
		/// Current state of UIButton
		/// </summary>
		public UIButtonState State
		{
			get 
			{
				return this.state;
			}
			set 
			{
				//only process if state changed
				//if(this.state == value)
				//	return;
				if(!lockSprite)
				{
					this.state = value;
					//Log.Debug("--------- sp order:"+(int)value+"= "+value+" spr:"+spriteOrder[(int)value]);
					base.Sprite = spriteOrder[(int)value];
				}
				//TODO use base.Sprite = (int)value;				
				//if(render!=null && sprites.Length > (int)value)
				//	render.sprite = sprites[(int)value];
				//--
				

				if (this.text != null)
				{
					TextMesh mesh = this.text.gameObject.GetComponent<TextMesh>();
					//only use the first 4 item
					int v = ((int)value > 3) ? (int)value - 4 : (int)value;
					mesh.color = textColors[v];
				}
			}
		}
		private UIButtonState state;


		///<summary>
		///Flag to blocking over state
		///This flag used for compability workaround for touch and mouse drivers
		///</summary>
		protected bool blockOver = false;
		
		// /// <summary>
		// ///  Parent GameObject which this button belongs
		// /// </summary>
		// public GameObject Parent
		// {
		// 	get { return gameObject;}

		// }
		// protected GameObject gameObject;

		// /// <summary>
		// /// reference name of the button
		// /// </summary>
		// public string Name
		// {
		// 	get { return gameObject.name; }
		// }


		/// <summary>
		/// Get/Set the text string
		/// </summary>
		public string Text
		{
			get 
			{
				return (text != null) ? text.Text : "";
			}
			set 
			{
				if(text != null)
				{
					text.Text = value;
				}
			}


		}
		private UIText text;
		private Color[] textColors;

		/// <summary>
		/// Delegates for Click (button down&up)
		/// </summary>
		public UIButtonFunction OnClick
		{

			get { return onClick; }
			set { onClick = value; }
		}
		protected UIButtonFunction onClick;

		// /// <summary>
		// /// Delegates for Click (button down)
		// /// </summary>
		// public UIButtonFunction OnPressed
		// {

		// 	get { return onPressed; }
		// 	set { onPressed = value; }
		// }
		// protected UIButtonFunction onPressed;

		/// <summary>
		/// Delegates for push (button down)
		/// </summary>
		public UIButtonFunction OnPush
		{
			get { return onPush; }
			set { onPush = value; }
		}
		protected UIButtonFunction onPush;

		/// <summary>
		/// Delegates for Click (button down)
		/// </summary>
		public UIButtonFunction OnRelease
		{
			get { return onRelease; }
			set { onRelease = value; }
		}
		protected UIButtonFunction onRelease;

		/// <summary>
		/// Delegates for Click (button down)
		/// </summary>
		public UIButtonFunction OnOver
		{
			get { return onOver; }
			set { onOver = value; }
		}
		protected UIButtonFunction onOver;
	
		public UIButtonFunction OnOverExit
		{
			get { return onOverExit; }
			set { onOverExit = value; }
		}
		protected UIButtonFunction onOverExit;
		
		//TODO : Nice would be to have the sprites file name directly taken 
		//       from the sprite renderer
		/// <summary>
		/// sprite sheets of this button
		/// </summary>
		private Sprite[] sprites;

		/// <summary>
		/// Every UIImage needs a sprite renderer
		/// </summary>
		private SpriteRenderer render;

		/// <summary>
		/// sound effects for onClick
		/// </summary>
		protected string onClickSound;

//------------------------------------------------------------------------------

		public UIButton(GameObject obj, Sprite[] sprites)
			:base (obj, sprites)
		{

			//Log.Debug("UIButton "+obj.name);

			this.type = UIType.Button;
			this.onClick = onClick;
			this.onRelease = onRelease;
			this.onOver = onOver;

			textColors = new Color[5];

			if(sprites != null)
			{
				this.render = obj.GetComponent<SpriteRenderer>();
				//if(this.render == null)
				//	Log.Error("Button must have a SpriteRenderer! Button:"+obj.name);

			}

		}//ctor()

		public virtual void Reset()
		{
			lockSprite = false;
			// if(GameManager.Instance.hidTouch == false)
				this.State = UIButtonState.RELEASE;

			blockOver = false;
		}

		/// <summary>
		/// Constructor for create ui image from dicitionary.		
		/// </summary>
		public UIButton(Dictionary<string,object> dict)
			:base(dict)
		{

			this.type = UIType.Button;

			//Parse(dict);

		}//ctor()

		/// <summary>
		/// Parse the content of the dictionary
		/// and create a new gameobjet with this parameters
		/// </summary>
		protected override void Parse(Dictionary<string,object> dict)
		{
			//Log.Info("<color=green>PARSE!!!!!!!!!!!!!!!!!!!!!!!!!</color>");

			//parse base stuff
			base.Parse(dict);

			//TODO parse button handlers and functions, button spec.
			
			//"collider": "CircleCollider"

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
				int i = 0;
				for(i = 0; i < 8; ++i)
				{
					spriteOrder[i] = 0;
				}
				//set values
				for(i=0; i < items.Length; ++i)
				{

					spriteOrder[i] = Convert.ToInt32(items[i]);
					// Log.Debug("sprite "+i+"="+spriteOrder[i]);
				}
			}

		}//Parse()

		/// <summary>
		/// Set the text of the button
		/// 
		/// </summary>
		public void SetText(UIText text)
		{
			this.text = text;
		}

		public void SetTextColors(
			Color upColor, Color downColor, 
			Color overColor, Color disabledColor,
						Color activeColor)
		{
			textColors[(int)UIButtonState.RELEASE] = upColor;
			textColors[(int)UIButtonState.PUSH] = downColor;
			textColors[(int)UIButtonState.OVER] = overColor;
			textColors[(int)UIButtonState.DISABLED] = disabledColor;
			//FIXME??
			//textColors[(int)UIButtonState.ACTIVE] = activeColor;
		}


		/// <summary>
		/// Register events to a ui button
		/// A button has 3 valid functions:
		/// Click, Release adn Over. The rest will not be processed.
		/// Return true if registration was successfull
		/// </summary>
		public bool Register(UIEvent uiEvent, UIButtonFunction function)
		{
			
			switch(uiEvent)
			{
				case UIEvent.Click:
					onClick += function;
					break;

				case UIEvent.Pressed:
					onPush += function;
					break;

				case UIEvent.Release:
					onRelease += function;
					break;

				case UIEvent.Over:
					onOver += function;
					break;

				case UIEvent.OverExit:
					onOverExit += function;
					break;

				default:
					Log.Error("Invalid event type for Button:"+uiEvent);
					return false;
			}	
			
			return true;
			
		}//Register()
//------------------------------------------------------------------------------
		/// <summary>
		/// Check if ui component is belongs to the same object
		/// </summary>
		public bool Contains(GameObject gameObject)
		{
			if(this.gameObject == gameObject)
				return true;

			return false;
		}

		/// <summary>
		/// set sprite order
		/// </summary>
		public void SetSpriteOrder(UIButtonState state, int spriteNum)
		{
			spriteOrder[(int)state] = spriteNum;
			State = UIButtonState.RELEASE;
		}

		/// <summary>
		/// Reset the sprite order with the new list.
		/// the list have to contain 8 elems!
		/// </summary>
		public void SetSpriteOrder(int[] newOrder)		
		{
			//Log.Assert(newOrder.Length == 8, "SpriteOrder length must be 8 ");
			spriteOrder = newOrder;
			State = UIButtonState.RELEASE;
		}

		/// <summary>
		/// Set the button state and call the delegates for click.
		/// </summary>
		public virtual bool Click()
		{
			//// Log.GameTimes("Click():"+this.state+" visible="+Visible);
			blinkNumber = 1;
			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			if(onClick != null )//&& GameManager.Instance.hidTouch == false)
				onClick(this);

			//TODO make it nicer and use only one!!
			//GameManager.Instance.Event(GameEvent.SCREEN, gameObject.name, "click");
			if(eventId.Length > 0)
				GameManager.Instance.Event("SCREEN", eventId, gameObject.name);

			if(onClickSound != null && onClickSound.Length > 0)
				AudioManager.Instance.PlaySound(onClickSound);


			//compability workaround for touch devices
			//avoid to stuck into over state after click
			blockOver = true;

			return true;
		}//Click()

		/// <summary>
		/// Set the button state and call the delegates for pressed down button.
		/// </summary>
		public virtual bool Push()
		{
			//Log.Debug("Pressed():"+Name);
			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;
			
			//WebPlayerDebugManager.addOutput("push "+State,1);
			State = UIButtonState.PUSH;

			// if(GameManager.Instance.hidTouch == true)
			// {
			// 	if(State != UIButtonState.OVER)
			// 	{
			// 		if(eventId.Length > 0) {
			// 			GameManager.Instance.Event("SCREEN", eventId, gameObject.name);
			// 			//WebPlayerDebugManager.addOutput("click, State="+State,1);
			// 		} else if(onClick != null) {
			// 			onClick(this);
			// 			//WebPlayerDebugManager.addOutput("click, State="+State,1);
			// 			if(State != UIButtonState.DISABLED && State != UIButtonState.ACTIVE_DISABLED)
			// 				State = UIButtonState.OVER;
			// 		}
			// 	}
			// }
			// else 
			if(onPush != null)
				onPush(this);

			return true;

		}//Pressed()

		/// <summary>
		/// Set the button state and call the delegates for relese button.
		/// </summary>
		public virtual bool Release()
		{
			//Log.Debug("Release():"+this.state);

			//WebPlayerDebugManager.addOutput("release "+this.Name,1);

			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			State = UIButtonState.RELEASE;
			if(onRelease != null)
				onRelease(this);

			blockOver = false;

			return true;


		}//Release()


		/// <summary>
		/// Set the button state and call the delegates for over button.
		/// </summary>
		public virtual bool Over()
		{
			//Log.Debug("Over():"+this.state + " gameObject:" + gameObject.name);

			if(blockOver)
				return false;

			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			State = UIButtonState.OVER;
			if(onOver != null)
				onOver(this);

			return true;

		}//Over()

		/// <summary>
		/// Set the state when cursore leave the object
		/// </summary>
		public virtual bool OverExit()
		{
			//Log.Debug("Exit():"+this.state + " gameObject:" + gameObject.name);
			
			//WebPlayerDebugManager.addOutput("over exit, State="+State,1);
			if (State == UIButtonState.DISABLED || State == UIButtonState.ACTIVE_DISABLED || !Visible)
				return false;

			State = UIButtonState.RELEASE;
			if(onOverExit != null)
				onOverExit(this);

			return true;
		}//OverExit()


	}//class UIButton

}//namespace
//------------------------------------------------------------------------------

#endif //!EPIGENE_UI_46
