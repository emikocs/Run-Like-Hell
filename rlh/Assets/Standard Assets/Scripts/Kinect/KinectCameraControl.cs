using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text; 
using System.Runtime.InteropServices;

public class KinectCameraControl : MonoBehaviour {

	public NiteWrapper.SkeletonJoint ControlJoint;
	public NiteWrapper.SkeletonJoint TranslationControlJoint;
	public float MovementThreshold= 2f;
	public float Smoothing = 0.0005f;	
	
	private bool walking = false;
	private int timer_cnt = 0;
	
	public int wip_timer = 5;
	public double wip_thresh = 0.0f;
	
	private bool turn_the_ship_around = false;

	Vector3 previousPosition = Vector3.zero;
	
	Kinect kin;
	Quaternion originalOrientation;
	
	// Use this for initialization
	void Start () {
	
		originalOrientation = transform.rotation;
		
		GameObject kinectContainer = GameObject.Find("Kinect");
		
		if(kinectContainer != null)
			kin = kinectContainer.GetComponent<Kinect>();
		
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log("Camera control updated");

		if(kin == null || kin.users.Count < 1) return;
				
		KinectUser kUser = kin.users[0];
		NiteWrapper.SkeletonJointTransformation trans = kin.getTransform(kUser.ID,ControlJoint);
		
		if(trans.ori.confidence > 0.5)
		{
			
			Vector3 kZ = new Vector3(trans.ori.m02, -trans.ori.m12, trans.ori.m22);
            Vector3 kY = new Vector3(-trans.ori.m01, trans.ori.m11, -trans.ori.m21);
            Quaternion newRotation = Quaternion.LookRotation(kZ, kY);
			
			
			//Quaternion Rotation180 = Quaternion.Euler(0,180,0);
			
			//newRotation = newRotation * Rotation180;
			
            //Quaternion newRotation = jointRotation * initialRotations[(int)joint];
			//newRotation = newRotation * originalOrientation;
			
            // Some smoothing
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 20);
			//De-roll
			transform.Rotate(0,0,-transform.eulerAngles.z);
				//transform.rotation = jointRotation;
		}
		
		trans = kin.getTransform(kUser.ID,TranslationControlJoint);
		NiteWrapper.SkeletonJointTransformation trans2;
		
		if(trans.pos.confidence > 0.5)
		{
			Vector3 newControlJointPos = new Vector3(trans.pos.x,trans.pos.y,trans.pos.z);
			Vector3 offset = (newControlJointPos - previousPosition) * Smoothing;
			
			if(offset.sqrMagnitude <= MovementThreshold)
			{
				Vector3 movement = Vector3.zero;
				movement  = movement + Camera.main.transform.forward * -offset.z;
				movement  = movement + Camera.main.transform.right * -offset.x;
				movement  = movement + Camera.main.transform.up * offset.y;
				
				
				transform.position += movement;
				Debug.Log("Offset = " + offset);
				previousPosition = newControlJointPos;
			}
		}
		
		
		if(timer_cnt > 0)
		{
			timer_cnt--;
		}
		
		if(timer_cnt <= 0)
		{
			walking = false;
		}
		
		if(walking)
		{
			Vector3 movement = Vector3.zero;
			movement  = movement + Camera.main.transform.forward * 0.3f;
			transform.position += movement;
		}
		
		trans = kin.getTransform(kUser.ID,NiteWrapper.SkeletonJoint.LEFT_KNEE);
		trans2 = kin.getTransform(kUser.ID,NiteWrapper.SkeletonJoint.RIGHT_KNEE);
		if(trans.pos.confidence > 0.2)
		{
			//Can we do better logic here?  You betcha
			//Check that BOTH knees are not up (jumping)
			if(!(trans.pos.y > wip_thresh && trans2.pos.y > wip_thresh))
			{
				//If they are not both up check if one is
				if(trans.pos.y > wip_thresh || trans2.pos.y > wip_thresh)
				{
					walking = true;
					timer_cnt = wip_timer;
				}
			}
		}
		
	}
}
