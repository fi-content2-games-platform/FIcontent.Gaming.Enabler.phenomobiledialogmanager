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
//------------------------------------------------------------------------------

using Epigene.GAME;
using Epigene.AUDIO;



namespace Epigene.UI
{
	
	/// <summary>
	/// Checkbox
	/// </summary>
	public class UICheckbox : UIImage
	{
		
		/// <summary>
		/// Current state of checkbox
		/// true if checked
		/// </summary>
		public bool State
		{
			get 
			{
				return this.state;
			}
			set 
			{
				Log.Debug("Set state:"+value);
				this.state = value;
				base.Sprite = value?1:0;				
			}			
		}
		private bool state;


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


//------------------------------------------------------------------------------

		public UICheckbox(GameObject obj, Sprite[] sprites)
			:base (obj, sprites)
		{

			if(sprites != null)
			{
#if !EPIGENE_UI_46 //TODO: remove ifdef
				this.render = obj.GetComponent<SpriteRenderer>();
				if(this.render == null)
					Log.Error("Checkbox must have a SpriteRenderer! Checkbox:"+obj.name);
#else
				this.image = obj.GetComponent<UnityEngine.UI.Image>();
				if(this.image == null)
					Log.Error("Checkbox must have an Image! Checkbox:"+obj.name);
#endif
			}

		}//ctor()

		public virtual void Reset()
		{
			lockSprite = false;
			this.State = false;
		}

		/// <summary>
		/// Constructor for create ui image from dicitionary.		
		/// </summary>
		public UICheckbox(Dictionary<string,object> dict)
			:base(dict)
		{

			this.type = UIType.Checkbox;

		}//ctor()

		// /// <summary>
		// /// Parse the content of the dictionary
		// /// and create a new gameobjet with this parameters
		// /// </summary>
		// protected override void Parse(Dictionary<string,object> dict)
		// {
		// 	//Log.Info("<color=green>PARSE!!!!!!!!!!!!!!!!!!!!!!!!!</color>");

		// 	//parse base stuff
		// 	base.Parse(dict);


		// }//Parse()

		/// <summary>
		/// Set the text of the button
		/// 
		/// </summary>
		public void SetText(UIText text)
		{
			Log.Debug("======================= SetText");
			this.text = text;
		}

	}//class 

}//namespace
//------------------------------------------------------------------------------