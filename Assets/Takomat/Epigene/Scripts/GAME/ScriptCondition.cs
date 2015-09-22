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
	/// Script condition pass a value to a function delegate,
	/// which will evaulte it. If the function returns true,
	/// the condition will be true.
	/// </summary>
	public class ScriptCondition<ValueType> : ACondition
	{

		public delegate bool ScriptFunction(ValueType value);

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
		/// Gets or sets the function delegates.
		/// </summary>
		/// <value>The function.</value>
		public ScriptFunction Function
		{
			get { return function;}
			set { function = value; }
		}
		private ScriptFunction function;

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.ScriptCondition`1"/> class.
		/// </summary>
		public ScriptCondition()
		{
			function = null;
		}//ctor()

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.ScriptCondition`1"/> class.
		/// </summary>
		/// <param name="function">Function delegates.</param>
		/// <param name="value">Value to pass to function.</param>
		public ScriptCondition(ScriptFunction function, ValueType value)
		{
			this.function = function;
			this.value = value;
			
		}//ctor()

		/// <summary>
		/// Check the condition.
		/// </summary>
		public override bool Check()
		{
			//Debug.Log ("ScriptCondition value="+value);
			return function(value);

		}//Check()


	}//class ScriptCondition

}//namespace GameManager

