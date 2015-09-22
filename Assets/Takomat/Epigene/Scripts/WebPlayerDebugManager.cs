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
using System.Collections.Generic;

//------------------------------------------------------------------------------
//namespace TWISTBueBe
namespace Epigene
{
	public class WebPlayerDebugManager : MonoBehaviour
	{		
		public Vector2 scrollPosition;
		
		protected string labelShowHide = "show";
		protected string activeButton = "All";
		public static string longString;

		protected static List<string> debugOutput = new List<string>();
		
		protected static bool updateGUI = false;
		protected static bool showDebug = false;

		protected string 	highscore = "18025";
		protected string 	title 	  = "Win";
		protected Epigene.MODEL.Z0000_GameObjectives 	mZ0000_GameObjectives;
		protected Epigene.MODEL.Achievements 			achievements;
		protected Texture2D 							texture;
		protected GameObject 							obj;

		public WebPlayerDebugManager()
		{
		}

		void Awake()
		{
		}

		public static void addOutput(string debugging, int from) {
			switch(from) {
				case 0:
					debugOutput.Add ("Info- "+Time.time.ToString("0000.00")+": <color=cyan>"+debugging+"</color>");
					break;
				case 1:
					debugOutput.Add ("GameTimes- "+Time.time.ToString("0000.00")+": <color=cyan>"+debugging+"</color>");
					break;
				case 2:
					debugOutput.Add ("Warning- "+Time.time.ToString("0000.00")+": <color=yellow>"+debugging+"</color>");
					break;
				case 3:
					debugOutput.Add ("Error- "+Time.time.ToString("0000.00")+": <color=red>"+debugging+"</color>");
					break;
				case 4:
					debugOutput.Add ("Exception- "+Time.time.ToString("0000.00")+": <color=orange>"+debugging+"</color>");
					break;
				default:
					break;
			}
			updateGUI = true;
		}

		protected void allButton() {
			longString = "";
			foreach (string item in debugOutput) {
				longString += item + "\n";
			}
			activeButton = "All";
		}

		protected void infoButton() {
			longString = "";
			foreach (string item in debugOutput) {
				if (item.Contains ("Info")) {
					if (longString == "")
						longString = item + "\n";
					else
						longString += item + "\n";
				}
			}
			activeButton = "Info";
		}

		protected void gametimesButton() {
			longString = "";
			foreach (string item in debugOutput) {
				if (item.Contains("GameTimes")) {
					if(longString == "")
						longString = item + "\n";
					else
						longString += item + "\n";
				}
			}
			activeButton = "GameTimes";
		}
		
		protected void warningButton() {
			longString = "";
			foreach (string item in debugOutput) {
				if (item.Contains("Warning")) {
					if(longString == "")
						longString = item + "\n";
					else
						longString += item + "\n";
				}
			}
			activeButton = "Warning";
		}
		
		protected void errorButton() {
			longString = "";
			foreach (string item in debugOutput) {
				if (item.Contains("Error")) {
					if(longString == "")
						longString = item + "\n";
					else
						longString += item + "\n";
				}
			}
			activeButton = "Error";
		}
		
		protected void exceptionButton() {
			longString = "";
			foreach (string item in debugOutput) {
				if (item.Contains("Exception")) {
					if(longString == "")
						longString = item + "\n";
					else
						longString += item + "\n";
				}
			}
			activeButton = "Exception";
		}
	


		virtual protected void sendHighscore(string totalscore) 
		{
		}

		public void OnGUI() {

			if (Epigene.GAME.GameManager.Instance.debugMode == false)
				return;

				gameObject.SetActive (true);
			// Make the toggle button.
			if (GUI.Button (new Rect (20, 15, 80, 20), labelShowHide)) {
				if (showDebug == false) {
					showDebug = true;
					labelShowHide = "hide";
				} else {
					showDebug = false;
					labelShowHide = "show";
				}
			}

			if (showDebug == true) {
				// Make a background box
				GUI.Box (new Rect (10, 10, 320, 190), "");

				// Make the clear button.
				if (GUI.Button (new Rect (105, 15, 80, 20), "clear")) {
					longString = "";
					debugOutput.Clear();
					//addOutput ("Version build: " + RetrieveLinkerTimestamp ().ToString (), 0);
				}
				
				// Make the Info only button.
				if (GUI.Button (new Rect (240, 15, 85, 20), "All")) {
					allButton();
				}
				
				// Make the Info only button.
				if (GUI.Button (new Rect (240, 40, 85, 20), "Info")) {
					infoButton();
				}
				
				// Make the GameTimes only button.
				if (GUI.Button (new Rect (240, 65, 85, 20), "GameTimes")) {
					gametimesButton();
				}
				
				// Make the GameTimes only button.
				if (GUI.Button (new Rect (240, 90, 85, 20), "Warning")) {
					warningButton();
				}
				
				// Make the GameTimes only button.
				if (GUI.Button (new Rect (240, 115, 85, 20), "Error")) {
					errorButton();
				}
				
				// Make the GameTimes only button.
				if (GUI.Button (new Rect (240, 140, 85, 20), "Exception")) {
					exceptionButton();
				}

				int height = 500;

				if (!string.IsNullOrEmpty(longString))
					height = (int) longString.Length/2; //((((float)longString.Length) / 30.0f) * 15);
				if (height < 500) height = 500;

				scrollPosition = GUI.BeginScrollView (
					new Rect (20, 40, 215, 150), 
					scrollPosition, new Rect (10, 20, 190, height));
				GUI.Box (new Rect (5, 15, 205, height), "");

				if (updateGUI == true) {
					switch (activeButton) {
						case "All":
							allButton();
							break;
						case "Info":
							infoButton();
							break;
						case "GameTimes":
							gametimesButton();
							break;
						case "Warning":
							warningButton();
							break;
						case "Error":
							errorButton();
							break;
						case "Exception":
							exceptionButton();
							break;
						default:
							break;
					}
					updateGUI = false;
				}

				GUI.Label (new Rect (10, 20, 205, height), longString);

				GUI.EndScrollView ();
			}
		}
	}
}
