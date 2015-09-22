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

public class RotateConstruction : MonoBehaviour
{
	private bool zoom = false;
	public GameObject[] objects;
	
	float rotationYAxis = 0.0f;
	float rotationXAxis = 0.0f;
	float velocityX = 0.0f;
	float velocityY = 0.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = 0.0f;
	public float yMaxLimit = 360.0f;
	
	public float smoothTime = 2f;
	GameObject cameraVirtualStudio = null;
	Vector3 cameraStartPos = Vector3.zero;
	Quaternion cameraStartRot;
	Vector3 cameraNormal = Vector3.zero;
	float cameraPosition = 0;
	float cameraTargetPosition = 0;
	
	void Start()
	{
		cameraVirtualStudio = GameObject.Find("Epigene/CameraVirtualStudio");
		cameraStartPos = cameraVirtualStudio.transform.localPosition;
		cameraStartRot = cameraVirtualStudio.transform.rotation;
		cameraNormal = cameraVirtualStudio.transform.forward;
		cameraNormal.Normalize();
	}
	
	void FixedUpdate()
	{
		if (zoom)
		{
			if (Input.GetMouseButton(0))
			{
				velocityX -= xSpeed * Input.GetAxis("Mouse X") * 0.002f;
				velocityY -= ySpeed * Input.GetAxis("Mouse Y") * 0.002f;
			}
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			
			Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
			                                           transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler(0, rotationYAxis, 0);
			
			transform.rotation = toRotation;
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
			
			//TODO: touchscreen pinch zoom
			//http://unity3d.com/learn/tutorials/modules/beginner/platform-specific/pinch-zoom
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				cameraTargetPosition -= 0.4f;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				cameraTargetPosition += 0.4f;
			}
			
			cameraTargetPosition = Mathf.Min(cameraTargetPosition, 2.2f);
			cameraTargetPosition = Mathf.Max(cameraTargetPosition, -0.5f);
			
			
			rotationXAxis = Mathf.Min(rotationXAxis, 20f);
			rotationXAxis = Mathf.Max(rotationXAxis, -40f);
			
			cameraPosition = Mathf.Lerp(cameraPosition, cameraTargetPosition, Time.deltaTime * smoothTime);
			cameraVirtualStudio.transform.localPosition = cameraStartPos + cameraNormal * cameraPosition;
			
			Vector3 target = renderer.bounds.center;
			target.y += 0.5f;
			cameraVirtualStudio.transform.RotateAround(target, Vector3.left, rotationXAxis);
			cameraVirtualStudio.transform.LookAt(target);
		}
		else
		{
			transform.Rotate(Vector3.up * Time.deltaTime * (-xSpeed / 3));
		}
	}
	
	public void OnZoom()
	{
		zoom = !zoom;
		
		Vector3 pos = transform.position;
		Vector3 scale = Vector3.zero;
		if (zoom)
		{
			pos.y = -3.96f;
			scale = new Vector3(0.45f, 0.45f, 0.45f);
			cameraVirtualStudio.transform.LookAt(renderer.bounds.center);
		}
		else
		{
			pos.y = -1.0f;
			scale = new Vector3(0.2f, 0.2f, 0.2f);
		}
		transform.position = pos;
		transform.localScale = scale;
		cameraVirtualStudio.transform.localPosition = cameraStartPos;
		cameraVirtualStudio.transform.rotation = cameraStartRot;
		foreach(GameObject obj in objects)
		{
			obj.SetActive(!zoom);
		}
	}
}
