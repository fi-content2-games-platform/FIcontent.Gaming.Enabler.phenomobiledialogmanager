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
	public class UIChartLine : UIBaseObject
	{
		public float Width = 2;
		public float Height = 2;
		public float Thickness = 0.005f;
		public float PointSize = 0.01f;
		public string Title;
		public string UnitX;
		public string UnitY;
		public ScrollArea scrollArea;

		public enum ChartType { LINE = 0, CURVE };
		public ChartType chartType = ChartType.LINE;

		private MeshFilter meshFilter;
		private MeshRenderer renderer;

		private float maxValue = 0;
		private float minValue = 0;

		private List<Vector3> Vertices;
		private List<Color32> VertexColors;
		private List<int> Triangles;

		private List<Vector3> vertices;

		private Color32[] defaultColors = {
			new Color32(121, 0, 127, 255), new Color32(242, 0, 255, 255),
			new Color32(80, 81, 6, 255), new Color32(160, 162, 12, 255),
			new Color32(0, 95, 69, 255), new Color32(0, 190, 139, 255),
			new Color32(50, 77, 93, 255), new Color32(100, 155, 187, 255),
			new Color32(95, 5, 12, 255), new Color32(190, 10, 24, 255),
			new Color32(102, 78, 0, 255), new Color32(255, 207, 0, 255),
			new Color32(38, 38, 38, 255), new Color32(77, 77, 77, 255),
		};

		private float[,]  values = new float[0,0];
		private Color32[] lineColors = new Color32[0];
		private string[]  lineLabels = new string[0];
		private Color32[] pointColors = new Color32[0];
		private string[]  xLabels = new string[0];

		/// <summary>
		/// Set this true and use "AddLabel" method to use labels with your pie chart
		/// </summary>
		private bool showLabel;
		public	bool ShowLabel
		{
			set { showLabel = value; }
			get { return showLabel; }
		}

		public UIChartLine(GameObject gameObject)
		{
			// Debug.Log("UIChartLine ctor");
			this.gameObject = new GameObject();
			this.gameObject.transform.parent = gameObject.transform;
			this.gameObject.transform.localScale = this.gameObject.transform.parent.transform.localScale; //new Vector3(1, 1, 1);
			this.gameObject.transform.localPosition = Vector3.zero;
			this.gameObject.name = "UIChartLine";

			// Debug.Log("transform:" + this.gameObject.transform);
			// Debug.Log("transform.localScale:" + this.gameObject.transform.localScale);
			// Debug.Log("this.gameObject.transform.parent:" + this.gameObject.transform.parent.localScale);

			this.type = UIType.ChartLine;

			meshFilter = this.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
			renderer.material.shader = Shader.Find("Sprites/Default"); //TODO
			renderer.material.color = Color.white;

			this.SortingOrder = 133;
			showLabel = false;
		}

		float[,] ResizeArray(float[,] original, int rows, int cols)
		{
			var newArray = new float[rows,cols];
			int minRows = rows < original.GetLength(0) ? rows : original.GetLength(0);
			int minCols = cols < original.GetLength(1) ? cols : original.GetLength(1);
			for(int i = 0; i < minRows; i++)
			{
				for(int j = 0; j < minCols; j++)
				{
					newArray[i, j] = original[i, j];
				}
			}
			return newArray;
		}

		T[] ResizeArray<T>(T[] original, int newLength)
		{
			var newArray = new T[newLength];
			int minLength = newLength < original.Length ? newLength	 : original.Length;
			for(int i = 0; i < minLength; i++)
			{
				newArray[i] = original[i];
			}
			return newArray;
		}

		public void SetSize(int rows, int cols)
		{
			values = ResizeArray(values, rows, cols);
			lineColors = ResizeArray(lineColors, rows);
			lineLabels = ResizeArray(lineLabels, rows);
			pointColors = ResizeArray(pointColors, rows);
			xLabels = ResizeArray(xLabels, cols);
		}

		public void SetValue(int row, int col, float val)
		{
			//TODO assert
			values[row, col] = val;
		}
		
		public void SetColor(int row, Color32 color)
		{
			SetColor(row, color, color);
		}

		public void SetColor(int row, Color32 color1, Color32 color2)
		{
			if (lineColors.Length <= row)
				return;
			lineColors[row] = color1;
			pointColors[row] = color2;
		}

		public void SetLabelX(int col, string label)
		{
			//TODO assert
			xLabels[col] = label;
		}

		public void RemoveValues()
		{
			for(int i = 0; i < values.GetLength(0); i++)
			{
				for(int j = 0; j < values.GetLength(1); j++)
				{
					values[i, j] = 0;
				}
			}
		}

		public void SetLineLabel(int row, string label)
		{
			if (lineLabels.Length <= row)
				return;
			lineLabels[row] = label;
		}

		public void Create()
		{
			Vertices = new List<Vector3>();
			VertexColors = new List<Color32>();
			Triangles = new List<int>();

			vertices = new List<Vector3>();

			int rows = values.GetLength(0);
			int cols = values.GetLength(1);

			float w = Width / (float)cols;

			maxValue = float.MinValue;
			minValue = float.MaxValue;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					float val = values[i, j];
					maxValue = val > maxValue ? val : maxValue;
					minValue = val < minValue ? val : minValue;
				}
			}
			
			CreateAxes();

			float left = -Width / 2;
			float top = -Height / 2;

			if (cols > 1)
			{
				if (chartType == ChartType.LINE)
				{
					for (int j = 0; j < rows; j++)
					{
						Color32 color = lineColors[j] != Color.clear ? lineColors[j] : defaultColors[(j % (defaultColors.Length / 2)) * 2];
						for (int i = 0; i < cols - 1; i++)
						{
							float x1 = left + (w * (float)i) + w / 2;
							float x2 = x1 + w; 
							float y1 = top + ((float)values[j, i] / maxValue) * Height;
							float y2 = top + ((float)values[j, i + 1] / maxValue) * Height;

							AddLineSegment(x1, y1, x2, y2, color);
						}
						CloseLine(color);
					}
				}
				else if (chartType == ChartType.CURVE)
				{
					int[, ] segment = new int[cols - 1, 4];;

					if (cols == 2)
					{
						segment[0, 0] = 0; segment[0, 1] = 0; segment[0, 2] = 1; segment[0, 3] = 1;
					}
					else
					{
						for (int i = 0; i < (cols - 1); i++)
						{
							if (i == 0) //first segment
							{
								segment[i, 0] = 0;
								segment[i, 1] = 0;
								segment[i, 2] = 1;
								segment[i, 3] = 2;
							}
							else if (i == cols - 2) //last segment
							{
								segment[i, 0] = cols - 3;
								segment[i, 1] = cols - 2;
								segment[i, 2] = cols - 1;
								segment[i, 3] = cols - 1;
							}
							else
							{
								segment[i, 0] = i - 1;
								segment[i, 1] = i;
								segment[i, 2] = i + 1;
								segment[i, 3] = i + 2;
							}
						}
					}

					for (int j = 0; j < rows; j++)
					{
						Color32 color = lineColors[j] != Color.clear ? lineColors[j] : defaultColors[(j % (defaultColors.Length / 2)) * 2];
						for (int i = 0; i < segment.Length / 4; i++)
						{
							Vector2 c1 = Vector2.zero;
							Vector2 c2 = Vector2.zero;

							Vector2 p0 = new Vector2(left + (w * (float)segment[i, 0]) + w / 2.0f,
								top + ((float)values[j, segment[i, 0]] / maxValue) * Height);
							Vector2 p1 = new Vector2(left + (w * (float)segment[i, 1]) + w / 2.0f,
								top + ((float)values[j, segment[i, 1]] / maxValue) * Height);
							Vector2 p2 = new Vector2(left + (w * (float)segment[i, 2]) + w / 2.0f,
								top + ((float)values[j, segment[i, 2]] / maxValue) * Height);
							Vector2 p3 = new Vector2(left + (w * (float)segment[i, 3]) + w / 2.0f,
								top + ((float)values[j, segment[i, 3]] / maxValue) * Height);
							
							calcControlPoints(p0, p1, p2, p3, out c1, out c2);
							calcLineSegments(p1, c1, c2, p2, color);
						}
						CloseLine(color);
						
					}
				}
			}

			if (PointSize > 0.0f)
			{
				for (int j = 0; j < rows; j++)
				{
					Color32 color = pointColors[j] != Color.clear ? pointColors[j] : defaultColors[(j % (defaultColors.Length / 2)) * 2 + 1];
					for (int i = 0; i < cols; i++)
					{
						Vector2 p0 = new Vector3(left + (w * (float)i) + w / 2,
							top + ((float)values[j, i] / maxValue) * Height, 0);
						CreatePoint(p0, PointSize, color);
					}
				}
			}
					
			Mesh mesh = new Mesh();
			mesh.name = chartType == ChartType.LINE ? "line chart" : "curve chart";
			mesh.vertices = Vertices.ToArray();
			mesh.triangles = Triangles.ToArray();
			mesh.colors32 = VertexColors.ToArray();
			mesh.RecalculateNormals();
			meshFilter.mesh = mesh;
			renderer.material.color = Color.white;

			CreateLabels();

			if (scrollArea != null)
			{
				scrollArea.RefreshContent();
			}
		}

		void CreateAxes()
		{
			float x = -(Width / 2);
			float y = -(Height / 2);

			AddLineSegment(x+Width, y, x, y, Color.black);
			AddLineSegment(x, y, x, y+Height, Color.black);
			CloseLine(Color.black);

			float w = Width / (float)xLabels.Length;
			x += w / 2;
			for (int i = 0; i < xLabels.Length; i++)
			{
				if (xLabels[i] != "")
					AddLineSegment(x, y-0.05f, x, y+0.05f, Color.black);
				CloseLine(Color.black);
				x += w;
			}

			x = -(Width / 2);
			float h = Height / 4;
			for (int i = 0; i < 4; i++)
			{
				y += h;
				AddLineSegment(x-0.05f, y, x+0.05f, y, Color.black);
				CloseLine(Color.black);
			}
		}

		void CreateLabel(string id, string str, Vector3 pos, int size, Color color, int sortingOrder)
		{
			GameObject obj = new GameObject();
			obj.name = id;
			obj.transform.parent = gameObject.transform;
			UIText text = obj.AddComponent<UIText>();
			text.transform.localPosition = pos;
			text.Text = str;
			text.FontColor = color;
			text.FontSize = size;
			// text.Alignment = TextAlignment.Right;
			text.SortingOrder = sortingOrder;
		}

		void CreateLabels()
		{
			float w = Width / (float)xLabels.Length;
			float x = -(Width / 2) + w / 2;
			float y = -(Height / 2) - 0.15f; //TODO

			foreach (Transform child in gameObject.transform)
			{
				Transform.Destroy(child.gameObject);
			}

			CreateLabel("Title", Title, new Vector3(0, Height / 2+ 0.1f, 0), 24, Color.black, 133);
			CreateLabel("UnitX", UnitX, new Vector3(Width / 2 + 0.2f, -Height / 2, 0), 16, Color.black, 133);
			CreateLabel("UnitY", UnitY, new Vector3(-Width / 2, Height / 2+ 0.1f, 0), 16, Color.black, 133);
			for (int i = 0; i < xLabels.Length; i++)
			{
				CreateLabel("LabelX" + i, xLabels[i], new Vector3(x, y, 0), 14, Color.black, 133);
				x += w;
			}


			float max = (maxValue != float.MinValue) ? maxValue : 1;
			for (int i = 0; i < 5; i++)
			{
				CreateLabel("LabelY" + i, string.Format("{0:0.00}", max / 4 * (float)i),
					new Vector3(-(Width / 2) - 0.35f, (-Height / 2) + (Height / 4 * i), 0), 14, Color.black, 133);
			}

			if (showLabel)
			{
				for (int i = 0; i < lineLabels.Length; i++)
				{
					if (lineLabels[i] == "") break;
					
					CreateLabel(
						"LineLabel" + i, 
						lineLabels[i], 
						new Vector3(-Width/2, -Height/2 - 0.3f - (i*0.2f), 0), 
						15, 
						(Color32)lineColors[i], 
						133);
				}
			}
		}

		void calcControlPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 c1, out Vector2 c2)
		{
			float len0 = (p1.x-p0.x) * (p1.x-p0.x) + (p1.y-p0.y) * (p1.y-p0.y);
			float len1 = (p2.x-p1.x) * (p2.x-p1.x) + (p2.y-p1.y) * (p2.y-p1.y);
			float len2 = (p3.x-p2.x) * (p3.x-p2.x) + (p3.y-p2.y) * (p3.y-p2.y);

			float ratio0 = len1 / (len0 + len1);
			float ratio1 = len1 / (len1 + len2);

			c1.x = ((p2.x - p0.x) / 2) * ratio0 + p1.x;
			c1.y = ((p2.y - p0.y) / 2) * ratio0 + p1.y;
			c2.x = p2.x - ((p3.x - p1.x) / 2) * ratio1;
			c2.y = p2.y - ((p3.y - p1.y) / 2) * ratio1;
		}

		void calcLineSegments(Vector2 p0, Vector2 c0, Vector2 c1, Vector2 p1, Color32 color)
		{
			//float len = Mathf.Sqrt((p1.x-p0.x) * (p1.x-p0.x) + (p1.y-p0.y) * (p1.y-p0.y));
			int steps = 12; // (int)(len / Thickness / 5);
			
			float step = 1.0f / (float)(steps - 1);
			float step2 = step * step;
			float step3 = step2 * step;
		
			float ax = 3.0f * (c0.x - c1.x) + p1.x - p0.x;
			float ay = 3.0f * (c0.y - c1.y) + p1.y - p0.y;
			float bx = 3.0f * (p0.x + c1.x) - 6.0f * c0.x;
			float by = 3.0f * (p0.y + c1.y) - 6.0f * c0.y;
			float cx = 3.0f * (c0.x - p0.x);
			float cy = 3.0f * (c0.y - p0.y);
			
			float xdelta = ax * step3 + bx * step2 + cx * step;
			float ydelta = ay * step3 + by * step2 + cy * step;
			float xdelta2 = 6.0f * ax * step3 + 2.0f * bx * step2;
			float ydelta2 = 6.0f * ay * step3 + 2.0f * by * step2;
			float xdelta3 = 6.0f * ax * step3;
			float ydelta3 = 6.0f * ay * step3;
			
			float x = p0.x;
			float y = p0.y;
			for (int i = 0; i < steps - 1; i++)
			{
				float lastX = x;
				float lastY = y;
				x += xdelta;
				xdelta += xdelta2;
				xdelta2 += xdelta3;
				
				y += ydelta;
				ydelta += ydelta2;
				ydelta2 += ydelta3;

				AddLineSegment(lastX, lastY, x, y, color);
			}
		}

		void AddLineSegment(float x1, float y1, float x2, float y2, Color32 color)
		{
			float px = y2 - y1;
			float py = -(x2 - x1);
			float length = Mathf.Sqrt(px * px + py * py);
			
			float nx = (px / length) * Thickness;
			float ny = (py / length) * Thickness;

			Vector3 vert0 = new Vector3(x1 + nx, y1 + ny, 0);
			Vector3 vert1 = new Vector3(x1 - nx, y1 - ny, 0);
			Vector3 vert2 = new Vector3(x2 + nx, y2 + ny, 0);
			Vector3 vert3 = new Vector3(x2 - nx, y2 - ny, 0);

			if (vertices.Count == 0)
			{
				vertices.Add(vert0);
				vertices.Add(vert1);
				vertices.Add(vert2);
				vertices.Add(vert3);
				return;
			}

			Vector3 vertP0 = vertices[vertices.Count - 4];
			Vector3 vertP1 = vertices[vertices.Count - 3];
			Vector3 vertP2 = vertices[vertices.Count - 2];
			Vector3 vertP3 = vertices[vertices.Count - 1];

			bool intersect = intersectLineLine(vertP0, vertP2, vert0, vert2) | intersectLineLine(vertP1, vertP3, vert1, vert3);
			if ((length < Thickness * 2) || badAngle(vertP0, vertP2, vert0, vert2) || !intersect)
			{

				CloseLine(color);
				vertices.Add(vert0);
				vertices.Add(vert1);
				vertices.Add(vert2);
				vertices.Add(vert3);
				return;
			}

			vertices[vertices.Count - 2] = lineIntersectionPoint(vertP0, vertP2, vert0, vert2);
			vertices[vertices.Count - 1] = lineIntersectionPoint(vertP1, vertP3, vert1, vert3);
			vertices.Add(vert2);
			vertices.Add(vert3);
		}

		bool intersectLineLine(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			if (v0 == v2 || v0 == v3 || v1 == v2 || v1 == v3)
				return false;

			float u_b  = (v3.y - v2.y) * (v1.x - v0.x) - (v3.x - v2.x) * (v1.y - v0.y);
			if (u_b == 0)
				return false;
			
			float ua = ((v3.x - v2.x) * (v0.y - v2.y) - (v3.y - v2.y) * (v0.x - v2.x)) / u_b;
			float ub = ((v1.x - v0.x) * (v0.y - v2.y) - (v1.y - v0.y) * (v0.x - v2.x)) / u_b;

			if (0.0 <= ua && ua <= 1.0 && 0.0 <= ub && ub <= 1.0)
				return true;
			return false;
		}

		double angleBetween2Lines(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			double angle1 = Mathf.Atan2(v0.y - v1.y, v0.x - v1.x);
			double angle2 = Mathf.Atan2(v2.y - v3.y, v2.x - v3.x);
			return (angle1-angle2) * Mathf.Rad2Deg;;
		}

		bool badAngle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			double angle = Mathf.Abs((float)angleBetween2Lines(v0, v1, v2, v3));
			
			if ((angle > 175 && angle < 195))
			{
				Debug.Log("bad angle: " + angle);
				return true;
			}
			return false;
		}

		Vector3 lineIntersectionPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			Vector3 ret = new Vector3();
			float distAB, theCos, theSin, newX, ABpos;

			v1.x-=v0.x; v1.y-=v0.y;
			v2.x-=v0.x; v2.y-=v0.y;
			v3.x-=v0.x; v3.y-=v0.y;

			distAB = Mathf.Sqrt(v1.x * v1.x + v1.y * v1.y);

			theCos = v1.x / distAB;
			theSin = v1.y / distAB;
			newX = v2.x * theCos + v2.y * theSin;
			v2.y = v2.y * theCos - v2.x * theSin;
			v2.x = newX;
			newX = v3.x * theCos + v3.y * theSin;
			v3.y = v3.y * theCos - v3.x * theSin;
			v3.x = newX;

			ABpos=v3.x+(v2.x-v3.x)*v3.y/(v3.y-v2.y);

			ret.x=v0.x + ABpos * theCos;
			ret.y=v0.y + ABpos * theSin;
			ret.z = 0;

			return ret; 
		}

		void CloseLine(Color32 color)
		{
			if (vertices.Count == 0)
				return;

			int offset = Vertices.Count;			
			int triangleCount = vertices.Count - 2;
			int[] triangles = new int[triangleCount * 3];
			for (int i = 0; i < triangleCount / 2; i++)
			{
				triangles[i*6] = offset + i*2;
				triangles[i*6+1] = offset + i*2+2;
				triangles[i*6+2] = offset + i*2+1;
				
				triangles[i*6+3] = offset + i*2+2;
				triangles[i*6+4] = offset + i*2+3;
				triangles[i*6+5] = offset + i*2+1;
			}

			for (int i = 0; i < vertices.Count; i++)
			{
				VertexColors.Add(color);
			}
			
			Triangles.AddRange(triangles);
			Vertices.AddRange(vertices);
			
			vertices.Clear();
		}

		void CreatePoint(Vector3 center, float r, Color32 color)
		{
			Vector3 p1 = new Vector3(r, 0, 0);

			for(int i = 0; i < 360 / 30; i++)
			{
				Vector3 p = p1;	
				
				p1 = Quaternion.Euler(0, 0, 30) * p1;
				Vertices.Add(center);
				Vertices.Add(center + p);
				Vertices.Add(center + p1);
				VertexColors.Add(color);
				VertexColors.Add(color);
				VertexColors.Add(color);
				Triangles.Add(Vertices.Count - 3);
				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 1);
			}
		}
	} //class UIChartLine

} //namespace Epigene.UI
