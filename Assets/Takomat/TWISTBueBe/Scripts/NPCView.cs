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

using Epigene;
using Epigene.UI;
using Epigene.MODEL;

namespace TWISTBueBe
{
	/// <summary>

	/// </summary>
	public class NPCView : MonoBehaviour 
	{

		public string id;
		public NPC npc;
		public UIImage image;
		public SpeechBubble bubble;

		//NPC npc;

		// Use this for initialization
		void Start () 
		{
	
			gameObject.name = id;		
			npc = NpcModels.Instance.Get(id);
			Log.Assert(npc != null, "Missing NPC:"+id);

			
			//image = (UIImage)UIManager.Instance.Get(gameObject.GetInstanceID());
			Log.Assert(image != null, "NO uiImage found as: "+gameObject.name);

			bubble = CreateBubble(id);
		}
		
		// Update is called once per frame
		void Update () 
		{

			switch(npc.emotion)
			{
				case EmotionType.NEUTRAL:
				image.Sprite = 0;
				break;

				case EmotionType.POSITIVE:
				image.Sprite = 1;
				break;

				case EmotionType.NEGATIVE:
				image.Sprite = 2;
				break;
			}

			if(npc.IsTalking && !bubble.active)
			{
				bubble.Name = id;
				bubble.Text = npc.Talk;
				bubble.SetActive(true);
			}
			else if(!npc.IsTalking && bubble.active)
			{

				bubble.SetActive(false);
			}
		
		}

		SpeechBubble CreateBubble(string npcId)
		{
			GameObject obj = MainGame.CreateGameObject(MainGame.hudPrefabs +"SpeechBubble");
			obj.name = "bubble-"+npcId;
			obj.transform.parent = gameObject.transform;
			SpeechBubble bubble = obj.GetComponent<SpeechBubble>();
			Log.Assert(bubble != null, "Can't find SpeecBubble in gameobject:"+obj.name);

			return bubble;
			
		}//CreateBubble()

	}//class NPCView

}//namespace