//------------------------------------------------------------------------------
// test class for conditions
//------------------------------------------------------------------------------
using UnityEngine;
using System;

namespace Epigene.GAME
{
	public class TestCondition : ACondition
	{

		public string Name
		{
			get { return name;}
		}
		private string name;
		
		private bool value;

		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlay.TestCondition"/> class.
		/// </summary>
		/// <param name="conditionName">Name of the condition.</param>
		/// <param name="conditionValue">If set to <c>true</c> condition value.</param>
		public TestCondition (string conditionName, bool conditionValue)
		{
			name = conditionName;
			value = conditionValue;

		}//ctor()

		/// <summary>
		/// Check this condition
		/// </summary>
		public override bool Check()
		{
			//Debug.Log ("Check "+name+":"+value);
			return value;

		}//Check()


	}//class TestCondition
}//namespace

