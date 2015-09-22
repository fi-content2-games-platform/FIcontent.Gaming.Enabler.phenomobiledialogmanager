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
	
	/// <summary>
	/// Use this script to add custom tooltip to an object
	/// </summary>
	public class Tooltip : MonoBehaviour 
	{

		public string id;

		//public float waitTime;
		private float time;
		private Vector3 pos;

		// Use this for initialization
		void Start () 
		{
			time = 0;

			//only add collider if not yet exist
			Collider2D c = gameObject.GetComponent<Collider2D>();
			if(c == null)
			{
				gameObject.AddComponent<BoxCollider2D>();
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnMouseOver()
		{
			//check if we are visible

			if(time == 0)
			{
				time = Time.time;				

			} 
			else if(time + UIManager.Instance.Tooltip.activationTime < Time.time)//	&& !UIManager.Instance.Tooltip.active)
			{
				if (id != null)
				{
					if (UIManager.Instance.CheckTooltipVisibility(gameObject))
					{
						UIManager.Instance.Tooltip.Show(id);
					}
				}
			}
		}

		void OnMouseExit()
		{
			UIManager.Instance.Tooltip.gameObject.SetActive(false);
			time = 0;
		}
	}
}