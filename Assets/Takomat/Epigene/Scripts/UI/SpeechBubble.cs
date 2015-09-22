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
using System.Text;
using System;

using Epigene.UI;

using Epigene.GAME;
using Epigene.MODEL;

namespace Epigene
{

	/// <summary>
	/// SpeechBubble class to create and manage the bubble with text.
	/// </summary>
	public class SpeechBubble : MonoBehaviour 
	{
		/// <summary>
		/// id of the bubble
		/// this could change dynamically during the game
		/// and allows to reuse the same bubble for multiple dialogs
		/// Leave get/set for compatibility reason
		/// </summary>
		public string Id
		{
			get{ return id; }
			set{ id = value; }
		}
		public string id;

		//need name field
		//public bool showName;

		//direction of the tail
		public TailType tail = TailType.SW;

		//colors
		public Color bubbleColor;
		public Color outlineColor;
		public Color nameColor;
		public Color nameOutlineColor;

		/// <summary>
		/// Key for close bubble
		/// </summary>
		public KeyCode closeButton = KeyCode.Space;

		/// <summary>
		/// Handler object for buttons
		/// </summary>
		public GameObject handler;
		/// <summary>
		/// Function name to call in handler
		/// in case of click on close
		/// </summary>
		public string onClose;
		/// <summary>
		/// Function name to call in handler
		/// in case of click on arrow
		/// </summary>
		public string onArrow;

		/// <summary>
		/// Text of this bubble
		/// </summary>
		public string Text
		{
			set{ 
				if(bubbleText != null)
				{ 
					//bubbleText.wrapSize = 25;
					//bubbleText.Text = WordWrap(value, 30);//value; 
					bubbleText.Text = value;
				}	 
			}
			get{ return bubbleText.Text; }
		}
		/// <summary>
		/// Localized text id of this bubble
		/// </summary>
		public string TextId
		{
			set{ 
				if(bubbleText != null)
				{ 
					//bubbleText.wrapSize = 30;
					//bubbleText.Text = WordWrap(value, 30);//value; 
					//bubbleText.Text = value;
					bubbleText.textId = value;
				}
					 
			}
			get{ return bubbleText.textId; }
		}

		/// <summary>
		/// Name field of the bubble
		/// </summary>
		public string Name
		{
			set
			{
				nameText.Text = value;
				if(nameText.Text.Length > 0)
					nameGroup.SetActive(true);
				else
					nameGroup.SetActive(false);
			  }
			get{ return nameText.Text; }
		}

		/// <summary>
		/// Text of this bubble
		/// </summary>
		public string DebugText
		{
			set{ 
				if(debugText != null)
				{ 
					debugText.Text = value;
				}	 
			}
			get{ return debugText.Text; }
		}


		/// <summary>
		/// Tail informations
		/// </summary>
		public enum TailType {E, N, NE, NW, S, SE, SW, W};
		private string[] tailNames;
		private GameObject[] tails;

		/// <summary>
		/// Localized text part of the bubble
		/// </summary>
		private UIText uiText;

		/// <summary>
		/// The user interface manager.
		/// </summary>
		private UIManager uiManager;

		/// <summary>
		/// Player manager
		/// </summary>
		private GameManager gpm;

		/// <summary>
		/// GameObjects for sub elements
		/// </summary>
		GameObject bubbleOutline;
		GameObject bubbleObj;
		GameObject nameOutline;
		GameObject nameObj;
		GameObject nameGroup;

		/// <summary>
		/// Text in the bubble
		/// </summary>
		UIText bubbleText;
		/// <summary>
		/// text in the name field
		/// </summary>
		UIText nameText;

		/// <summary>
		/// debug info about the bubble
		/// </summary>
		UIText debugText;

		//some fake anim
		//TODO use triggers for proper animations
		private Vector3 origScale;
		private Vector3 scale;
		private int dir = 1;
		private float nextActionTime = 0.0f;
		private float period = 0.0001f;

		/// <summary>
		/// sorting order of sprites
		/// </summary>
		public int sortingOrder = 5;

