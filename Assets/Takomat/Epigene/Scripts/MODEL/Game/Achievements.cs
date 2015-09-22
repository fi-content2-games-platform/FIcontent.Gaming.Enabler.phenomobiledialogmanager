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

using Epigene;
using Epigene.GAME;
//------------------------------------------------------------------------------
namespace Epigene.MODEL
{

	/// <summary>
	/// Achivement
	/// </summary>
	public class Achievement
	{
		/// <summary>
		/// available items for npc, based on sprite order
		/// </summary>
		public enum Character {GIRL = 0, BOY, GRANDPA, DAD, LADY, LOGO, ENGINEER};

		/// <summary>
		/// id of this achievement
		/// this allows to have multiple 
		/// similar (same npc) defined by muitple screen
		/// use unique id like screen or dialog fullId.
		/// </summary>
		public string Id
		{
			get{return id;}
		}
		private string id;

		public bool      completed;
		public bool		 started;
		public Character npc;
		public string    text;
		public string 	 infotext;
		/*public string    LogTime
		{
			set{ logTime = value+" UHR"; }
			get{ return logTime; }
		}
		public string logTime;*/

		/// <summary>
		/// ctor
		/// </summary>
		public Achievement(string id)
		{
			this.id     = id;
			completed   = false;
			started		= false;
			npc         = Character.GIRL;
			text        = "";
			infotext 	= "";
			//logTime    = "";

		}//ctor()

	}//class Achievement
//------------------------------------------------------------------------------
	/// <summary>
	/// Model for manage achievements in the game.
	/// </summary>
	public sealed class Achievements
	{
		/// <summary>
		/// Contains a the iterators of completed achievements
		/// </summary>
		public List<int> achievementsDoneOrStarted;

		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static Achievements Instance
		{
			get { return instance; }
		}
		private static readonly Achievements instance = new Achievements();

		/// <summary>
		/// list of achievements
		/// </summary>
		//private Dictionary<string, Achievement> items;
		public List<Achievement> Items
		{
			get { return items; }
		}
		private List<Achievement> items;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIManager"/> class.
		/// </summary>
		Achievements()
		{
			Log.Debug("Achievements initialized.");

			//items = new Dictionary<string, Achievement>();
			items 					  = new List<Achievement>();
			achievementsDoneOrStarted = new List<int>();;
			
		}//ctor()


		/// <summary>
		/// Add an item.
		/// </summary>
		public Achievement Add(string id)
		{
			//search for existing item first
			//Log.GameTimes ("Achievements Get Add" + id);
			foreach(Achievement it in items)
			{
				if(it.Id == id)
				{
					//we found it
					return it;
				}
			}

			//create a new item
			Achievement item = new Achievement(id);
			items.Add(item);

			return item;	

		}//Get()


		/// <summary>
		/// Add an item.
		/// </summary>
		public Achievement Get(string id)
		{
			//search for existing item first
			//Log.GameTimes ("Achievements Get Add" + id);
			foreach(Achievement it in items)
			{
				if(it.Id == id)
				{
					//we found it
					return it;
				}
			}

			return null;	
			
		}//Get()


		/// <summary>
		/// Check if item in list.
		/// </summary>
		public bool Check(string id)
		{
			//search for existing item first
			Log.GameTimes ("Achievements Check Add" + id);
			foreach(Achievement it in items)
			{
				if(it.Id == id)
				{
					return true;
				}
			}
			return false;
			
		}//Check()

		/// <summary>
		/// Resets all items in list.
		/// </summary>
		public void Reset()
		{
			//search for existing item first
			Log.GameTimes ("Achievements Resetted");
			foreach(Achievement it in items)
			{
				it.completed = false;
				it.started   = false;
			}
			achievementsDoneOrStarted.Clear ();
		}//Check()

		public void logAchievements()
		{
			for (int i = 0; i < items.Count; i++)
			{
				if((items[i].Id == "26b02e36-4fa0-4f0c-98f0-9915646483d1") ||
				   (items[i].Id == "b27a3ba6-745b-4797-a1a7-1e5a569ab510") ||
				   (items[i].Id == "1f0f2c91-9a58-4bbc-af2c-5330f1ce0208"))
				{
					// skip TechSpots "Achievements"
				}
				else if (((items[i].completed) || (items[i].started)) &&
				    (!achievementsDoneOrStarted.Contains(i)))
					achievementsDoneOrStarted.Add(i);
			}
		}

	}//class Achievements
}//namespace
//------------------------------------------------------------------------------