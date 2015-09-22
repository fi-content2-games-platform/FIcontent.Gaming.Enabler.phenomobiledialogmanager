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


namespace Epigene.UI
{
	
	//TODO:
	// - implement step value
	// - implement HORIZONTAL and MAP types	


	/// <summary>
	/// Slider
	/// </summary>
	public class UISlider : UIButton 
	{
		

		private Rect rect;

		private Vector3 pos;

		public enum UISliderType {VERTICAL, HORIZONTAL, MAP};

		private UISliderType sliderType;
		private float value;
		private float min;
		private float max;
		private float step;

		/// <summary>
		/// value of the slider
		/// setting this value will reposition
		/// the slider to the new value
		/// </summary>
		public float Value
		{
			set { 
					if(value <= max && value >= min)
					{
						this.value = value;
						UpdatePosition();
					}
				}
			get { 
					return this.value; 
				}
		}

		public float Max
		{
			set { 
				max = value;
				if(this.value > max)
					this.value = max;
				else
					UpdatePosition();
			}
			get { 
				return max; 
			}
		}

		public float Min
		{
			set { 
				min = value;
				if(this.value < min)
					this.value = min;
				else
					UpdatePosition();
			}
			get { 
				return min; 
			}
		}


		//private UpdateFunction onUpdate;
		//private UIImage image;

		/// <summary>
		/// Delegates for update value on slider.
		/// The given functions will be called
		/// when the value is changes.
		/// Hook your callback to this function
		/// if you want the slider to pass the value on change.
		/// (ie. user moves slider and a callback will notify your code)
		/// You don't have to add a function if you don't need to.
		/// </summary>
		private UpdateFunction onUpdate;
		
	
		public UISlider(GameObject obj, Sprite[] sprites)				
			:base(obj, sprites)
		{

			this.type = UIType.Slider;

			Log.Debug(obj.name+" screenPos:"+pos);

			sliderType = UISliderType.VERTICAL;		

		}//ctor()

		public void Set(UISliderType sliderType, Rect rect, float value, float min, float max, float step)
		{
			
			this.sliderType = sliderType;
			this.rect = rect;
			this.value = value;
			this.min = min;
			this.max = max;
			this.step = step;

			//Log.Info(string.Format("Slider set: rect:{0} value: {1} min: {2} max: {3} step: {4}", rect, value, min, max, step));


			UpdatePosition();
		}

		/// <summary>
		/// Call when need to update the slider
		/// </summary>
		public void UpdatePosition()
		{
			//Debug.Log("<color=green>rect:</color>"+rect);

			this.pos.x = rect.x;
			this.pos.y = rect.y;
			this.pos.z = 0;

			switch(sliderType)
			{
				case UISliderType.VERTICAL:
					this.pos.y = rect.y + (value / (max-min)) * rect.height;
					break;

				case UISliderType.HORIZONTAL:
					this.pos.x = rect.x + (value / (max-min)) * rect.width;
					break;

				default:
					//TODO implement HORIZONTAL and MAP
					Log.Error("ONLY VERTICAL Implemented yet! Sorry..");
					break;
			}

			//change slider position
			// Debug.Log("GO name:"+gameObject.name);
			// Debug.Log("trans.localPosition:"+gameObject.transform.localPosition);
			gameObject.transform.localPosition = new Vector3(this.pos.x, this.pos.y, this.pos.z);
			//Log.Info("this.pos=" + this.pos + " value:"+value);
		}

		/// <summary>
		/// Register an event to function for the slider.
		/// </summary>
		public bool Register(UIEvent uiEvent, UpdateFunction function)
		{

			if(uiEvent == UIEvent.Update)
			{
				Log.Debug("Register event:"+uiEvent+" to button:"+gameObject.name);
				onUpdate = function;
				return true;
			}

			return false;
		}//Register()

		/// <summary>
		/// Update the slider value based on mouse move
		/// and selected type
		/// </summary>
		public override bool Push()
		{
			//Log.Info("Slider Pressed:"+this.gameObject.name+" pos:"+pos);

			if(base.Push())
			{
				//get mouse coord.
				Vector3 globalPoint = UIManager.UICamera.ScreenToWorldPoint(Input.mousePosition);
				Vector3 point = gameObject.transform.parent.InverseTransformPoint(globalPoint);

				float x = pos.x;
				float y = pos.y;

				//Log.Info("Slider mouse.pos:" + point + " input:"+Input.mousePosition + " rect:"+rect);

				switch(sliderType)
				{
					case UISliderType.VERTICAL:
						float minY = (rect.y < rect.y + rect.height) ? rect.y : rect.y + rect.height;
						float maxY = (rect.y > rect.y + rect.height) ? rect.y : rect.y + rect.height;
						y = (point.y < minY) ? minY : point.y;
						y = (y > maxY) ? maxY : y;

						value = ((y - rect.y) / rect.height) * (max - min);
					 	//Log.Info(string.Format("y={0} rect.y={1} rect.height= {2} minY={3} maxY={4}",y,rect.y,rect.height,minY,maxY));
					 	//Log.Info("VALUE = "+value);
					 	break;

					case UISliderType.HORIZONTAL:
						float minX = (rect.x < rect.x + rect.width) ? rect.x : rect.x + rect.width;
						float maxX = (rect.x > rect.x + rect.width) ? rect.x : rect.x + rect.width;
						x = (point.x < minX) ? minX : point.x;
						x = (x > maxX) ? maxX : x;
					 	value = ((x - rect.x) / rect.width) * (max - min);
					 	//Log.Info(string.Format("x={0} rect.x={1} rect.height= {2} minx={3} maxx={4}",x,rect.x,rect.height,minX,maxX));
						break;

					case UISliderType.MAP:
						//TODO implement me!!
						break;


				}


				//move the sprite
				UpdatePosition();

				//calls the delegates
				if(onUpdate != null)
					onUpdate(value);

				return true;		
				
			}

			return false;

		}//Pressed()





	}//class UISlider

}//UIImage