		/// <summary>
		/// Create the text and set the tail on bubble
		/// </summary>
		void Awake()
		{
			//create text component
			origScale = gameObject.transform.localScale;
			
			tailNames = new string[] {"tail_E", "tail_N", "tail_NE", "tail_NW", 
										   "tail_S", "tail_SE", "tail_SW", "tail_W"};

			tails = new GameObject[8];
			for(int i = 0; i < tailNames.Length; i++)
			{
				string objname = gameObject.name + "/tails/" + tailNames[i];
			 	tails[i] = GameObject.Find(objname);
			 	Log.Assert(tails[i], "Cannot find tail:"+objname);
			}

			//save original size
			scale = origScale;

			string path = (gameObject.transform.parent != null) 
						? gameObject.transform.parent.name + "/" + gameObject.name
						: gameObject.name;
			bubbleObj 		= GameObject.Find(path+"/bubble_scale/bubble_text");			
			bubbleOutline 	= GameObject.Find(path+"/bubble_scale/bubble_outline");
			nameGroup 		= GameObject.Find(path+"/name_group");
			nameObj 		= GameObject.Find(path+"/name_group/name_text");
			nameOutline 	= GameObject.Find(path+"/name_group/name_outline");

			Log.Assert(bubbleObj, "Missing BubbleObject in "+ path);
			Log.Assert(bubbleOutline, "Missing bubbleOutline in "+ gameObject.name);
			Log.Assert(nameGroup, "Missing nameGroup in "+ gameObject.name);
			Log.Assert(nameObj, "Missing nameObj in "+ gameObject.name);
			Log.Assert(nameOutline, "Missing nameOutline in "+ gameObject.name);

			GameObject obj;
			obj = GameObject.Find(path + "/Text");
			if(obj != null)
			{
				//Log.Assert(obj, "Missing Text object in "+path);
				bubbleText = obj.GetComponent<UIText>();				
				Log.Assert(bubbleText, "Cannot find text in bubble " + obj.name);
				bubbleText.isLocalized = false;
				//bubbleText.wrapSize = 30;
			}
			else
			{
				Log.Error("OOPS1"+path+"/Text");
			}
			//grab name
			obj = GameObject.Find(path + "/name_group/Text");
			if(obj != null)
			{
				nameText = obj.GetComponent<UIText>();
				Log.Assert(nameText, "Cannot find name in bubble " + obj.name);
			}
			//grab debugText
			obj = GameObject.Find(path + "/DebugText");
			if(obj != null)
			{
				//Log.Assert(obj, "Missing Text object in "+path);
				debugText = obj.GetComponent<UIText>();				
				Log.Assert(debugText, "Cannot find text in bubble " + obj.name);
				debugText.isLocalized = false;
				//debugText.Text = "";
			}

			//get managers
			uiManager = UIManager.Instance;
			gpm = GameManager.Instance;

		}//Awake()

		/// <summary>
		/// Enable the bubble
		/// </summary>
		void OnEnable()
		{
			//TODO start open animation here
			bubbleText.isLocalized = false;
			//bubbleText.wrapSize = 30;

			SetTail(tail);


		}//OnEnable()

		/// <summary>
		/// Disable the bubble
		/// </summary>
		void OnDisable()
		{
			//TODO start close animation here
			
		}//OnEnable()

		/// <summary>
		/// Start the bubble, lets init some stuff
		/// </summary>
		void Start () 
		{

			SetTail(tail);
			SetColor();

			//if(!showName)
			if(nameText.Text.Length > 0)
				nameGroup.SetActive(true);
			else
				nameGroup.SetActive(false);
			
		}

		IEnumerator WaitSomeSeconds ()
		{
			yield return new WaitForSeconds(5);
			if(!MessagePopups.Instance.XClick) //if still false
			{
				GameManager.Instance.Event("POPUP_MESSAGE", "001", "show");
				//MessagePopups.Instance.XClick = true;
			}
		}
		
