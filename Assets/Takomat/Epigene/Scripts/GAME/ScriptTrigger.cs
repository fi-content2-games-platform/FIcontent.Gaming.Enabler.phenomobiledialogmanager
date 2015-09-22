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

/*
	 TODO 
	 - infinite repeat = -1 or use deleteCondition istead?
	 - do we need entity? I only see it usefull in the xml file, but not here

*/
	/// <summary>
	/// Script trigger with Generic value type.
	/// ScriptTrigger calls ScriptFunction(ValueType value) delegates,
	/// if all fireConditions are true.
	/// </summary>
	public class ScriptTrigger<ValueType> : ATrigger
	{
		///Delegate function definition
		public delegate void ScriptFunction(ValueType value);

		/// <summary>
		/// Gets or sets the generic value.
		/// </summary>
		/// <value>The value.</value>
		public ValueType Value
		{
			get { return value;}
			set { this.value = value; }
		}
		private ValueType value;


		/// <summary>
		/// Gets or sets the function(s) delegates
		/// </summary>
		/// <value>The function.</value>
		public ScriptFunction Function
		{
			get { return function;}
			set { function = value; }
		}
		private ScriptFunction function;

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.ScriptTrigger"/> class.
		/// </summary>
		public ScriptTrigger()
		{
			Clear();

		}//ctor()

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.ScriptTrigger`1"/> class.
		/// </summary>
		/// <param name="function">Function to trigger</param>
		/// <param name="fireCondition">delegate conditions to check if function needs to be triggered</param>
		/// <param name="deleteCondition">Delete conditions to check if trigger can be removed</param>
		/// <param name="value">generic value to pass to function</param>
		/// <param name="priority">priority of this trigger</param>
		/// <param name="numberOfRepeat">Number of times to repeat</param>
		public ScriptTrigger(int priority, int numberOfRepeat, 
		                     ACondition.FireCondition fireCondition, 
		                     ACondition.FireCondition deleteCondition, 
		                     ScriptFunction function, ValueType value)
		{

			Clear ();
			this.type = TriggerType.SCRIPT;
			this.priority = priority;
			this.repeat = numberOfRepeat;
			this.fireCondition = fireCondition;
			this.deleteCondition = deleteCondition;
			this.function = function;
			this.value = value;

		}//ctor()

		/// <summary>
		/// Reset this trigger to defaults.
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			function  = null;
		}

		/// <summary>
		/// Fire a trigger. 
		/// Result false if condition is false and trigger won't be activated.
		/// </summary>
		/// <returns>true if function was fired by this trigger</returns>
		public override bool Fire ()
		{
			//call function(s) if Fire allowed
			if(base.Fire() && function != null)
			{
				function(value);
				return true;
			}

			return false;
		}//Fire()


	}//class ScriptTrigger

}//namespace GamePlayManger

