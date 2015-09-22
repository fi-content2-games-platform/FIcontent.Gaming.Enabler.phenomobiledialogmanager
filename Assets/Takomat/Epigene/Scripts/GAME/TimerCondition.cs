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
	//------------------------------------------------------------------------------
		/// <summary>
		/// Timer enum types
		/// </summary>
		public enum TimerType
		{
			// Absolute is seen as the checking absolute against the game time
		    // e.g. if i wanna check after 30 seconds gameTime
		    // I do it by setting time condition to 30
			Absolute = 0, 
			// Relative seen from the moment this 
		    // TimerCondition is first set. So when the set gameTime is 
		    // 11 seconds and the Condition time is 5 seconds
		    // than it is true after 16 seconds
		    // e.g. a game (elapsed) time
			Relative      
		}
		
		/// <summary>
		/// Timer condition.
		/// </summary>
		public class TimerCondition : ACondition
		{
			/// <summary>
			/// Gets or sets the time in sec.
			/// </summary>
			/// <value>The time in sec.</value>
			public double TimeInSec
			{
				get { return  time;}
				set { this.time = value; this.originalTime =  value; }
			}
			private double time;  // in seconds = 1 millisecond is 0.001
			private double originalTime; // in seconds
			private TimeSource timeSource;
			private double timeCond; // in sec.
			
			/// <summary>
			/// Gets or sets the type of this timer
			/// </summary>
			/// <value>The TimerType.</value>
			public TimerType Type
			{
				get { return type;}
				set { this.type = value;}
			}
			private TimerType type;

		    // states if TimerCondition is first checked
			private bool firstCheck = true; 
//------------------------------------------------------------------------------		
			/// <summary>
			/// Initializes a new instance 
			/// of the <see cref="GamePlay.TimerCondition"/> class.
			/// </summary>
			public TimerCondition (TimerType type, 
		                           double    _time,
		                           TimeSource _timeSource)
			{

				this.timeSource = _timeSource;
				this.time = timeSource.Time; 
				this.type = type;
				switch(type)
				{	case TimerType.Absolute:
						this.time = _time;
						break;
					default: 
						this.time += _time;
						this.timeCond = _time;
					break;
				}
			}//ctor()
			
			/// <summary>
			/// Check the time
			/// </summary>
			/// <returns>true if time expired</returns>
			public override bool Check()
			{
				bool   result = false;
				double deltaTime; 

			    // process for all TimerType type
				deltaTime = time-timeSource.Time;
				if(deltaTime<(double)0.0)
				{
					result     = true;
				}

				switch(type)
				{
					case TimerType.Relative:
						if(result)
						{
							this.time  = timeSource.Time;
							this.time += timeCond;
						}
						break;
					default: 
						Debug.LogWarning("Invalid Timer type defined!");
						break;
				}
				return result;
			}//Check()
		}//class TimerCondition
	}//namespace
//------------------------------------------------------------------------------
