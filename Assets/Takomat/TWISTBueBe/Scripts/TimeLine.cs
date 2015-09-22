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

using Epigene;
//using Epigene.GAME;
//using Epigene.MODEL;
using Epigene.UI;
//using Epigene.AUDIO;
//using Epigene.VIEW;

//------------------------------------------------------------------------------
using Epigene.GAME;


namespace TWISTBueBe
{

	/// <summary>
	/// TimeLine 
	/// </summary>
	public class TimeLine : MonoBehaviour 
	{

		public 	GameObject 	btnOpen;
		public 	GameObject 	btnClose;
		public 	GameObject 	bgImageOpen;
		public 	GameObject 	bgImageClose;
		public 	GameObject 	bgImageTimeline;
		public	GameObject 	bgTimebar;
		public 	Button 		btnPlay;
		public	Button 		btnPause;
		public	Button 		btnOne;
		public	Button 		btnTwo;
		public	Button 		btnFour;
		public 	GameObject 	slider;
		public 	UIScreen   	backTo;
		private Slider	   	sl;
		public 	bool	   	isActive;
		private int			multiplyButtonState;

		public enum TimelineType : int {CP = 0, SIM};
		public 		TimelineType   type;

		public UIText timelineValue;

		/// <summary>
		/// Handling visibility. When this flag is set 
		/// it will always be displayed in the UIScreen,
		// in which it is defined
		/// </summary>
		/// <value>The function to call.</value>
		public bool VisibleWhenInScreen
		{
			get { return visibleWhenInScreen; }
			set { visibleWhenInScreen = value;}
		}
		private static UpdateFunction updateType;
		
		private bool visibleWhenInScreen = false;

		/// <summary>
		/// set/get time value directly
		/// in case of set, this will change
		/// the slider position
		/// </summary>
		public float TimeValue
		{
			set
			{
				UpdateSlider(value);
			}
			get
			{
				return (sl != null) ? sl.value : 0;
			}
		}
		
		/// <summary>
		/// Get/Set the text of the slider
		/// </summary>
		public string Text
		{
			set
			{
				timelineValue.Text = value;
			}
			get
			{
				return timelineValue.Text;
			}
		}

		/// <summary>
		/// material for the bgImage of the slider
		/// used to pass value to the shader
		/// </summary>
		private Material material;


		/// <summary>
		/// Check components
		/// </summary>
		void Awake()
		{
			Log.Assert(btnOpen, "Missing btnOpen in:"+gameObject.name);
			Log.Assert(btnClose, "Missing btnClose in:"+gameObject.name);
			Log.Assert(bgImageOpen, "Missing bgImageOpen in:"+gameObject.name);
			Log.Assert(bgImageClose, "Missing bgImageClose in:"+gameObject.name);
			Log.Assert(bgImageTimeline, "Missing bgImageTimeline in:"+gameObject.name);
			Log.Assert(bgTimebar, "Missing bgTimebar in:"+gameObject.name);
			Log.Assert(slider, "Missing slider in:"+gameObject.name);
			Log.Assert(timelineValue, "Missing timelineValue in:"+gameObject.name);

			sl = slider.GetComponent<Slider> ();
			isActive = false;
			multiplyButtonState = 1;

			SpriteRenderer spriteRenderer = bgImageTimeline.GetComponent<SpriteRenderer>();
			if(spriteRenderer)
			{
				material = spriteRenderer.material;
			}
			//Show(false);

		}//Awake()

		/// <summary>
		/// Enable the timeline
		/// </summary>
		void OnEnable()
		{
			ShowControl(false);

		}//OnEnable()

		/// <summary>
		/// Disable the timeline
		/// </summary>
		void OnDisable()
		{

		}//OnDisable()

		// Use this for initialization
		void Start () 
		{
		}
		
		// Update is called once per frame
		void Update () 
		{
			float startX = bgImageTimeline.transform.renderer.bounds.min.x;
			float endX = bgImageTimeline.transform.renderer.bounds.max.x;

			float percent = (slider.transform.position.x - startX) / (endX - startX);

			if(material)
				material.SetFloat("_Percent", percent);
		}

		/// <summary>
		/// Show or hide the timeline
		/// </summary>
		public void Show(bool flag)
		{

			btnOpen.SetActive(flag);
			btnClose.SetActive(flag);
			bgImageOpen.SetActive(flag);
			bgImageClose.SetActive(flag);
			bgImageTimeline.SetActive(flag);
			bgTimebar.SetActive(flag);
			slider.SetActive(flag);

		}//Show()

