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

using UnityEngine;
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
			Active = false;
		}//ctor()

		/// <summary>
		/// Constructor for create ui image from dicitionary.		
		/// </summary>
		public UIToggleButton(Dictionary<string,object> dict)
			:base(dict)
		{
			//this.pushed = false;
			this.type = UIType.ToggleButton;
			Active = false;

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

			
			if(State == UIButtonState.RELEASE)
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

			//compability workaround for touch devices
			//avoid to stuck into over state after click
			blockOver = true;

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

			blockOver = false;
			
			return true;


		}//Release()


		/// <summary>
		/// Set the button state and call the delegates for over button.
		/// </summary>
		public override bool Over()
		{
			//Log.Debug("Over():"+State + " gameObject:" + gameObject.name);

			if(blockOver)
				return false;

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

	}//class UIToggleButton
}//namespace

#endif //!EPIGENE_UI_46
