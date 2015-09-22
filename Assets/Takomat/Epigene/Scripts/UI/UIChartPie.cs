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

namespace Epigene.UI
{
	public class UIChartPie : UIBaseObject
	{		
		public ScrollArea scrollArea;

		private MeshFilter meshFilter;
		private MeshRenderer renderer;

		private List<Vector3> Vertices;
		private List<Vector2> UV;
		private List<Color32> Colors;
		private List<int> Triangles;

		private Color32[] defaultColors = {
			new Color32(121, 0, 127, 255), new Color32(242, 0, 255, 255),
			new Color32(80, 81, 6, 255), new Color32(160, 162, 12, 255),
			new Color32(0, 95, 69, 255), new Color32(0, 190, 139, 255),
			new Color32(50, 77, 93, 255), new Color32(100, 155, 187, 255),
			new Color32(95, 5, 12, 255), new Color32(190, 10, 24, 255),
			new Color32(102, 78, 0, 255), new Color32(255, 207, 0, 255),
			new Color32(38, 38, 38, 255), new Color32(77, 77, 77, 255),
		};

		private float factor = 1.0f;
		public	float Factor{
			get { return factor; }
			set { factor = value; }
		}
		public float Width = 2;
		public float Height = 2;
		public string Title;
		private List<Dictionary<string, object>> datas;

		/// <summary>
		/// Set this true and use "AddLabel" method to use labels with your pie chart
		/// </summary>
		private bool showLabel;
		public	bool ShowLabel
		{
			set { showLabel = value; }
			get { return showLabel; }
		}
		
		public UIChartPie(GameObject gameObject)
		{
			Debug.Log("UIChartPie ctor");
			this.gameObject = new GameObject();
			this.gameObject.transform.parent = gameObject.transform;
			this.gameObject.transform.localScale = this.gameObject.transform.parent.transform.localScale; //new Vector3(1, 1, 1);
			this.gameObject.transform.localPosition = Vector3.zero;
			this.gameObject.name = "UIChartPie";

			Debug.Log("transform:" + this.gameObject.transform);
			Debug.Log("transform.localScale:" + this.gameObject.transform.localScale);
			Debug.Log("this.gameObject.transform.parent:" + this.gameObject.transform.parent.localScale);

			this.type = UIType.ChartPie;

			meshFilter = this.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
			renderer.material.shader = Shader.Find("Sprites/Default"); //TODOSHADER ScrollAreaMesh
			renderer.material.color = Color.white;

			this.SortingOrder = 133;
			datas = new List<Dictionary<string, object>>();
			showLabel = false;
		}
		
		public void SetColor(int index, Color32 color1, Color32 color2)
		{
			//TODO Assert
			if (index < datas.Count)
			{
				Dictionary<string, object> data = datas[index];
				data["color1"] = color1;
				data["color2"] = color2;
				datas[index] = data;
			}
		}

		public void AddValues(float[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				Dictionary<string, object> data = new Dictionary<string, object>();
				data["value"] = values[i];
				datas.Add(data);
			}
		}

		public void SetValue(int index, float value)
		{
			if (index < datas.Count)
			{
				Dictionary<string, object> data = datas[index];
				data["value"] = value;
				datas[index] = data;
			}
		}
		
		public float GetValue(int index)
		{
			if (index < datas.Count)
			{
				Dictionary<string, object> data = datas[index];
				float value = (float)data["value"];
				return value;
			}
			return -1f;
		}

		public void AddValue(float val)
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			data["value"] = val;
			datas.Add(data);
		}

		public void SetLabel(int index, string newLabel)
		{
			if (index < datas.Count)
			{
				Dictionary<string, object> data = datas[index];
				data["label"] = newLabel;
				datas[index] = data;
			}
		}

		public void RemoveValues()
		{
			datas.Clear();
		}

