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
//using System;

namespace Epigene
{

	///TODO
	/// - add log file with capturing exception
	/// - improve it further, so every object log level could be set (register logger by object)

	/// <summary>
	/// Exception for Assert.
	/// This will be thrown by the Assert() if the condition fails.
	/// This class use directly the UnityEngine.Debug functions to log
	/// so it will always work, even when the Log.Level is set to NONE.
	/// </summary>
	public class AssertException : System.Exception
	{
	    public AssertException()
	    {
	        // Add implementation.
	        Debug.LogError(Time.time.ToString("0000.00")+": <color=red> ASSERT </color>");
	    }
	    public AssertException(string message)
	    {
	        // Add implementation.
	        Debug.LogError(Time.time.ToString("0000.00")+": <color=red> ASSERT: " + message + "</color>");
	    }
	    public AssertException(string message, System.Exception inner)
	    {
	        Debug.LogError(Time.time.ToString("0000.00")+": <color=red> ASSERT: " + message + "</color>");
	    }
	    public AssertException(string message, GameObject obj)
	    {
	    	string from = (obj != null) ? " in " +obj.name : "";
	        Debug.LogError(Time.time.ToString("0000.00")+": <color=red>" + message + from + "</color>");
	    }

	    // This constructor is needed for serialization.
	   // protected AssertException(SerializationInfo info, StreamingContext context)
	   // {
	   //      // Add implementation.
	   // }

	}//class AssertException

	/// <summary>
	/// Helper class to be used to loggin and simple error handling.
	/// Use the Level property to set which level of log should be printed.
	/// Anything equals or lower level will be printed.
	/// Use Level=LogLevel.All to get debug messages 
	/// or Level=LogLevel.NONE to get no message at all.
	/// By default the level is set to ALL, so 
	/// ALL message will be printed.	
	/// </summary>
	public sealed class Log
	{

		public enum LogLevel {ALL = 0, INFO, WARNING, ERROR, GAMETIMES, SCORE, NONE};
		
		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static Log Instance
		{
			get { return instance; }
		}
		private static readonly Log instance = new Log();

		public static LogLevel Level
		{
			get { return level;}
			set { level =  value;}
		}
		private static LogLevel level = LogLevel.ALL;

		/// <summary>
		/// ctor set the logger default level to ERROR
		/// </summary>
		private Log()
		{			
			//Debug.Log("<color=cyan>Logger initialized.</color>");			
			Debug(Time.time.ToString("0000.00")+": Logger initialized");
			
		}//ctor

		/// <summary>
		/// Check a condition and log an error message if value is false
		/// </summary>
		public static void Assert(bool condition, string message = "", GameObject obj = null)
		{
			if(!condition)
				throw new AssertException(message, obj);

		}//Assert()


		/// <summary>
		/// Log a message as info
		/// This message will be logged only if the
		/// logger level is == ALL
		/// </summary>
		public static void Debug(string message, GameObject obj = null)
		{
			if( level == LogLevel.ALL)
			{
				string from = (obj != null) ? " in " +obj.name : "";
	        	UnityEngine.Debug.Log(Time.time.ToString("0000.00")+": <color=magenta>" + message + from + "</color>");
				
			}
			
		}//Debug()

		/// <summary>
		/// Log a message as info
		/// This message will be logged only if the
		/// logger level is >= INFO
		/// </summary>
		public static void Info(string message, GameObject obj = null)
		{
			if( level <= LogLevel.INFO)
			{
				string from = (obj != null) ? " in " +obj.name : "";
	        	UnityEngine.Debug.Log(Time.time.ToString("0000.00")+": <color=cyan>" + message + from + "</color>");
				
			}
			
		}//Info()


		/// <summary>
		/// Log a message as info
		/// This message will be logged only if the
		/// logger level is >= INFO
		/// </summary>
		public static void GameTimes(string message, GameObject obj = null)
		{
			if( level <= LogLevel.GAMETIMES)
			{
				string from = (obj != null) ? " in " +obj.name : "";
				UnityEngine.Debug.Log(Time.time.ToString("0000.00")+": <color=cyan>" + message + from + "</color>");
				
			}
			
		}//Info()


		/// <summary>
		/// Log a message as info
		/// This message will be logged only if the
		/// logger level is >= INFO
		/// </summary>
		public static void Score(string message, GameObject obj = null)
		{
			if( level <= LogLevel.SCORE)
			{
				string from = (obj != null) ? " in " +obj.name : "";
				UnityEngine.Debug.Log(Time.time.ToString("0000.00")+": <color=cyan>" + message + from + "</color>");
				
			}
			
		}//Info()


		/// <summary>
		/// Log a message as Warning 
		/// This message will be logged only if the
		/// logger level is => WARNING
		/// </summary>
		public static void Warning(string message, GameObject obj = null)
		{
			if( level <= LogLevel.WARNING)
			{
				string from = (obj != null) ? " in " +obj.name : "";
	        	UnityEngine.Debug.LogWarning(Time.time.ToString("0000.00")+": <color=yellow>" + message + from + "</color>");
			}
			
		}//Warning()

		/// <summary>
		/// Log a message as error
		/// This message will be logged only if the
		/// logger level is >= INFO
		/// </summary>
		public static void Error(string message, GameObject obj = null)
		{
			if( level <= LogLevel.ERROR)
			{
				string from = (obj != null) ? " in " +obj.name : "";
	        	UnityEngine.Debug.LogError(Time.time.ToString("0000.00")+": <color=red>" + message + from + "</color>");
			}
			
		}//Error()

		/// <summary>
		/// Log a message as error
		/// This message will be logged only if the
		/// logger level is >= INFO
		/// </summary>
		public static void Exception(string message, GameObject obj = null)
		{
			string from = (obj != null) ? " in " +obj.name : "";
        	UnityEngine.Debug.LogException(new System.Exception(message +" from "+ from));
			
		}//Error()

	}//class Log	
}//namespace
