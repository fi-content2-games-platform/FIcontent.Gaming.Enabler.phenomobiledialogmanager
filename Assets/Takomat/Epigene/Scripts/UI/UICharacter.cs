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

using Epigene.MODEL;

namespace Epigene.UI
{
	
	/// <summary>
	/// Class to manage a Character / NPC in the game
	/// with emotional levels.
	/// Each level has it's own image associated
	/// and changes when the emotional level changes.
	/// </summary>
	public class UICharacter : UIImage
	{

		/// <summary>
		/// NPC model
		/// </summary>
		public NPC Npc
		{
			get{return npc;}
		}
		private NPC npc;

		/// <summary>
		/// Emotion of the character
		/// </summary>
		public EmotionType Emotion
		{
			get{ return npc.emotion; }
			set
			{ 
				base.Sprite = (int)value;
				//
				npc.emotion = value;
			}
		}
		
		/// <summary>
		/// relative position for bubble
		/// origo is the image center
		/// </summary>
		public Vector3 bubblePosition = new Vector3(-0.2f, 4.5f);
		/// <summary>
		/// scale of bubble
		/// </summary>
		public Vector3 bubbleScale = new Vector3(40,40,1);
		/// <summary>
		/// tail orientation of bubble
		/// </summary>
		public string bubbleTail = "SE";


		/// <summary>
		/// Ctor
		/// </summary>
		public UICharacter(GameObject obj, Sprite[] sprites)
		: base(obj, sprites)
		{

			this.type = UIType.Character;
			this.npc = NpcModels.Instance.Get(obj.name);
			Emotion = npc.emotion;

			bubblePosition = gameObject.transform.position;

		}//ctor()

		/// <summary>
		/// ctor for creating it from dictionary (from file)
		/// </summary>
		public UICharacter(Dictionary<string,object> dict)
		: base(dict)
		{
			this.type = UIType.Character;

			//Log.Info("UIChar ID:"+dict["id"].ToString());

			this.npc = NpcModels.Instance.Get(dict["id"].ToString());
			Emotion = npc.emotion;
			//bubblePosition = gameObject.transform.position;
			
			//Log.Info("NPC activated name:"+npc.name);
		}//ctor()

		public void Update()
		{
			//Emotion = npc.emotion;
			base.Sprite = (int)npc.emotion;
		}

	}//class UICharacter

}//namespace