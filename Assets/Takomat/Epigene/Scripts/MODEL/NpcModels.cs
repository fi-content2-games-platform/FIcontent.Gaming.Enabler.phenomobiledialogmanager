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

using Epigene;
using Epigene.UI;

namespace Epigene.MODEL
{

	/// <summary>
	/// emotional levels of the character
	/// </summary>
	public enum EmotionType {NEUTRAL, POSITIVE, NEGATIVE};

	/// <summary>
	/// Mail data
	/// </summary>
	public class NPC
	{
		/// <summary>
		/// id of the NPC
		/// </summary>
		public string id;

		/// <summary>
		/// name of the npc
		/// </summary>
		public string name;

		/// <summary>
		/// Set text to talk
		/// </summary>
		public string Talk
		{
			set { text = value;}
			get { return text; }
		}

		/// <summary>
		/// helper to check if the npc is talking or not
		/// </summary>
		public bool IsTalking
		{
			get{ return (text.Length > 0); }
		}
		
		/// <summary>
		/// if npc currently talking,
		/// this value will be set to
		/// the text.
		/// </summary>
		private string text;

		/// <summary>
		/// emotion of the npc
		/// </summary>
		public EmotionType emotion;

		/// <summary>
		/// Default ctor
		/// </summary>
		public NPC(string id)
		{
			this.id = id;
			this.name = I18nManager.Instance.Get("NPC."+id);
			this.emotion = EmotionType.NEUTRAL;
		}//ctor()


	}//class NPC

	/// <summary>
	/// Model for manage enpcs in the game.
	/// </summary>
	public sealed class NpcModels
	{
		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static NpcModels Instance
		{
			get { return instance; }
		}
		private static readonly NpcModels instance = new NpcModels();

		/// <summary>
		/// Get list of npcs.
		/// We use List instead of Dictionary
		/// because we want to keep an order
		/// </summary>
		public List<NPC> Npcs
		{
			get{ return npcs;}
		}
		private List<NPC> npcs;

		/// <summary>
		/// default ctor
		/// </summary>
		private NpcModels()
		{
			this.npcs = new List<NPC>();
		}//ctor()

		/// <summary>
		/// Add a npc
		/// </summary>
		public NPC Add(string id)
		{
			return Add(new NPC(id));
		}

		/// <summary>
		/// Add a npc
		/// </summary>
		public NPC Add(NPC npc)
		{
			foreach(NPC n in npcs)
			{
				//avoid to add the same npc again
				if(npc.id == n.id)
					return n;
			}			
			npcs.Add(npc);

			return npc;

		}//Add()

		/// <summary>
		/// Get one npc by subject
		/// </summary>
		public NPC Get(string id)
		{
			foreach(NPC n in npcs)
			{
				if(n.id == id)
					return n;
			}

			//add new npc with id
			NPC npc = new NPC(id);
			npcs.Add(npc);
			return npc;

			//throw new System.Exception("No NPC exist with id:"+id);
			

		}//Get()

	}//class NpcModels

}//namespace
