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
	public abstract class ACondition
	{
		/// Delegates for conditions, used for Fire and Delete as well.
		public delegate bool FireCondition();

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GamePlay.ACondition"/> is equal.
		/// </summary>
		/// <value><c>true</c> if equal; otherwise, <c>false</c>.</value>
		bool Equal
		{
			get {return equal;}
			set {equal = value;}
		}
		protected bool equal;

		/// <summary>
		/// Gets or sets the current operator.
		/// </summary>
		/// <value>The current operator.</value>
		CompareOperator CurrentOperator
		{
			get {return currentOperator;}
			set {currentOperator = value;}
		}
		protected CompareOperator currentOperator;

		/// <summary>
		/// Check the condition.
		/// </summary>
		public abstract bool Check();


	}//class ACondition

	/// <summary>
	/// False condition.
	/// </summary>
	public class FalseCondition : ACondition
	{
		public override bool Check()
		{
			//Debug.Log ("FALSE Condition");
			return false;
		}

	}//class FalseCondition

	/// <summary>
	/// True condition.
	/// </summary>
	public class TrueCondition : ACondition
	{
		public override bool Check()
		{
			//// Log.GameTimes("TRUE Condition");
			return true;
		}
		
	}//class FalseCondition


}//namespace

