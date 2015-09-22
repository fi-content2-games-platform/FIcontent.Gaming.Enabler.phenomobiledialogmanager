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
using Epigene;
using Epigene.UI;
using Epigene.GAME;
//------------------------------------------------------------------------------
namespace Epigene.MODEL
{
	public sealed class H0000_Population
	{


		//Action
		public bool talk  = false;
		public bool vote  = false;
		public bool party = false;

		//TODO GameScenarioXML set by GameScenario.Load()
		public int numberOfPeople = 500;
		public int NumberOfPeople
		{
			get { return numberOfPeople; }
			set { numberOfPeople = value;
				//// Log.GameTimes("set balance ");
				
				// inform the views of the change
				string eventID = "numberOfPeople";
				string strValue = 
					numberOfPeople.ToString(
						System.Globalization.CultureInfo.InvariantCulture);
				//"111 Euro"; //TODO value.ToString();
				GameManager.Instance.Event("SCREEN", eventID, strValue);
			}
		}

		private static readonly H0000_Population instance = 
			new H0000_Population();
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static H0000_Population Instance
		{
			get{ return instance;}
		}

		// Use this for initialization
		//void Start () 
		private H0000_Population()
		{
			Debug.Log("<color=cyan>H0000_Population</color> initialized.");
					
		}//H0000_Population()



	}//F0000_Finance
}//namespace
//------------------------------------------------------------------------------