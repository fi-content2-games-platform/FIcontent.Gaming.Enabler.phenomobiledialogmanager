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
//-------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

using Epigene;
using Epigene.UI;
using Epigene.MODEL;
using Epigene.GAME;


//------------------------------------------------------------------------------
namespace TWISTBueBe
{
	
	public class InfoSettings : MonoBehaviour 
	{
		public GameObject popupSettings;

		public UIText txtVersion;

		public Button btnClose;

		void Start ()
		{
			Log.Assert(popupSettings, "Please assign the popup in "+gameObject.name);
			Log.Assert(txtVersion, "Please assign the txtVersion in "+gameObject.name);
			Log.Assert(btnClose, "Please assign the btnClose in "+gameObject.name);

			GameObject obj = null;
			
			popupSettings = GetChildObject("popup");
			obj = GetChildObject("popup/txt_Copyright_Content2");
			txtVersion = obj.GetComponent<UIText>();
			txtVersion.text = "Version: "+GameObject.Find("TWISTBueBe").GetComponent<TWISTBueBe.TWISTGame>().buildTime;
			obj = GetChildObject("popup/btn_Close");
			btnClose = obj.GetComponent<Button>();
			obj = GetChildObject("popup/btn_Close/Text");
			UIText btnCloseText = obj.GetComponent<UIText>();
			btnCloseText.Text = I18nManager.Instance.Get("POPUP_BUTTON", "003");

			Hide();

		}//Start()

		/// <summary>
		/// Enable the object will register event handler
		/// </summary>
		public void OnEnable()
		{
			GameManager.Instance.RegisterEventHandler("POPUP_GAMEINFO", EventHandler);
		}
		
		/// <summary>
		/// Disable the object will remove event handler
		/// </summary>
		public void OnDisable()
		{
			GameManager.Instance.RemoveEventHandler("POPUP_GAMEINFO", EventHandler);
		}
		
		GameObject GetChildObject(string name)
		{
			GameObject obj = GameObject.Find(gameObject.name+"/"+name);
			Log.Assert(obj, "Could not find object for:"+name);
			return obj;
			
		}//GetChildObject();
		
		public void Show()
		{
			popupSettings.SetActive(true);

			UIManager.Instance.ShowModal(popupSettings);
			
		}//Show()

		public void Hide()
		{
			popupSettings.SetActive(false);			
		}//Hide()
		
		// Left Button
		public void OnClick(string button)
		{
			Log.Info("Click: "+button);
			
			if(button == "btn_Close") 
			{
				Hide();
			}
			
		}

		public void EventHandler(string eventId, string param)
		{
			
			//Log.GameTimes("Parameter : "+param);
			if(param == "show")
			{
				Show();
			}
			if(param == "hide")
			{
				Hide();
			}
			
		}//EventHandler()
	}
} // Namespace
