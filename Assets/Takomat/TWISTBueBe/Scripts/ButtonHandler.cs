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
using System.Collections.Generic;

using Epigene;
using Epigene.MODEL;
using Epigene.GAME;
using Epigene.UI;
//------------------------------------------------------------------------------
namespace TWISTBueBe
{
	/// <summary>
	/// Helper class for navigation with buttons
	/// </summary>
	public class ButtonHandler : MonoBehaviour 
	{
		/// <summary>
		/// list of child objects
		/// </summary>
		List<GameObject> childrens;

		/// <summary>
		/// name of the active screen
		/// </summary>
		//private string screenName;

		/// <summary>
		/// UI Manager
		/// </summary>
		private UIManager uiManager;

		/// <summary>
		/// Initializ the buttons and register event handler
		/// </summary>
		void Awake ()
		{
			Log.Debug("Awake "+gameObject.name);

			childrens = new List<GameObject>();

			foreach (Transform child in transform)
			{
				childrens.Add(child.gameObject);
				child.gameObject.SetActive(true);
			}

			uiManager = UIManager.Instance;

			//register handler for screen change
			//UIManager.ScreenChanged = ScreenChanged;
		}//Awake()

//------------------------------------------------------------------------------
		/// <summary>
		/// Event handler to process Screen events and re-register buttons.
		/// </summary>
		public void ScreenChanged(UIScreen oldScreeen, UIScreen newScreen)
		{
			if(newScreen != null)
			{
				//reenable each, so they on OnEnable() will do the registration
				foreach(GameObject child in childrens)
				{
					child.SetActive(false);
					child.SetActive(true);
				}
			}
		}		
//------------------------------------------------------------------------------
		/// <summary>
		/// handle button click
		//TODO a lot more flexible
		/// </summary>
		public void OnClick(string name)
		{
			// Log.GameTimes(gameObject.name+" OnClick:"+name);

			// Button Event Processing
			if(name=="btn_X"      || 
			   name=="btn_Finish" || 
			   name=="btn_CityHall")
			{
				string activeScreenName = 
					((UIScreen)uiManager.ActiveScreen).name;
				//TODO move to screen
				if(activeScreenName=="CP1_7_1")
				{
					uiManager.ActivateScreen("CP1_8_4");
				} else
				{
					((UIScreen)uiManager.ActiveScreen).NextScreen();
				}
				// How to make a Button visible and invisible?

			}
			else if(name == "btn_Settings")
			{
				UIScreen screen = (UIScreen)uiManager.ActiveScreen;				
				screen.UpdateSettings();				
				//TODO back?
			}
			else if(name == "btn_Back")
			{
				((UIScreen)uiManager.ActiveScreen).BackScreen();
			}

		}//OnClick()
//------------------------------------------------------------------------------
		// ?? we need this ??
		IEnumerator ReloadGame()
		{			
			// ... pause briefly
			yield return new WaitForSeconds(2);
			// ... and then reload the level.
			//Application.LoadLevel(Application.loadedLevel);
			//Application.LoadLevel("TWISTBueBe_level2");
			Application.LoadLevel(0);

		}//ReloadGame()

	}//class ButtonHandler
}//namespace
//------------------------------------------------------------------------------