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

namespace Epigene.GAME
{
	/// <summary>
	/// TimeSource of a Game. Handles the game time
	/// The TimeSource represents real-time of the game.
	/// That are the seconds
	/// between startDate and endDate(startDate+runtime)
	/// </summary>
	public class TimeSource
	{
		// absolute startTime in seconds from worldtime(DateTime class)
		private double startTime = 0.0;
		private double gameTime  = 0.0;	// gameTime in seconds
		private bool   pause     = false;   // if the game is paused
		private double pauseTime 	= 0.0;
		private double unpauseTime	= 0.0;


//------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of TimeSource
		/// </summary>
		public TimeSource()
		{
			Stop();

		}//ctor()

		public void Stop()
		{
			gameTime  = 0.0;
			startTime = getSeconds ();
			pause     = false;
		}


		/// <summary>
		/// Gets the delta time between _time and gameTime
		/// </summary>
		/// <value>The time in sec.</value>
		public double DeltaTime(double _time)
		{
			double deltaTime = getSeconds ();
			//Debug.Log ("deltaTime : " + deltaTime);
			//Debug.Log ("startTime : " + startTime);
			//Debug.Log ("elapsedTime : " + elapsedTime);
			deltaTime -= startTime;
		    deltaTime -= gameTime;
			gameTime += deltaTime;
			return  (gameTime-_time);
		}

//------------------------------------------------------------------------------

		/// <summary>
		/// Gets the absolute gameTime in sec.
		/// </summary>
		/// <value>The time in sec.</value>
		public double Time
		{
			get { if (!pause) { 

					double deltaTime = getSeconds (); 
					double newGameTime = deltaTime - startTime;
					if(newGameTime>gameTime)
					{
						newGameTime -= gameTime;
						gameTime = newGameTime+gameTime;
					}
					//Debug.Log ("Delta Time " + deltaTime + "StartTime " + startTime);
				    //gameTime = deltaTime - gameTime - startTime;

					//Debug.Log ("Gametime " + gameTime);
				  }
				  return gameTime;
			    }
		}

		public bool Pause
		{
			get { return pause;	}
			set { 
				if (value == true)
				{
					pauseTime = 0.0;
					pauseTime = Time;
					pause = value;
				}
				else
				{	
					pause = value;
					unpauseTime = 0.0;
					unpauseTime = Time;
					
					startTime  += unpauseTime-pauseTime;
				}
			}
		}

		/// <summary>
		/// Set & Gets the startTime
		/// </summary>
		/// <value>The time in sec.</value>
		public double StartTime
		{
			get { return startTime;	}
			set { 
#if DEBUG
				  Debug.Log (" startTime set");
#endif
				  pause = false;
				  startTime = getSeconds ();}
		}
//------------------------------------------------------------------------------

		/// <summary>
		/// Starts the Game Time with "0"
		/// </summary>
		public void Start()
		{
			// Debug.Log (" startTime set");
			pause = false;
			gameTime  = 0.0;
			startTime = getSeconds ();

		}

		/// <summary>
		/// Gets the current world time in seconds
		/// </summary>
		private double getSeconds()
		{
			// One tick is 100 Nanosecond or a ten millions second
			return ((DateTime.Now.Ticks) / 10000000.0);
		}

	}//TimeSource
}//namespace
//------------------------------------------------------------------------------

			