		/// <summary>
		/// Update the bubble with some anims
		/// TODO use trigger in start instead of this..
		/// </summary>
		void Update () 
		{

			//TODO quick hack.. this will be managed by gpm triggers..
			// if(scale.x > origScale.x + 10
			// 	|| scale.x < origScale.x - 10)
			//check keyboard input
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1))
			{
				MessagePopups.Instance.XClick = true;
				ButtonClose();
			}

			if(!MessagePopups.Instance.XClick)
			{
				StartCoroutine(WaitSomeSeconds());
			}
			// 	dir = -dir;

			 if(scale.x > origScale.x + 10
			 	|| scale.x < origScale.x - 10)
			 	return;
			

			if (Time.time > nextActionTime ) 
			{
				nextActionTime += period;
				
				scale.x = scale.x + 0.8f * dir;
				scale.y = scale.y + 0.8f * dir;
				//TODO fix this
				//gameObject.transform.localScale = scale;
			}



			// if(nameText.Text.Length > 0)
			// 	nameGroup.SetActive(true);
			// else
			// 	nameGroup.SetActive(false);

		}//Update()

		/// <summary>
		/// Set the tail orientation based on id
		/// </summary>
		public void SetTail(string id)
		{

			if(id == "N") tail = TailType.N;
			if(id == "NE") tail = TailType.NE;
			if(id == "E") tail = TailType.E;
			if(id == "SE") tail = TailType.SE;
			if(id == "S") tail = TailType.S;
			if(id == "SW") tail = TailType.SW;
			if(id == "W") tail = TailType.W;
			if(id == "NW") tail = TailType.NW;

			SetTail(tail);
		}

		/// <summary>
		/// Enable the selected tail
		/// </summary>
		void SetTail(TailType tail)
		{
			//Log.Debug("SetTail : "+tail );
			
			//disable all tails		
			foreach(GameObject t in tails)
			{
				t.SetActive(false);
			}

			//enable the one
			tails[(int)tail].SetActive(true);

			//TODO FIX this, because this overwrites the outline colors too
			//MeshRenderer meshRenderer = tails[(int)tail].GetComponent<MeshRenderer>();
			//meshRenderer.material.color = bubbleColor;
			
		}//SetTail()

		/// <summary>
		/// Set the color of each parts
		/// </summary>
		void SetColor()
		{
			//set colors
			MeshRenderer meshRenderer;
			meshRenderer = bubbleObj.GetComponent<MeshRenderer>();
			meshRenderer.material.color = bubbleColor;
			meshRenderer.sortingOrder = sortingOrder;

			meshRenderer = bubbleOutline.GetComponent<MeshRenderer>();
			outlineColor = new Color32 (103, 105, 107, 255);
			meshRenderer.material.color = outlineColor;
			meshRenderer.sortingOrder = sortingOrder;

			meshRenderer = nameObj.GetComponent<MeshRenderer>();
			//meshRenderer.material.color = nameColor;
			meshRenderer.material.mainTexture = 
				Resources.Load("Models/SpeechBubble/large_bubble_move_group_v01.fbm/name_gradient")
					as Texture2D;
			 
			meshRenderer.sortingOrder = sortingOrder;

			meshRenderer = nameOutline.GetComponent<MeshRenderer>();
			meshRenderer.material.color = nameOutlineColor;
			meshRenderer.sortingOrder = sortingOrder;

		}//SetColor()

		/// <summary>
		/// Helper to activate the gameobject
		/// </summary>
		public void SetActive(bool flag)
		{
			gameObject.SetActive(flag);
		}//SetActive()


		/// <summary>
		/// Function to handle the close button
		/// </summary>
		public void ButtonClose()
		{
			Log.Info("Close button was clicked");

			if(handler && onClose.Length > 0)
			{
				handler.SendMessage(onClose, this);
			}

		}//ButtonClose()

		/// <summary>
		/// Function to handle the arrow button
		/// </summary>
		public void ButtonArrow()
		{
			//Log.Info("Arrow button was clicked");		

			if(handler && onArrow.Length > 0)
			{
				handler.SendMessage(onArrow, this);
			}
		}//ButtonArrow()
	}//class SpeechBubble
}//namespace
