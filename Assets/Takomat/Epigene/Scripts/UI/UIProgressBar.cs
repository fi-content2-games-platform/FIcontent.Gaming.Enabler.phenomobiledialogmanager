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

namespace Epigene.UI
{

	//TODO finish implementation, use UIImage for each state.

	/// <summary>
	/// Available states of a UIProgressBar
	/// </summary>
	public enum UIProgressBarState {ACTIVE = 1, DISABLED = 0};

	/// <summary>
	/// Delegate functions definition for progressBar
	/// </summary>
	public delegate void UIProgressBarFunction(UIProgressBar progressBar);

	/// <summary>
	/// ProgressBar
	/// </summary>
	public class UIProgressBar
	{

		/// <summary>
		/// TODO finalize this
		/// Current state of UIButton
		/// </summary>
		public UIProgressBarState State
		{
			get 
			{
				return this.state;
			}
			set 
			{
				//only process if state changed
				if(this.state == value)
					return;

				this.state = value;
				//TODO ? I need UIImage here
				//render.sprite = sprites[(int)value];
			}
		}
		private UIProgressBarState state;

		/// <summary>
		/// Current state of UIButton
		/// </summary>
		public int Value
		{
			get 
			{
				return this.Value;
			}
			set 
			{
				//only process if state changed
				if(this.value == value || images.Length < value)
					return;

				this.value = value;
				//render.sprite = images[(int)value];
			}
		}
		private int value;

		/// <summary>
		///  Parent GameObject which this button belongs
		/// </summary>
		public GameObject Parent
		{
			get { return obj;}

		}
		private GameObject obj;

		/// <summary>
		/// reference name of the button
		/// </summary>
		public string Name
		{
			get {return obj.name;}
		}

		/// <summary>
		/// Delegates for Click (button down)
		/// </summary>
		public UIProgressBarFunction OnUpdate
		{
			get {return onUpdate;}
			set {onUpdate = value;}
		}
		private UIProgressBarFunction onUpdate;

		/// <summary>
		/// image list of the progress bar.
		/// Every state has it's own image.
		/// </summary>
		public UIImage[] images;

		/// <summary>
		/// Backgroun image
		/// </summary>
		private UIImage bgImage;

		/// <summary>
		/// max Value
		/// </summary>
		public int Count
		{
			get { return images.Length;}
		}

		/// <summary>
		/// Render component
		/// </summary>
		private SpriteRenderer render;

		/// <summary>
		/// Ctor
		/// </summary>
		public UIProgressBar(GameObject obj, UIImage[] images, UIProgressBarFunction onUpdate)
		{
			this.obj = obj;
			this.images = images;
			this.onUpdate = onUpdate;

			this.render = obj.GetComponent<SpriteRenderer>();
			if(this.render == null)
				Debug.LogError("Button must have a SpriteRenderer! ProgressBar:"+obj.name);

		}//ctor()



	}//class UIProgressBar

}//namespace