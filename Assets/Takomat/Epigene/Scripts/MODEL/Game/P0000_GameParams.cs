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
using Epigene.GAME;
//------------------------------------------------------------------------------
using Epigene.IO;

namespace Epigene.MODEL
{

	/// <summary>
	/// This class holds all the general parameters of a game.
	/// 
	/// </summary>
	public sealed class P0000_GameParams
	{
		/*
		private static var _ONE_YEAR:Number = (_p0003_MIN_PER_YEAR * 60 * 1000)/2;
		private static var _ONE_DAY:Number = _ONE_YEAR/(12*30);
		
		public var simulationSpeed:Number = 1;
		
		private var 		_p0001_playgroundSize:Number 		= 0;
		private var 		_p0002_roundInMonths:Number 		= 1;
		private static var 	_p0003_MIN_PER_YEAR:Number 			= 3;
		public var 			p0006_moneyInMio:Number 			= 0;
		*/

		GameConfiguration gameConfiguration 			= 	GameConfiguration.Instance;

		private static double minutesPerYear	= 0.5;			//SIM
		private static double minutesPerWeek	= 1.0;			//CP
		private static double oneYearInMilliseconds;
		private static double P0006_balance;
		
		public double simulationSpeed;					

//------------------------------------------------------------------------------
		//Action

		private static readonly P0000_GameParams instance = new P0000_GameParams();
		             
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static P0000_GameParams Instance
		{
			get{ return instance;}
		}

//------------------------------------------------------------------------------
		/// <summary>
		/// Balance value in Euro
		/// </summary>

		public double P0006_Balance
		{
			get { return P0006_balance; }
			set { P0006_balance = value;
				  //// Log.GameTimes("set balance ");

				  // inform the views of the change
				  string eventID  = "P0006_balance";
				  string strValue = P0006_balance.ToString("N2");
				  GameManager.Instance.Event("SCREEN", eventID, strValue);
			}
		}

		public double MinutesPerYear
		{
			get { return minutesPerYear; }
			set { minutesPerYear = value; }
		}

		public double MinutesPerWeek
		{
			get { return minutesPerWeek; }
			set { minutesPerWeek = value; }
		}
		
		// Use this for initialization
		//void Start () 
		private P0000_GameParams()
		{
			Debug.Log("<color=cyan>P0000_GameParams</color> initialized.");
			Reset();
		}//P0000_GameParams()

		public void Reset()
		{
			Debug.Log("<color=cyan>P0000_GameParams</color> initialized.");
			if (gameConfiguration.HasKey("P0006_balance"))
				P0006_balance = double.Parse(
					gameConfiguration.GetValue("P0006_balance"));
			if (gameConfiguration.HasKey("simulationSpeed"))
				simulationSpeed = double.Parse(
					gameConfiguration.GetValue("simulationSpeed"));
			if (gameConfiguration.HasKey("minutesPerYear"))
				minutesPerYear = double.Parse(
					gameConfiguration.GetValue("minutesPerYear"));	//SIM
			if (gameConfiguration.HasKey("minutesPerWeek"))
				minutesPerWeek = double.Parse(
					gameConfiguration.GetValue("minutesPerWeek"));  //CP
			oneYearInMilliseconds	=	minutesPerYear * 60 * 1000;
		}
	}//P0000_GameParams
}//namespace
//------------------------------------------------------------------------------
