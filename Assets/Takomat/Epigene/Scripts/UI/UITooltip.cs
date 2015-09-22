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
	/// <summary>
	/// This class create a tooltip and show it for a few seconds
	/// </summary>
	public class UITooltip : UIText 
	{

		public GameObject bgImage;

		/// <summary>
		/// Distance from the mouse pointer
		/// </summary>		
		public float distance;
		/// <summary>
		/// size of boundary
		/// </summary>
		public float edgeSize;
		/// <summary>
		/// how long the tooltip should be alive
		/// </summary>
		public float aliveTime;
		/// <summary>
		/// how much time needed to activate
		/// </summary>
		public float activationTime;

		/// <summary>
		/// time of activation
		/// </summary>	
		private float time;

		private Vector3 position;

		private Vector3 lastPosition;

		// Use this for initialization
		void Start () 
		{
			//TODO create bg image
			
			

		}

		/// <summary>
		/// Enable the object will reset the bg image size
		/// based on current text size and will place the tooltip
		/// around the mouse position
		/// </summary>
		public override void OnEnable()
		{
			//set size of the bgImage based on current text
			base.OnEnable();
			
			//Log.Debug("TextSize:"+TextArea);

			Vector3 c = bgImage.transform.localScale;
			Vector3 s = new Vector3(TextArea.width + edgeSize*3,
			 						TextArea.height,
			 						0);

			//set the size
			bgImage.transform.localScale = s;	
			//and position to the middle of the text
			bgImage.transform.localPosition = new Vector3(0, edgeSize*2, 0);
			
			SetPosition(Input.mousePosition);

			time = Time.time;

		}

		/// <summary>
		/// Disable the object
		/// </summary>
		void OnDisable()
		{
			//base.OnDisable();
		}

		/// <summary>
		/// Update will check if time expired an can be disabled,
		/// or mouse moved away.
		/// </summary>
		public override void Update()
		{
			base.Update();

			//disable after aliveTime
			if(time + aliveTime < Time.time)
			{
				Log.Debug("Time exp");
				gameObject.SetActive(false);
			}

			if(lastPosition != Input.mousePosition)
			{
				gameObject.SetActive(false);
			}
			
			
		}


		/// <summary>
		/// Show the tooltip for a few sec with the textid
		/// </summary>
		public void Show(string id)
		{

			//disable
			gameObject.SetActive(false);

			//Text = id;
			//set localized text id
			textId = id;

			//enable will trigger the reset
			gameObject.SetActive(true);

		}

		/// <summary>
		/// force a hide
		/// </summary>
		public void Hide()
		{
			gameObject.SetActive(false);
		}


		/// <summary>
		/// Set position of the tooltip based on simple logic
		/// The position must be in screen coordinate.
		/// </summary>
		public void SetPosition(Vector3 newPosition)
		{
			float distValue = distance;
			//float w = (Size.width/gameObject.transform.localScale.x)/2;
			float w = (TextArea.width / 2) / gameObject.transform.localScale.x;
			float h = (TextArea.height / 2) / gameObject.transform.localScale.y;
			//Log.Debug("S:"+Screen.width+"x"+Screen.height);
			//Log.Debug("w:"+w+" newx:"+newPosition.x);
			//Log.Debug("h:"+h+" newy:"+newPosition.y);
			//
			//
			
			lastPosition = newPosition;

			if(newPosition.x - w < 0)
			{
				newPosition.x = w;
			}
			else if(newPosition.x + w > Screen.width)
			{
				newPosition.x = Screen.width - w;
			}
			
			if(newPosition.y - h < 0)
			{
				newPosition.y = h;
				distValue = distance;				
			}
			else if(newPosition.y + h > Screen.height)
			{
				newPosition.y = Screen.height - h;
				distValue = -distance;
				//Log.Info("newy:"+newPosition);
			}
			
			//set the position based on mouse position
			position = UIManager.UICamera.ScreenToWorldPoint(newPosition);			
			position.y += distValue;
			gameObject.transform.position = position;

			//Log.Info("pos:"+position);

		}

		
	}//class
}//namespace