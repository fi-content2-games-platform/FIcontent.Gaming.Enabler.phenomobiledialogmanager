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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//------------------------------------------------------------------------------
using Epigene;
using Epigene.UI;
using Epigene.GAME;
using Epigene.VIEW;
using Epigene.MODEL;
using Epigene.IO;
using MiniJSON;
//------------------------------------------------------------------------------
public class DialogScreen : MonoBehaviour 
{

	//ScreenID
	public int ID 
	{
		get { return id;}
		set { this.id = value;}
	}
	private int id;

	/// <summary>
	/// The user interface manager.
	/// </summary>
	private UIManager uiManager;

	/// <summary>
	/// Player manager
	/// </summary>
	private GameManager gpm;

	/// <summary>
	/// MultiChoice view
	/// </summary>
	private DialogView dialogView;

//------------------------------------------------------------------------------

	/// <summary>
	/// Init the components
	/// </summary>
	void Awake()
	{

		DialogView[] diaComps;
		diaComps = gameObject.GetComponentsInChildren<DialogView>();
		Log.Assert(diaComps.Length == 1, 
			"Missing or two many DialogView found:"+
		           diaComps.Length+" in children of "+gameObject.name);

		dialogView = diaComps[0];


	}//Awake()
	
	/// <summary>
	/// Init the screen, load and set the dialogs
	/// </summary>
	void Start () 
	{

		uiManager = UIManager.Instance;
		gpm = GameManager.Instance;
		//set up dialogs
		dialogView.Load("Config/dialogs_CP1_4_2");
		dialogView.ProcessAnswer = ProcessMultiChoiceAnswer;
		dialogView.ActivateDialog("D001");

	}//Start()

	/// <summary>
	/// Function to process the answer for multichoice dialogs
	/// </summary>
	public void ProcessMultiChoiceAnswer(UIDialog dialog, EmotionType answer)
	{
		Log.Info("Dialog "+dialog.Id+" answer:"+answer);

	}//ProcessAnswer


}//class CP_DialogTest
//------------------------------------------------------------------------------
