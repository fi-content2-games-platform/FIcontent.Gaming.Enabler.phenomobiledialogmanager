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
using System;
using System.Collections;
using System.Collections.Generic;

using Epigene.MODEL;



namespace Epigene.UI
{
	
	/// <summary>
	/// type of dialog
	/// </summary>
	public enum DialogType {BUBBLE, MULTICHOICE};

		/// <summary>
	/// emotional levels of the character
	/// </summary>
	//public enum EmotionType {NEUTRAL, POSITIVE, NEGATIVE};

	/// <summary>
	/// Dialog class represents the model.
	/// It holds all data in one place, which can be
	/// visualized by the DialogView
	/// </summary>
	public class UIDialog
	{

		/// <summary>
		/// Gets or sets the id of the dialog
		/// </summary>
		/// <value>The value.</value>
		public string Id
		{
			get { return storyId+"."+id;}
		}
		private string id;

		/// <summary>
		/// type of the dialog
		/// bubble, multichocie
		/// </summary>
		public DialogType Type
		{
			get { return type; }
		}
		private DialogType type;

		/// <summary>
		/// screen id where this bubble is valid
		/// requried
		/// </summary>
		public string StoryId
		{
			get {return storyId;}
		}
		private string storyId;

		/// <summary>
		/// Gets or sets the position of the dialog
		/// </summary>
		/// <value>The value.</value>
		public Vector3 Position
		{
			get { return position;}
			set { position = value;}
		}
		private Vector3 position;

		/// <summary>
		/// Gets or sets the position of the npc
		/// 
		/// </summary>
		/// <value>The value.</value>
		public Vector3 NpcPosition
		{
			get { return npcPosition;}
			set { npcPosition = value;}
		}
		private Vector3 npcPosition;

		/// <summary>
		/// Image of the character who is speaking.
		/// If no image specified, no image will be shown.
		/// This value must be the same as 
		/// corresponding UICharacter's name.
		/// optional		
		/// </summary>
		public string CharacterImage 
		{	
			get { return characterImage; }
		}
		private string characterImage;

		/// <summary>
		/// Name of the character.
		/// This value will be shown in the text 
		/// field of the bubble.
		/// optional		
		/// </summary>
		public string CharacterName
		{
			get 
			{ 
				return (characterName.Length > 0) 
						? I18nManager.Instance.Get("NPC."+characterName) 
						: characterName; 
			}
			set { characterName = value; }
		}
		private string characterName;


		public bool CharacterVisible
		{
			get 
			{ 
				return characterVisible;
			}
			set { characterVisible = value; }
		}
		private bool characterVisible;



		/// <summary>
		/// list of texts for each emotional levels
		/// min. required to have neutral field
		/// </summary>
		private Dictionary<string, string> texts;

		/// <summary>
		/// Get the localized text based on emotion
		/// </summary>
		public string Text
		{
			get{ return GetText(emotion); }
		}

		/// <summary>
		/// level of emotion
		/// </summary>
		public EmotionType Emotion 
		{
			get{ return emotion; }
			set{ emotion = value; }
		}
		private EmotionType emotion;

		/// <summary>
		/// Ctor
		/// </summary>
		public UIDialog(string id, 
		                DialogType type, 
		                string storyId, 
		                string characterImage, 
		                string characterName, 
		                bool characterVisible, 
		                Dictionary<string, string> texts,
						Vector3 position,
						Vector3 npcPosition)
		{
			this.id = id;
			this.type = type;
			this.storyId = storyId;
			this.characterImage = characterImage;
			this.characterName = characterName;
			this.characterVisible = characterVisible;
			this.texts = texts;
			this.emotion = EmotionType.NEUTRAL; //"neutral";
			this.position = position;
			this.npcPosition = npcPosition;

		}//ctor()

		/// <summary>
		/// Get localized text by paramter.
		/// </summary>
		//public string GetText(string emo)
		public string GetText(EmotionType emo)
		{
			
			if(!texts.ContainsKey(GetEmotionName(emo)))
			{
				throw new Exception("Missing text for emotion:"+GetEmotionName(emo)+" in UIDialog:"+id);
			}
			
			string txt = I18nManager.Instance.Get(storyId+"."+texts[GetEmotionName(emo)]);
			//auto strip for buttons
			if(type == DialogType.MULTICHOICE)
			{
				//remove new line
				txt = txt.Replace('\n', ' ');
			}			

			return txt;
		}//GetText()

		/// <summary>
		/// Get string name of the emotion type
		/// </summary>
		public static string GetEmotionName(EmotionType type)
		{
			if(type == EmotionType.NEUTRAL)
					return "neutral";
			if(type == EmotionType.POSITIVE)
					return "positive";
			if(type == EmotionType.NEGATIVE)
					return "negative";

			throw new System.ArgumentException("Invalid EmotionType: "+type);

		}//GetEmotionName()

		/// <summary>
		/// Get the emotion type from a string
		/// </summary>
		public static EmotionType GetEmotionType(string name)
		{
			if(name == "neutral")
				return EmotionType.NEUTRAL;
			if(name == "positive")
				return EmotionType.POSITIVE;
			if(name == "negative")
				return EmotionType.NEGATIVE;

			throw new System.ArgumentException("Invalid emotion: "+name);

		}//GetEmotionType()

	}//class Dialog

}//namespace