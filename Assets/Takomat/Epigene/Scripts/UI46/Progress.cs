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
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(Progress))]
public class ProgressInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		Progress p = (Progress) target;
		if(GUILayout.Button("Set Native Size"))
		{
			p.SetNativeSize();
		}
	}

}
#endif

public class Progress : Graphic {
	
	public Sprite sprite;
	[Range(0,1)]
	public float percent = 1.0f;

	public override Texture mainTexture
	{
		get
		{
			return sprite == null ? s_WhiteTexture : sprite.texture;
		}
	}

	protected override void OnFillVBO(List<UIVertex> vbo)
	{
		vbo.Clear();
		float x0 = -rectTransform.rect.width / 2;
		float y0 = rectTransform.rect.height / 2;
		float p = rectTransform.rect.width * percent;

		UIVertex vert = UIVertex.simpleVert;
		Vector4 uv = new Vector4(0, 0, 1, 1);
		// Vector4 uv = (sprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;
		// Vector4 uv = (sprite != null) ? UnityEngine.Sprites.DataUtility.GetInnerUV(sprite) : Vector4.zero;

		vert.color = color;
		vert.position = new Vector3(x0, y0);
		vert.uv0 = new Vector2(uv.x, uv.w);
		vbo.Add(vert);

		vert.position = new Vector3(x0 + p, y0);
		vert.uv0 = new Vector2((uv.z - uv.x) * percent, uv.w);
		vbo.Add(vert);

		vert.position = new Vector3(x0 + p, y0 - rectTransform.rect.height);
		vert.uv0 = new Vector2((uv.z - uv.x) * percent, uv.y);
		vbo.Add(vert);

		vert.position = new Vector3(x0, y0 - rectTransform.rect.height);
		vert.uv0 = new Vector2(uv.x, uv.y);
		vbo.Add(vert);
	}

	public override void SetNativeSize()
	{
		if (sprite != null)
		{
			float w = sprite.rect.width / pixelsPerUnit;
			float h = sprite.rect.height / pixelsPerUnit;
			rectTransform.anchorMax = rectTransform.anchorMin;
			rectTransform.sizeDelta = new Vector2(w, h);
			SetAllDirty();
		}
	}
	
	public float pixelsPerUnit
	{
		get
		{
			float spritePixelsPerUnit = 100;
			if (sprite)
				spritePixelsPerUnit = sprite.pixelsPerUnit;

			float referencePixelsPerUnit = 100;
			if (canvas)
				referencePixelsPerUnit = canvas.referencePixelsPerUnit;

			return spritePixelsPerUnit / referencePixelsPerUnit;
		}
	}
}
