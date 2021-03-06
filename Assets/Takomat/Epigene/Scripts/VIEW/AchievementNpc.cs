﻿//------------------------------------------------------------------------------
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

using Epigene;
using Epigene.MODEL;
using Epigene.UI;

namespace Epigene.VIEW
{
	/// <summary>
	/// View of an achievement
	/// </summary>
	public class AchievementNpc : MonoBehaviour 
	{

		/// <summary>
		/// Npc for this slot
		/// </summary>
		public Achievement.Character Npc
		{
			set
			{	
				if(npc == null || image == null)
					return;
				image.Sprite = (int)value;
				npc = value;
			}
			get{ return npc; }
		}
		private Achievement.Character npc;
		

		/// <summary>
		/// status of check
		/// </summary>
		public bool Check
		{
			set
			{ 
				if(checkObj == null)
					return;
				if(value)
					checkObj.SetActive(true);
				else
					checkObj.SetActive(false);
			}
			get { return check;}
		}
		private bool check;

		/// <summary>
		/// check if this npc is active
		/// </summary>		
		public bool Active
		{
			get { return jointObj.active; }
		}


		/// <summary>
		/// check sing objecet
		/// </summary>
		private GameObject checkObj;
		/// <summary>
		/// object of the npc
		/// </summary>
		private GameObject npcObj;
		/// <summary>
		/// object of joint sign
		/// </summary>
		private GameObject jointObj;
		/// <summary>
		/// sprite renderer for npc
		/// </summary>
		//private SpriteRenderer renderer;
		private UIImage image;
		/// <summary>
		/// filename for the npc's sprites
		/// </summary>
		private string spriteFile = 
			"Sprites/Achievement/badge_Achievements";

		/// <summary>
		/// Set up base components
		/// </summary>
		void Awake()		
		{

			//get all childs
			foreach (Transform child in transform)
   			{
   				if(child.gameObject.name == "check")
   					checkObj = child.gameObject;
   				else if( child.gameObject.name == "npc")
   				{
   					npcObj = child.gameObject;
   					image = new UIImage(npcObj, UIManager.LoadSprite(spriteFile));

   				}
   				else if( child.gameObject.name == "symbol_Joint")
   					jointObj = child.gameObject;
   			}

			Log.Assert(checkObj, "Missing check in "+gameObject.name);
			Log.Assert(npcObj, "Missing npc in "+gameObject.name);
			//Log.Assert(image, "Missing image in "+gameObject.name);
			Log.Assert(jointObj, "Missing joint in "+gameObject.name);

			Check = false;
			image.Sprite = 0;

		}//Awake()

		/// <summary>
		/// helper for activate gameObject
		/// </summary>
		public void Show(bool flag)
		{
			// Log.GameTimes("AchShow : "+gameObject.name+" activate joint: "+flag);
			gameObject.SetActive(flag);
			//hide the joint when hiding/closeing the item
			//if(!flag)
			SetActive(false);

		}//SetActive()

		/// <summary>
		/// set the joint to active
		/// </summary>
		public void SetActive(bool flag)
		{
			// Log.GameTimes("AchNPC : "+gameObject.name+" activate joint: "+flag);
			if(jointObj!=null)
				jointObj.SetActive(flag);
		}//SetActive()
	}//class AchievementNpc

}//namespace 