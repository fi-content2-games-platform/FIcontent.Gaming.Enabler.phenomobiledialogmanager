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
using Epigene.UI;
using Epigene.MODEL;
using Epigene.GAME;
using Epigene.VIEW;

namespace  TWISTBueBe
{

	public class EmailViewCreateButton : EmailView
	{
		private Emails emails;
		private GameObject inbox;
		private UIManager uiManager;
		private I18nManager i18n;
		private List<UIButton> buttons;
		private int lastNumber;
		
		//TODO add text colors for buttons
		private Color upColor = new Color32(103,105,107,255);
		private Color downColor = new Color32(103,105,107,255);
		private Color overColor = new Color32(103,105,107,255);
		private Color disabledColor = new Color32(103,105,107,255);
		private Color activeColor = new Color32(103,105,107,255);
		
		private float messageButtonPosY = 1.58f;
		

		public override void Awake () 
		{
			//Log.GameTimes ("EmailView Awake");
			base.Awake();
			
			uiManager = UIManager.Instance;
			i18n = I18nManager.Instance;
			emails = Emails.Instance;
			
			GameObject obj = null;
			
			inbox = GetChildObject("Inbox");
			obj = GetChildObject("Mail/txt_Date");
			obj = GetChildObject("Mail/txt_Subject");
			obj = GetChildObject("Mail/txt_Message");
			
			obj = GetChildObject("Mail/btn_Close_Mail/Text"); 
			UIText btnCloseText = obj.GetComponent<UIText>(); 
			btnCloseText.Text = I18nManager.Instance.Get("POPUP_BUTTON", "003");
			
			buttons = new List<UIButton>();
		}//Awake()
		//------------------------------------------------------------------------------

		
		GameObject GetChildObject(string name)
		{
			GameObject obj = GameObject.Find(gameObject.name+"/"+name);
			Log.Assert(obj, "Could not find object for:"+name);
			return obj;
			
		}//GetChildObject();

		
		/// <summary>
		/// Create text buttons for inbox's messages
		/// </summary>
		public override UIToggleButton CreateButton(int pos)
		{
			//create button with mail subject as text
			GameObject obj = GameObject.Find("btn_Email_"+pos);
			if(obj == null)
			{
				obj = new GameObject();
				obj.name = "btn_Email_"+pos;
				obj.transform.parent = inbox.transform;
				obj.transform.position = new Vector3(0, messageButtonPosY - (pos * 0.5f), 0f);
				obj.AddComponent<SpriteRenderer>();
				BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
				col.size = new Vector2(5.5f, 0.4f);
				UIToggleButton button = (UIToggleButton)uiManager.Add(UIType.ToggleButton, obj, "Sprites/Email/btn_MessageHead");
				button.Register(UIEvent.Click, OnMessageClick);
				button.State = UIButtonState.RELEASE;
				button.Active = false;
				button.SortingOrder = 201;
				
				//create UIText
				GameObject t = new GameObject();
				t.name = obj.name+"-Text";
				t.transform.parent = obj.transform;
				
				t.transform.position = 	new Vector3(obj.transform.position.x, 
				                                    obj.transform.position.y - 0.05f, 
				                                    obj.transform.position.z);
				UIText txt = t.AddComponent<UIText>();
				txt.isLocalized = false;		
				txt.SortingOrder = button.SortingOrder + 1;
				txt.FontSize = 25;
				txt.maxLength = 50;
				//TODO use color here if message new
				//txt.FontColor = newColor;
				button.SetTextColors(upColor, downColor, overColor, disabledColor, activeColor);
				button.SetText(txt);
				
				return button;
			}
			else
				return null;
		}//CreateButton()
		
	}//class EmailView
	
	
}//namespace