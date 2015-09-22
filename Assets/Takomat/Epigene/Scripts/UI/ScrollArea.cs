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
// 
// using Epigene.AUDIO;

namespace Epigene
{
	public class ScrollArea : MonoBehaviour
	{
		public Slider slider;
		public Rect visibleArea;
		public bool debug;

		private float lastValue = 0.0f;
		private GameObject cube;

		void Awake()
		{
			Log.Assert(slider, "Slider is missing");

			//TODO editor (or debug) only
			cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.SetActive(false);
			cube.layer = gameObject.layer;
			cube.GetComponent<Renderer>().material.color = new Color(0.9f, 0.2f, 0.2f, 0.1f);
			cube.transform.parent = gameObject.transform.parent;
			cube.transform.localPosition = new Vector3(visibleArea.x + (visibleArea.width / 2.0f), 
		 							visibleArea.y + (visibleArea.height / 2), 
	 								gameObject.transform.position.z);
			cube.transform.localScale = new Vector3(visibleArea.width, visibleArea.height, 1);
			cube.GetComponent<Renderer>().sortingOrder = 1000;//gameObject.renderer.sortingOrder+1;
		}

		// Use this for initialization
		void Start()
		{
			RefreshContent();
		}
		
		// Update is called once per frame
		void Update()
		{
			cube.SetActive(debug);
		}

		public void OnSliderUpdate(float value)
		{
			Log.Info("OnSliderUpdate: " + value);
			// value = lastValue +0.001f;
			// UpdateShaderProperties(gameObject, value - lastValue);
			UpdatePositions(gameObject, value - lastValue);
			lastValue = value;
		}

		public void RefreshContent()
		{
			SetShaders(gameObject);
			UpdateShaderProperties(gameObject, lastValue);
		}

		void UpdatePositions(GameObject obj, float value)
		{
			Vector3 pos = obj.transform.position;
			pos.y += value;
			obj.transform.position = pos;

			// foreach(Transform child in obj.transform)
			// {
			// 	pos = child.gameObject.transform.position;
			// 	pos.y += value;
			// 	child.gameObject.transform.position = pos;
			// 	UpdatePositions(child.gameObject, value);
			// }
		}

		void UpdateShaderProperties(GameObject obj, float value)
		{
			if (obj.GetComponent<Renderer>() != null)
			{
				obj.GetComponent<Renderer>().material.SetFloat("_PositionY", value);
			}

			foreach(Transform child in obj.transform)
			{
				if (child.gameObject.GetComponent<Renderer>() != null)
				{
					child.gameObject.GetComponent<Renderer>().material.SetFloat("_PositionY", value);
				}
				UpdateShaderProperties(child.gameObject, value);
			}
		}

		void SetShaders(GameObject obj)
		{
			Material spriteMaterial = (Material)Resources.Load("Epigene/Materials/ScrollArea", typeof(Material));
			Material textMaterial = (Material)Resources.Load("Epigene/Materials/ScrollAreaText", typeof(Material));
			Material chartMaterial = (Material)Resources.Load("Epigene/Materials/ScrollAreaChart", typeof(Material));

			Vector3 globalPos = gameObject.transform.parent.TransformPoint(visibleArea.x, visibleArea.y, 0);

			Vector4 area = new Vector4(globalPos.x, globalPos.y, 
				globalPos.x + visibleArea.width, globalPos.y + visibleArea.height);

			if (obj.GetComponent<Renderer>() != null)
			{
				TextMesh textMesh = obj.GetComponent<TextMesh>();
				if (textMesh != null)
				{
					MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
					meshRenderer.material = textMaterial;
					meshRenderer.material.SetVector("_Area", area);
					meshRenderer.material.SetColor("_Color", textMesh.color);
				}
				else
				{
					if (obj.name == "UIChartLine") //TODO
					{
						obj.GetComponent<Renderer>().material = chartMaterial;
					}
					else if (obj.name == "UIChartPie") //TODO
					{
						obj.GetComponent<Renderer>().material = chartMaterial;
					}
					else
					{
						obj.GetComponent<Renderer>().material = spriteMaterial;
					}
					obj.GetComponent<Renderer>().material.SetVector("_Area", area);
				}
				// area size is static
			}

			foreach(Transform child in obj.transform)
			{
				if (child.gameObject.GetComponent<Renderer>() != null)
				{
					TextMesh textMesh = obj.GetComponent<TextMesh>();
					if (textMesh != null)
					{
						MeshRenderer meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
						meshRenderer.material = textMaterial;
						meshRenderer.material.SetVector("_Area", area);
						meshRenderer.material.SetColor("_Color", textMesh.color);
					}
					else
					{
						child.gameObject.GetComponent<Renderer>().material = spriteMaterial;
						child.gameObject.GetComponent<Renderer>().material.SetVector("_Area", area);
					}
				}
				SetShaders(child.gameObject);
			}
		}
	}
}
