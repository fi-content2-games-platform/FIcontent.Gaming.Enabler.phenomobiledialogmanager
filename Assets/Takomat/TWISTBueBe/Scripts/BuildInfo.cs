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
using Epigene.AUDIO;

namespace TWISTBueBe
{
	public enum ConstructionType {NONE, PIPE, GREYWATERFILTER, 
		FILTER, SMALLFILTER, PIPESUBPRESSURE, BIOGASPLANT, TREATMENTPLANT};

	/// <summary>
	/// Class for managing the Build Information Panel.
	/// This panel shows the information for the currently
	/// selected construction item.
	/// </summary>
	public class BuildInfo : MonoBehaviour 
	{

		/// delegate function type for update
		public delegate void UpdateFunction();

		/// <summary>
		/// Add delegate to notify type selection.
		/// </summary>
		/// <value>The function to call.</value>
		public static UpdateFunction UpdateType
		{
			get { return updateType; }
			set { updateType += value;}
		}
		private static UpdateFunction updateType;
		

		public ConstructionType Type
		{
			get {return type;}
			set 
			{
				type = value;
				UpdateIcon();
				if(updateType != null)
					updateType();
			}
		}
		private ConstructionType type = ConstructionType.NONE;

		//resources for the mini icon		
		public GameObject iconObj;
		private string resourcePath		= "Sprites/HUD/Icons/";
		//TODO these could be more dynamic later (get from selection or Resource)
		private string filterResource	= "Icon_SmallFilterPlant_50px";
		private string pipeResource		= "Icon_PipeSystem_50px";
		private string biogasResource	= "Icon_SmallFilterPlant_50px";
		private string greywaterfilterResource	= "Icon_SmallFilterPlant_50px";

		// Use this for initialization
		void Awake () 
		{
			Log.Assert(iconObj, "Missing IconObject in "+gameObject.name);


		}//Awake()


		/// <summary>
		/// Start when activated
		/// </summary>
		void OnEnable() 
		{

        	//AudioManager.Instance.Play(AudioManager.Audio.SFX, "AUDIO.T0019");

    	}//OnEnable()

    	/// <summary>
    	/// Update the build icon
    	/// </summary>
		private void UpdateIcon()
		{
			SpriteRenderer spriteRenderer = iconObj.GetComponent<SpriteRenderer>();
			Log.Assert(spriteRenderer,"Missing sprtieRenderer component in"+iconObj.name);

			string iconResource = resourcePath;
			if(type == ConstructionType.PIPE)
			{
				iconResource += pipeResource;
			}
			else if(type == ConstructionType.SMALLFILTER)
			{
				iconResource += filterResource;
			}
			else if(type == ConstructionType.BIOGASPLANT)
			{
				iconResource += biogasResource;
			}
			else if(type == ConstructionType.GREYWATERFILTER)
			{
				iconResource += greywaterfilterResource;
			}
			
			//Log.Warning("LOAD:"+iconResource);
			Sprite[] sprites = UIManager.LoadSprite(iconResource);
			spriteRenderer.sprite = sprites[0];

		}//SetIcon()
		
	}//class BuildInfoPanel

}//namespace