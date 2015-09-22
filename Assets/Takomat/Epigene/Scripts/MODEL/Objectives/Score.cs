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
using Epigene.GAME;

//------------------------------------------------------------------------------
namespace Epigene.MODEL
{
	/// <summary>
	/// Class for manage Scores
	/// </summary>
	public class Score
	{
		//public enum ScoreType : int {NONE = -1, SOCIAL = 0, ECONOMY, ECOLOGY}
		// always take this order to MAKE life easy
		public enum ScoreType : int {SOCIAL = 0, ECONOMY, ECOLOGY };

		public enum ScoreMode : int {SELF = 0, AI};
		/// <summary>
		/// Total score value (mZ0104, mZ0108)
		/// </summary>
		public int Total
		{
			get { return social + economy + ecology; }
		}
		
		/// <summary>
		/// Social value (mZ0101, mZ0105)
		/// Counts for "ActiveCitizen"
		/// </summary>
		public int Social
		{
			get { return social;}
			set 
			{ 
				social = value;
				if(social < rangeMin) social = rangeMin;
				if(social > rangeMax) social = rangeMax;
			}
		}
		private int social;
		
		/// <summary>
		/// Economy value (mZ0102,mZ0106)
		/// </summary>
		public int Economy
		{
			get { return economy;}
			set 
			{ 
				economy = value;
				if(economy < rangeMin ) economy = rangeMin;
				if(economy > rangeMax) economy = rangeMax;
			}
		}
		private int economy;
		
		/// <summary>
		/// Ecology value (mZ0103,mZ0107)
		/// </summary>
		public int Ecology
		{
			get { return ecology;}
			set 
			{ 
				ecology = value;
				if(ecology < rangeMin ) ecology = rangeMin;
				if(ecology > rangeMax) ecology = rangeMax;
			}
		}
		private int ecology;
		
		/// <summary>
		/// Maximum score allowed
		/// </summary>
		public int rangeMax = 100;
		public int rangeMin = 0;
		
		/// <summary>
		/// Ctor for set each value
		/// </summary>
		public Score()
		{
			this.social =  0;
			this.economy = 0;
			this.ecology = 0;
		}//ctor()
		
	}//class Score
}//namespace
//------------------------------------------------------------------------------