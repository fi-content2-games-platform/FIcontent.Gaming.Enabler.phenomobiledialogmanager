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
using System.Text;
using Epigene.UI;



namespace Epigene.MODEL
{
	/// <summary>
	/// Thsi class represents a question - answer pairs	 
	/// Questiongs taken from the localization db.
	/// Every question has 3 answers, each of these
	/// has a prefix in the id which determines the type.
	/// Q = question
	/// G = Good answer
	/// B = Bad answer
	/// N = Neutral answer
	/// Example: 
	/// id = 001
	/// Q001="What?"
	/// G001="Can I ask something?"
	/// B001="Leave me alone!"
	/// N001="nothing.."
	/// </summary>
	public class MultiChoice
	{

		/// <summary>
		/// Prefix used for multichoice localization ID
		/// </summary>
		public static string[] prefix = {"Q", "G", "B", "N"};

		public static string idFormat = "000";

		/// <summary>
		/// Question
		/// </summary>
		public string Question
		{
			get { return question;}
		}
		private string question;

		/// <summary>
		/// Good answer
		/// </summary>
		public string Good
		{
			get { return answerGood;}
		}
		private string answerGood;

		/// <summary>
		/// Bad answer
		/// </summary>
		public string Bad
		{
			get { return answerBad;}
		}
		private string answerBad;

		/// <summary>
		/// Neutral answer
		/// </summary>
		public string Neutral
		{
			get { return answerNeutral;}
		}
		private string answerNeutral;

		/// <summary>
		/// localization id
		/// </summary>
		public int ID
		{
			get {return id;}			
		}
		private int id;

		/// <summary>
		/// localization section
		/// </summary>
		public string Section
		{
			get {return section;}			
		}
		private string section;

		/// <summary>
		/// ctor set up question with answers
		/// by question id from localization db.
		/// Every id start with a letter which 
		/// describe the type.
		/// Use id without the prefix.
		/// </summary>
		public MultiChoice(string id)
		{
			//init by i18n qid, it should fill in with answ.
			I18nManager i18n = I18nManager.Instance;

			
			//set id
			
			string strId 	= SetId(id);

			StringBuilder sb = new StringBuilder();
			sb.Append(section);
			sb.Append(".");
			sb.Append(prefix[0]);
			sb.Append(strId);
			question = sb.ToString();
			question 		= i18n.Get(question);

			sb = new StringBuilder();
			sb.Append(section);
			sb.Append(".");
			sb.Append(prefix[1]);
			sb.Append(strId);
			answerGood = sb.ToString();
			answerGood 		= i18n.Get(answerGood);

			sb = new StringBuilder();
			sb.Append(section);
			sb.Append(".");
			sb.Append(prefix[2]);
			sb.Append(strId);
			answerBad = sb.ToString();
			answerBad 		= i18n.Get(answerBad);

			sb = new StringBuilder();
			sb.Append(section);
			sb.Append(".");
			sb.Append(prefix[3]);
			sb.Append(strId);
			answerNeutral = sb.ToString();
			answerNeutral = i18n.Get(answerNeutral);

		}//ctor()
		
		/// <summary>
		/// Helper to convert string id from localization db
		/// into an int
		/// Although this is a one line function,
		/// it helps to keep the logic of the id
		/// in one place, hence easy to modify
		/// </summary>
		private string SetId(string id)
		{
			// Log.Debug("MC Set id:"+id);
			string[] ids = id.Split('Q');
			// Log.Assert(ids.Length == 2, "Missing 'Q' in id:"+id);

			section = ids[0].Trim('.');
			// Log.Debug("Loading MultiChoice:"+section+"."+ids[1]);
			this.id = System.Convert.ToInt32(ids[1]);
			

			return this.id.ToString(idFormat);

		}//ConvertId()

		/// <summary>
		/// Helper to convert object to string for debugging only
		/// </summary>
		public override string ToString()
		{
			return string.Format("MultiChoice id={0}\nQ: {1}\nG: {2}\nB: {3}\nN: {4}", 
				id, question, answerGood, answerBad, answerNeutral);
		}

	}//class MultiChoice

	/// <summary>
	/// Manager for multichoices.
	/// It's a singleton which will keep 
	/// and id for the active question and 
	/// manage the selected answer.
	/// </summary>
	public class MultiChoiceManager 
	{
		/// <summary>
		/// Gets the instance.
		/// </summary>		
		public static MultiChoiceManager Instance
		{
			get { return instance; }
		}
		private static readonly MultiChoiceManager instance = new MultiChoiceManager();

		/// <summary>
		/// list for questions and answers by id
		/// </summary>
		private Dictionary<int, MultiChoice> multiList;

		/// <summary>
		/// get current multichoice
		/// </summary>
		public MultiChoice Current
		{
			get{ return multiList[currentId]; }
		}

		/// <summary>
		/// id of the current/active multichoice
		/// </summary>
		public int ID
		{
			get { return currentId; }
			set { currentId = value; }
		}
		private int currentId;

		/// <summary>
		/// ctor
		/// This function will read the localization db
		/// and set up each MultiChoice item from 
		/// the section "MultiChoice"
		/// </summary>
		private MultiChoiceManager()
		{

			//get all items from localization db
			I18nManager i18n = I18nManager.Instance;
			List<string> keys = i18n.GetKeys("MultiChoice");
			
			//create list of multichoices
			multiList = new Dictionary<int, MultiChoice>();

			// Log.Assert(keys.Count > 0, 
			//	"Cannot find "+i18n.Language+".MultiChoice resource section in localization db");

			// Log.Debug("key0:"+keys[0]);

			//set statid nice and big
			int startId = 10000;
			foreach(string strId in keys)
			{
				if(strId.Contains(MultiChoice.prefix[0]))
				{
					MultiChoice mc = new MultiChoice(strId);
					multiList.Add(mc.ID, mc);

					//find lowest id
					if(mc.ID < startId)
						startId = mc.ID;
				}
			}
			
			//set the current id to the lowest id
			currentId = startId;

			// Log.Debug("current id"+currentId);

		}//ctor()



	}//class MultiChoiceManager

}//namespace 