		void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color32 color1, Color32 color2)
		{
			Vertices.Add(p1);
			Vertices.Add(p2);
			Vertices.Add(p3);
			UV.Add(new Vector2(p1.x, p1.y)); //TODO: new shader
			UV.Add(new Vector2(p2.x, p2.y));
			UV.Add(new Vector2(p3.x, p3.y));
			Colors.Add(color1);
			Colors.Add(color2);
			Colors.Add(color2);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 2);
			Triangles.Add(Vertices.Count - 1);
		}

		private float Angle(Vector3 a, Vector3 b)
		{
			float angle = (Mathf.Atan2(a.y, a.x) - Mathf.Atan2(b.y, b.x)) * Mathf.Rad2Deg;
			if (angle < 0.0f)
				angle += 360.0f;

			return angle;
		}

		public void Create()
		{
			Vertices = new List<Vector3>();
			UV = new List<Vector2>();
			Colors = new List<Color32>();
			Triangles = new List<int>();

			float total = 0.0f;
			for (int i = 0; i < datas.Count; i++)
			{
				total += (float)datas[i]["value"];
			}

			float percent = 0.0f;
			for (int i = 0; i < datas.Count; i++)
			{
				Color32 color1 = datas[i].ContainsKey("color1") ? (Color32)datas[i]["color1"] : defaultColors[(i % (defaultColors.Length / 2)) * 2];
				Color32 color2 = datas[i].ContainsKey("color2") ? (Color32)datas[i]["color2"] : defaultColors[(i % (defaultColors.Length / 2)) * 2 + 1];

				float rad = percent * Mathf.PI * 2;
				Vector3 p1 = new Vector3(Mathf.Sin(rad) * factor, Mathf.Cos(rad) * factor, 0.0f);

				float val = (float)datas[i]["value"];

				percent += val / total;
				datas[i]["percent"] = val / total;;
				rad = percent * Mathf.PI * 2;
				Vector3 p2 = new Vector3(Mathf.Sin(rad) * factor, Mathf.Cos(rad) * factor, 0.0f);
				
				float angle = (val == total) ? 360.0f : Angle(p1, p2);
		
				while (angle > 5.0f)
				{
					Vector3 p = p1;
					
					p1 = Quaternion.Euler(0, 0, -5) * p1;

					AddTriangle(Vector3.zero, p, p1, color1, color2);
					
					angle = Angle(p1, p2);
				}
				if (angle <= 5.0f)
				{

					AddTriangle(Vector3.zero, p1, p2, color1, color2);
				}
			}

			Mesh mesh = new Mesh();
			mesh.name = "pie chart";
			mesh.vertices = Vertices.ToArray();
			mesh.uv = UV.ToArray(); //TODO: shader
			mesh.triangles = Triangles.ToArray();
			mesh.colors32 = Colors.ToArray();
			mesh.RecalculateNormals();
			meshFilter.mesh = mesh;
			//renderer.material.shader = Shader.Find("Sprites/Default");
			//renderer.material.color = Color.white;

			if (showLabel)
				CreateLabels();

			if (scrollArea != null)
			{
				scrollArea.RefreshContent();
			}
		}

		void CreateLabels()
		{
			float y = 0; //TODO
			
			foreach (Transform child in gameObject.transform)
			{
				if (child.name.StartsWith("PieLabel")
				    || child.name == "Title")
				{
					Transform.Destroy(child.gameObject);
				}
			}

			if (Title != "")
				CreateLabel(
					"Title", 
					Title, 
					new Vector3(0, Height / 2+ 0.15f, 0), 
					24, 
					Color.black, 
					133, 
					TextAlignment.Center, 
					TextAnchor.MiddleCenter);

			for (int i = 0; i < datas.Count; i++)
			{
				if (!datas[i].ContainsKey("label")) break;

				CreateLabel(
					"PieLabel" + i, 
					(string) datas[i]["label"] + ": ", 
					new Vector3(-Width/2, -Height/2 - ((i+1)*0.15f), 0), 
					15, 
					(Color32)datas[i]["color1"], 
					133, 
					TextAlignment.Left, 
					TextAnchor.UpperLeft);

				CreateLabel(
					"PieLabelValue" + i, 
					((float)datas[i]["value"]).ToString("N2") + " (" + (100.0f * (float)datas[i]["percent"]).ToString("00.00") + "%)", 
					new Vector3(Width/2, -Height/2 - ((i+1)*0.15f), 0), 
					15, 
					Color.black,  
					133, 
					TextAlignment.Right, 
					TextAnchor.UpperRight);
			}
		}

		void CreateLabel(string id, string str, Vector3 pos, int size, Color color, int sortingOrder, TextAlignment alignement, TextAnchor anchor)
		{
			GameObject obj = new GameObject();
			obj.name = id;
			obj.transform.parent = gameObject.transform;
			UIText text = obj.AddComponent<UIText>();
			text.transform.localPosition = pos;
			text.Text = str;
			text.FontColor = color;
			text.FontSize = size;
#if !EPIGENE_UI_46 //TODO
			text.Alignment = alignement;
			text.Anchor	= anchor;
#endif
			text.SortingOrder = sortingOrder;
		}

	} //class UIChartPie

} //namespace Epigene.UI