		public void OnButtonClick(string name)
		{
			//CP
			if (type == TimelineType.CP) 
			{
				switch (name)
				{
				case "btn_OpenCP":
					Log.Info ("Open TimeLine CP");
					ShowControl (true);
					if (isActive)
					{
						if (btnPlay != null)
							btnPlay.Toggle();
						if (btnPause != null)
							btnPause.Up ();
					}
					else
					{
						if (btnPlay != null)
							btnPlay.Up();
						if (btnPause != null)
							btnPause.Toggle ();
					}
						break;
				case "btn_CloseCP":
				case "btn_CloseSIM":
					Log.Info ("Close TimeLine CP");
					ShowControl (false);
					break;
				case "btn_PlayCP":
					if (!isActive)
					{
						GameManager.Instance.Event("TIMELINE",
					                           type.ToString(),
					                           "Play");
						if (btnPlay != null)
							btnPlay.Toggle();
						if (btnPause != null)
							btnPause.Up ();
					}
					break;
				case "btn_PauseCP":
					if (isActive)
					{
						GameManager.Instance.Event("TIMELINE",
						                           type.ToString(),
						                           "Pause");
						if (btnPlay != null)
							btnPlay.Up();
						if (btnPause != null)
							btnPause.Toggle();

						backTo = UIManager.Instance.ActivateScreen ("Pause");
						backTo.backScreenName = "Simulation";
						UIManager.Instance.ActivateScreen ("Pause");
					}
					break;
				}
			}
			
			//SIM
			else if (type == TimelineType.SIM)
			{
				switch (name)
				{
				case "btn_OpenSIM":
					Log.Info("Open TimeLine SIM");
					ShowControl(true);

					if (isActive)
					{
						if (btnPlay != null)
							btnPlay.Toggle();
						if (btnPause != null)
							btnPause.Up ();
					}
					else
					{
						if (btnPlay != null)
							btnPlay.Up();
						if (btnPause != null)
							btnPause.Toggle();
					}
					multiplyButtonState = (int) GameManager.Instance.Get("SIM").Speed;
					switch (multiplyButtonState)
					{
					case 1: if (btnOne != null)
								btnOne.Toggle();
							if (btnTwo != null)
								btnTwo.Up();
							if (btnFour != null)
								btnFour.Up();
							break;
					case 2: if (btnOne != null)
								btnOne.Up();
							if (btnTwo != null)
								btnTwo.Toggle();
							if (btnFour != null)
								btnFour.Up();
							break;
					case 4: if (btnOne != null)
								btnOne.Up();
							if (btnTwo != null)
								btnTwo.Up();
							if (btnFour != null)
								btnFour.Toggle();
							break;
					}
						break;
				case "btn_PlaySIM":
					if (!isActive)
					{
						GameManager.Instance.Event("TIMELINE",
						                           type.ToString(),
						                           "Play");
						if (btnPlay != null)
							btnPlay.Toggle();
						if (btnPause != null)
							btnPause.Up ();
					}
					break;
				case "btn_CloseCP":
				case "btn_CloseSIM":
					Log.Info("Close TimeLine SIM");
					ShowControl(false);
					break;
				case "btn_PauseSIM":
					if (isActive)
					{
						GameManager.Instance.Event("TIMELINE",
						                           type.ToString(),
						                           "Pause");
						if (btnPlay != null)
							btnPlay.Up();
						if (btnPause != null)
							btnPause.Toggle();
						
						backTo = UIManager.Instance.ActivateScreen ("Pause");
						backTo.backScreenName = "Simulation";
						UIManager.Instance.ActivateScreen ("Pause");
					}
					break;
				case "btn_1x":
					GameManager.Instance.Event("TIMELINE",
					                           type.ToString(),
					                           "Speed_1");
					multiplyButtonState = 1;
					if (btnOne != null)
						btnOne.Toggle();
					if (btnTwo != null)
						btnTwo.Up();
					if (btnFour != null)
						btnFour.Up();
					Log.Info("btn_1x TimeLine SIM");
					break;
				case "btn_2x":
					GameManager.Instance.Event("TIMELINE",
					                           type.ToString(),
					                           "Speed_2");
					multiplyButtonState = 2;
					if (btnOne != null)
						btnOne.Up();
					if (btnTwo != null)
						btnTwo.Toggle();
					if (btnFour != null)
						btnFour.Up();
					Log.Info("btn_2x TimeLine SIM");
					break;
				case "btn_4x":
					GameManager.Instance.Event("TIMELINE",
					                           type.ToString(),
					                           "Speed_4");
					multiplyButtonState = 4;
					if (btnOne != null)
						btnOne.Up();
					if (btnTwo != null)
						btnTwo.Up();
					if (btnFour != null)
						btnFour.Toggle();
					Log.Info("btn_4x TimeLine SIM");
					break;
				}
			}
			
		}//OnButtonClick()

		/// <summary>
		/// Update the slider with current value
		/// </summary>
		public void UpdateSlider(float param)
		{
			//Log.Info("UpdateSlider:"+param+" - "+type+" sl:"+(sl!=null) + " isActive:"+isActive);

			if (isActive && sl != null)
			{
				sl.Value = param;
			}
		}//UpdateSlider	

		public void StartSlider()
		{
			WebPlayerDebugManager.addOutput ("StartSlider", 1);
			isActive = true;
			timelineValue.Text = "";

			//StartCoroutine ("UpdateSlider", startTime+duration);
		}
		
		public void StopSlider()
		{
			isActive = false;
		}
		
		public void PauseSlider()
		{
			isActive = false;
			if (btnPlay.UIButton == null) return;
			if (btnPlay != null)
				btnPlay.Up();
			if (btnPause != null)
				btnPause.Toggle();
		}
		
		public void UnpauseSlider()
		{
			isActive = true;
			if (btnPlay.UIButton == null) return;
			if (btnPlay != null)
				btnPlay.Toggle();
			if (btnPause != null)
				btnPause.Up();
		}
		
		public void SwitchPauseSlider()
		{
			isActive = !isActive;
		}

		/// <summary>
		/// Show the control part
		/// </summary>
		public void ShowControl(bool flag)
		{

			//for now we simply show/hide it
			bgImageOpen.SetActive(flag);
			btnOpen.renderer.enabled = !flag;

			/// TODO: here could come some triggers to make
			/// animation on opening (slow open/close)
			/// using repeat value every fire could open/close
			/// only a bit.

		}//ShowControl()

		/// <summary>
		/// Set the time bar values.
		/// value will
		/// </summary>
		public void SetTimeBar(float value, float minValue, float maxValue)
		{
			if(sl)
			{
				sl.minValue = minValue;
				sl.maxValue = maxValue;
				sl.value = value;
			}
			else
			{
				Log.Warning("No slider yet! "+gameObject.name);
			}
		}

	}//class TimeLine
}//namespace