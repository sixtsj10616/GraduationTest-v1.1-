using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraToCenter : MonoBehaviour
{

	public GameObject target;//the target object
	public float dragSpeedMod = 10.0f;//a speed modifier
	public float roomInSpeedMod = 20.0f;//a speed modifier

	public bool clampXY = false;
	public float minDistance = 5.0f;
	public float maxDistance = 300.0f;


	private Vector3 targetPoint;//the coord to the point where the camera looks at
	private float x = 0;
	private float y = 0;
	private int isClamp = 0;
	private bool isRotating = false;
	public float distanceToTarget;
	public float oriDistanceToTarget;
	
	void Start()
	{//Set up things on the start method

		if (target) targetPoint = target.transform.position;//get target's coords
		transform.LookAt(targetPoint);//makes the camera look to it
		distanceToTarget = oriDistanceToTarget = Vector3.Distance(targetPoint, transform.position);
	}

	void Update()
	{//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
		
			if (Input.GetMouseButtonDown(0) && !isRotating) isRotating = true;

			if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
			{
				distanceToTarget = Vector3.Distance(targetPoint, transform.position);
				float desiredDistance = distanceToTarget;
				float offset = (-Mathf.Sign(Input.GetAxisRaw("Mouse ScrollWheel")) * roomInSpeedMod * Time.smoothDeltaTime);
				desiredDistance += offset;
				desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

				transform.position = targetPoint - (transform.forward * desiredDistance);

			}
		if (isRotating)
		{
			if (clampXY)
			{
				if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Mouse Y")) && isClamp != -1)
				{
					isClamp = 1;
					x += Input.GetAxis("Mouse X") * dragSpeedMod * Time.smoothDeltaTime;

					transform.RotateAround(targetPoint, target.transform.up, x);

				}
				else if (Mathf.Abs(Input.GetAxis("Mouse X")) < Mathf.Abs(Input.GetAxis("Mouse Y")) && isClamp != 1)
				{
					isClamp = -1;

					y += Input.GetAxis("Mouse Y") * dragSpeedMod * Time.smoothDeltaTime;
					transform.RotateAround(targetPoint, -transform.right, y);
				}
			}
			else
			{
				x += Input.GetAxis("Mouse X") * dragSpeedMod * Time.smoothDeltaTime;
				y += Input.GetAxis("Mouse Y") * dragSpeedMod * Time.smoothDeltaTime;
				transform.RotateAround(targetPoint, target.transform.up, x);
				transform.RotateAround(targetPoint, -transform.right, y);
			}

		}
		if (Input.GetMouseButtonUp(0))
		{
			x = 0; y = 0;
			isClamp = 0;
			isRotating = false;
		}
	}
}