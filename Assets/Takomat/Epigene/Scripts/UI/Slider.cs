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

using Epigene.UI;

namespace Epigene.UI
{

	public class Slider : Button 
	{

		public string onUpdate;

		public UISlider.UISliderType sliderType;
		public float value;
		public float minValue;
		public float maxValue;
		public float step;
		public Rect dimension;


		public float MaxValue
		{
			set { 
				maxValue = value;
				if (uiSlider != null)
				{
					uiSlider.Max = value;
				}
			}
			get { 
				return this.uiSlider.Max; 
			}
		}

		public float MinValue
		{
			set { 
				minValue = value;
				if (uiSlider != null)
				{
					uiSlider.Min = value;
				}
			}
			get { 
				return this.uiSlider.Min; 
			}
		}

		public float Value
		{
			set { 
					this.value = value;
					if (uiSlider != null)
					{
						uiSlider.Value = value;
					}
				}
			get { 
					return this.uiSlider.Value; 
				}
		}

		private UISlider uiSlider;

		/// <summary>
		/// Set the main value for the slider.
		/// TODO We have here some problem due to sealed monobehavior functions
		/// and inheritance. For now we use Awake here and Start in the base Button.
		/// </summary>
		public void Awake()
		{

			this.uiType = UIType.Slider;

			Vector3 pos = gameObject.transform.localPosition;

			float h = 0, w = 0;
			step = 1;
			switch(sliderType)
			{
				case UISlider.UISliderType.VERTICAL:
					//h = 100;
					Log.Assert(dimension.height != 0, "Missing dimension.height for vertical slider: "+gameObject.name);
					//w = 0;
					break;
				case UISlider.UISliderType.HORIZONTAL:
					Log.Assert(dimension.width != 0, "Missing dimension.width for HORIZONTAL slider: "+gameObject.name);
					break;

				case UISlider.UISliderType.MAP:
					throw new System.ArgumentException("Slider type MAP is not implemented!");
			}
			Log.Assert(minValue != maxValue, "Min and max cannot be the same! "+gameObject.name);

			dimension.x = pos.x;
			dimension.y = pos.y;
			//dimension.height = -dimension.height; //flip coord

			base.Init();

		}//Awake()

		// TODO check this why works and sometimes not works
		// void Start()
		// {
		// 	Log.Warning(" 1111 - Start");
		// 	base.Init();
		// }

		/// <summary>
		/// Register the slider's functions
		/// </summary>
		public override void Register(UIButton slider)
		{
			uiSlider = (UISlider)slider;
			base.Register(uiSlider);
			uiSlider.Register(UIEvent.Update, UpdateValue);
			uiSlider.Set(sliderType, dimension, value, minValue, maxValue, step);

		}//Register()

		/// <summary>
		/// Callback to update the handler function with
		/// the updated value of the slider.
		/// </summary>
		public void UpdateValue(float value)
		{
			//Log.Info("slider "+gameObject.name+" updated:"+value);
	
			this.value = value;
			if(handler != null && onUpdate != null)
			{
				handler.SendMessage(onUpdate, value);
			}
		}//UpdateValue()

	}//class Slider

}//namespace