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

namespace Epigene
{

	public class Checkbox : MonoBehaviour 
	{
		public string resourcePath;
		public bool defaultState = false;		
		public UIText text;
	
		/// <summary>
		/// Get the uiButton object
		/// </summary>
		public UICheckbox UICheckbox
		{
			get
			{
				return uiCheckbox;
			}
		}
		private UICheckbox uiCheckbox;

		/// <summary>
		/// Name of the button
		/// </summary>
		public string Name
		{
			get{ return gameObject.name; }
			set{ gameObject.name = value; }
		}

		public bool State
		{
			get{return uiCheckbox.State;}
			set{uiCheckbox.State = value;}
		}

		protected UIType type = UIType.Checkbox;

		void Awake()
		{

		}

		void Start()
		{
			
		}

		public Checkbox()
		{
		}

		public Checkbox(string _resourcePath)
		{
			resourcePath = _resourcePath;
		}

		/// <summary>
		/// Enable this button - register it
		/// </summary>
		void OnEnable()
		{
			UIManager uiManager = UIManager.Instance;

			
			//use resourcePath only if given
			if(resourcePath.Length > 0)
			{
				Log.Info("add res"+resourcePath);
				uiCheckbox = (UICheckbox)uiManager.Add(type, gameObject, resourcePath);
			}			
			else
			{
				////check if we have sprite to use instead
				SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
				if(sr != null && sr.sprite != null)
				{
					//TODO fix localization and find a way to load the rest of the sprites..				
					uiCheckbox = (UICheckbox)uiManager.Add(type, gameObject, "", sr.sprite);
				}
				else
				{
					throw new AssertException("No resource nor sprite set in button: "+gameObject.name, gameObject);
				}
			}

			State = defaultState;

		}

		/// <summary>
		/// Disable this button (un register)
		/// </summary>
		void OnDisable()
		{
			
			//unregister
			 UIManager.Instance.Remove(gameObject.GetInstanceID());			 


		}

		/// <summary>
		/// helper to activate a button object
		/// </summary>
		public void SetActive(bool flag)
		{
			gameObject.SetActive(flag);
		}//SetActive()

	}//class Button

}//namespace