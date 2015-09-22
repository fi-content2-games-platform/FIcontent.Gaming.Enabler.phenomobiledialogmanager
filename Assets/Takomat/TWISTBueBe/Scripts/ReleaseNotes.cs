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


//------------------------------------------------------------------------------
using TWISTBueBe;


namespace Epigene.VIEW
{
	
	public class ReleaseNotes : MonoBehaviour {
		
		public Button btn_Dropdown;
		public GameObject selectionArea;
		public UIText textContent;
		//public GameObject selection;

		
		void Start ()
		{
			
			Log.Assert((selectionArea), "Missing selectionArea");
			selectionArea.SetActive(false); 	
			// Log.Assert((selection), "Missing selection");
			// {
			// 	selection.SetActive(false); 	
			// }
			HideAll();
			
			btn_Dropdown.text.Text = 
				I18nManager.Instance.Get("ReleaseNotes.ButtonText001", "btn");
			textContent.Text =
				I18nManager.Instance.Get("ReleaseNotes.ButtonText001", "text");
		}
		
		public void ActivateDropdown()
		{
			selectionArea.SetActive(!selectionArea.active);
		}
		
		public void SelectItem(string button)
		{
			Log.Debug("Selected: "+button);
			selectionArea.SetActive(false);
			
			if(button == "btn_Item1")
			{				
				btn_Dropdown.text.Text = 
					I18nManager.Instance.Get("ReleaseNotes.ButtonText001", "btn");
				textContent.Text =
					I18nManager.Instance.Get("ReleaseNotes.ButtonText001", "text");
			}
			else if(button == "btn_Item2")
			{
				btn_Dropdown.text.Text = 
					I18nManager.Instance.Get("ReleaseNotes.ButtonText002", "btn");
				textContent.Text =
					I18nManager.Instance.Get("ReleaseNotes.ButtonText002", "text");
			}
			else if(button == "btn_Item3")
			{
				btn_Dropdown.text.Text = 
					I18nManager.Instance.Get("ReleaseNotes.VersionHistory", "btn");

				string txt = "";
				for (int i = 100; i > 0; i--)
				{
					if (I18nManager.Instance.Contains("ReleaseNotes.VersionHistory", i.ToString("D3")))
					    txt += I18nManager.Instance.Get("ReleaseNotes.VersionHistory", i.ToString("D3")) + "\n";
					else
						WebPlayerDebugManager.addOutput("No Entry for " + i.ToString("D3"), 3);
				}
				textContent.Text = txt;
			}
			else if(button == "btn_Item4")
			{				
				HideAll();
			}
			else if (button == "btn_Item5")
			{
				HideAll();
			}
		}
		
		private void HideAll()
		{

		}
	}
} // Namespace
