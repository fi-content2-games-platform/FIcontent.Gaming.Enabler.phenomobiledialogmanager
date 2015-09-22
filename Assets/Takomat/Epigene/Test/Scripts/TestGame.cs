using UnityEngine;
using System.Collections;

using Epigene.GAME;

public class TestGame : MonoBehaviour
{

	private GameManager gameManager;

	/// <summary>
	/// Start this instance.
	/// </summary> 
	public void Start()
	{
		Debug.Log("Start TestGame");

		//create games
		GMGame game1 = CreateGame1();
		GMGame game2 = CreateGame2();

		//create gamemanager
		gameManager = GameManager.Instance;

		//add games to manager
		gameManager.Add(game1);
		gameManager.Add(game2);
		gameManager.Start(game1.Name);


	}//Start()

	/// <summary>
	/// Update called by every frame
	/// </summary>
	public void Update()
	{
		//update game state
		gameManager.Update();
		
	}//Update

	/// <summary>
	/// Creates Game1.
	/// </summary>
	public GMGame CreateGame1()
	{
		//create games and add triggers
		GMGame game = new GMGame("game1");

		//create conditions
		ACondition cond1 = new TestCondition("Test-Cond-1", true);
		//ACondition cond2 = new TestCondition("Test-Cond-2", true);
		ACondition falseCond = new FalseCondition();
		ACondition trueCond = new TrueCondition();
		ACondition relTimeCond = new TimerCondition(TimerType.Relative, 1, game.TimeSource);  //true after 1sec

		//create triggers
		ScriptTrigger<int> trigger1 = new ScriptTrigger<int>(
			   2, 500, 
		       trueCond.Check, null, TestScriptFunctionInt, 111);

		ScriptTrigger<int> trigger2 = new ScriptTrigger<int>();
		trigger2.Value = 222;
		trigger2.Function = TestScriptFunctionInt;
		trigger2.Priority = 3;
		trigger2.Repeat = 3;
		trigger2.FireCondition = cond1.Check;

		//this will only be fired, when both fireCondition is true at the same time
		ScriptTrigger<string> trigger3 = new ScriptTrigger<string>();
		trigger3.Value = "game2";
		trigger3.Function = TestScriptFunctionString;
		trigger3.Priority = 1;
		trigger3.Repeat = 1;							//only fire once
		trigger3.FireCondition += trueCond.Check;		//always true, but you can check sg extra here
		trigger3.FireCondition += relTimeCond.Check;	//should fire only when time reached
		trigger3.DeleteCondition = falseCond.Check;		//don't remove until repeating

		game.AddTrigger(trigger1);
		game.AddTrigger(trigger2);
		Debug.Log ("Added trigger 3");
		game.AddTrigger(trigger3);

		return game;

	}//CreateGame1()

	/// <summary>
	/// Creates Game2.
	/// </summary>
	public GMGame CreateGame2()
	{
		
		//create conditions
		ACondition falseCond = new FalseCondition();
		ACondition trueCond = new TrueCondition();
		ScriptCondition<bool> scriptCond =  new ScriptCondition<bool>(TestScriptConditionEvaulateBool, true);

		//create triggers
		ScriptTrigger<int> trigger1 = new ScriptTrigger<int>(2, 2, trueCond.Check, trueCond.Check, TestScriptFunctionInt, 1);
		ScriptTrigger<float> trigger2 = new ScriptTrigger<float>(5, 10, trueCond.Check, falseCond.Check, TestScriptFunctionFloat, 2.2f);
		ScriptTrigger<string> trigger3 = new ScriptTrigger<string>(1, 3, null, null, TestScriptFunctionString, "no conditions");
		DialogTrigger trigger4 = new DialogTrigger(1, 3, scriptCond.Check, null, "myDialog");

		//create games and add triggers
		GMGame game = new GMGame("game2");
		game.AddTrigger(trigger1);
		game.AddTrigger(trigger2);
		game.AddTrigger(trigger3);
		game.AddTrigger(trigger4);
		
		return game;
		
	}//CreateGame1()

	/// <summary>
	/// Test script processing int value.
	/// </summary>
	/// <param name="value">Value as int</param>
	public void TestScriptFunctionInt(int value)
	{
		Debug.Log (" ----> Test int value="+value);
	}//TestScriptFunctionInt()

	/// <summary>
	/// Test script processing float value.
	/// </summary>
	/// <param name="value">Value as float</param>
	public void TestScriptFunctionFloat(float value)
	{
		Debug.Log (" ----> Test float value="+value);
	}//TestScriptFunctionFloat()

	/// <summary>
	/// Test script processing string value.
	/// </summary>
	/// <param name="value">Value as string</param>
	public void TestScriptFunctionString(string value)
	{
		Debug.Log (" ----> Test string value="+value);

		//switch game based on it's name
		if(value == "game2" || value == "game2")
			gameManager.Start(value);

	}//TestScriptFunctionString()

	/// <summary>
	/// Tests the script condition.
	/// </summary>
	/// <param name="value">value to evaulate for condition</param>
	/// <returns>same as value</returns>
	public bool TestScriptConditionEvaulateBool(bool value)
	{
		Debug.Log (" ----> Test Condition value="+value);

		return value;
	}//TestScriptConditionEvaulateBool()


}//class MyGame
