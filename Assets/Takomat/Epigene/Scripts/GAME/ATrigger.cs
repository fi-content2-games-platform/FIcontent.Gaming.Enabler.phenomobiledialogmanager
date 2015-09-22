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
using UnityEngine; //?
using System;
//------------------------------------------------------------------------------
namespace Epigene.GAME
{

	/// <summary>
	/// list of available types of trigger
	/// </summary>
	public enum TriggerType {SCRIPT, DIALOG};
	
	/// <summary>
	/// Abstract trigger.
	/// </summary>
	public abstract class ATrigger
	{

		/// <summary>
		/// Type of trigger
		/// </summary>
		public TriggerType Type
		{
			get{ return type;}
		}
		protected TriggerType type;

		/// <summary>
		/// Priority of this trigger.
		/// Higher values are the winner.
		/// </summary>
		public int Priority
		{
			get {return priority;}
			set {priority = value;}
		}
		protected int priority;
		
		/// <summary>
		/// How many times it should repeated
		/// </summary>
		public int Repeat
		{
			get {return repeat;}
			set {repeat = value;}
		}
		protected int repeat;

		/// <summary>
		/// Delegate function for compare.
		/// </summary>
		CompareOperator CurrentOperator
		{
			get {return currentOperator;}
			set {currentOperator = value;}
		}
		protected CompareOperator currentOperator;

		/// <summary>
		/// Condition to evaulate to determine if this trigger
		/// can be fired.
		/// </summary>
		public ACondition.FireCondition FireCondition
		{
			get { return fireCondition;}
			set { fireCondition = value;}
		}
		protected ACondition.FireCondition fireCondition;

		/// <summary>
		/// Condition to evaulate to determine is this 
		/// trigger can be removed.
		/// </summary>
		public ACondition.FireCondition DeleteCondition
		{
			get { return deleteCondition;}
			set { deleteCondition = value;}
		}
		protected ACondition.FireCondition deleteCondition;

		/// <summary>
		/// Reset this trigger to defaults.
		/// </summary>
		public virtual void Clear()
		{
			//Debug.Log("clear ATrigger");
			priority  = 0;
			repeat = 0;
			//use default false condition if not explicitly set
			//ScriptCondition.FireCondition falseCondition = new ScriptCondition.FireCondition(FalseCondition);
			fireCondition = null; //falseCondition;
			deleteCondition = null; //falseCondition;
			currentOperator = CompareOperator.Equal;

		}//Clear()

		/// <summary>
		/// Fire a trigger. 
		/// Result false if condition is false and trigger won't be activated.
		/// </summary>
		/// <returns>true if function was fired by this trigger</returns>
		public virtual bool Fire()
		{
			//Infinite Loo
			if(repeat == -1 )
			{
				//fire if no fireCondition to block it
				bool fire = true;
				
				//all fireConditions must be true to continue
				if(fireCondition != null)
					foreach( ACondition.FireCondition condition in fireCondition.GetInvocationList())
						if(!condition())
					{
						fire = false;
						break; //stop at first false
					}
				
				//fire the function if all fireConditions are TRUE
				if(fire)
				{
					//update repeat
					if( repeat != -1)
					{
						return false;
					}
				}
				
				//if all deleteCondition is true, trigger can be removed.
				if(deleteCondition != null)
				{
					bool del = true;
					foreach( ACondition.FireCondition condition in deleteCondition.GetInvocationList())
						if(!condition())
							del = false;
					if(del) 
						repeat = 0;
				}
				
				return fire;
			} 

			//only fire if still repeating
			else if(repeat > 0 )
			{
				//fire if no fireCondition to block it
				bool fire = true;

				//all fireConditions must be true to continue
				if(fireCondition != null)
					foreach( ACondition.FireCondition condition in fireCondition.GetInvocationList())
						if(!condition())
						{
							fire = false;
							break; //stop at first false
						}

				//fire the function if all fireConditions are TRUE
				if(fire)
				{
					//update repeat
					if( repeat > 0)
						--repeat;
				}

				//if all deleteCondition is true, trigger can be removed.
				if(deleteCondition != null)
				{
					bool del = true;
					foreach( ACondition.FireCondition condition in deleteCondition.GetInvocationList())
							if(!condition())
								del = false;
					if(del) 
						repeat = 0;
				}

				return fire;
			} 
			
			return false;

		}//Fire()
//------------------------------------------------------------------------------
		/// <summary>
		/// Fires the delete operation of the trigger. This function will check if the trigger can be removed.
		/// Dead trigger won't be fired anymore, but use this function to remove from any registred list.
		/// </summary>
		/// <returns><c>true</c>if delete is allowed <c>false</c> otherwise.</returns>
		public virtual bool FireDelete()
		{
			if((repeat <= 0 && repeat != -1)  || (deleteCondition != null && deleteCondition()) )
			{
				repeat = 0;
				return true;
			}

			return false;
		}//FireDelete()


		/// <summary>
		/// Helper to connect or remove event handler
		/// </summary>
		public virtual void ConnectEventHandler(bool flag)
		{
		 	Log.Debug("Override this!");

		// 	// if(flag)
		// 	// 	GameManager.Instance.AddEventHandler(EventHandler);
		// 	// else
		// 	// 	GameManager.Instance.RemoveEventHandler(EventHandler);

		}


	}//class ATrigger

}//namespace GameManager
//------------------------------------------------------------------------------

