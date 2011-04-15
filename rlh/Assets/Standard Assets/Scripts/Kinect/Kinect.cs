using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text; 
using System.Runtime.InteropServices;

public class KinectUser
{
	public uint ID {get; set;}
	public bool calibrationCompleted {get; set;}

	public KinectUser(uint uid)
	{
		ID = uid;
		calibrationCompleted = false;
	}
}

public class Kinect : MonoBehaviour {

	public List<KinectUser> users;
	NiteWrapper.UserDelegate NewUser;
    NiteWrapper.UserDelegate CalibrationStarted;
    NiteWrapper.UserDelegate CalibrationFailed;
    NiteWrapper.UserDelegate CalibrationSuccess;
    NiteWrapper.UserDelegate UserLost;
	
	public bool DisplayPosition = false;
	public bool DisplayOrientation = true;
	
	// Use this for initialization
	void Start () {
		
		users = new List<KinectUser>();
		
		uint test = NiteWrapper.Init(new StringBuilder(".\\OpenNI.xml"));
		if (test != 0)
        {
            Debug.Log(String.Format("Error initing OpenNI: {0}", Marshal.PtrToStringAnsi(NiteWrapper.GetStatusString(test))));
        }
		
		// init user callbacks
        NewUser = new NiteWrapper.UserDelegate(OnNewUser);
        CalibrationStarted = new NiteWrapper.UserDelegate(OnCalibrationStarted);
        CalibrationFailed = new NiteWrapper.UserDelegate(OnCalibrationFailed);
        CalibrationSuccess = new NiteWrapper.UserDelegate(OnCalibrationSuccess);
        UserLost = new NiteWrapper.UserDelegate(OnUserLost);

        // Start looking
        NiteWrapper.StartLookingForUsers(NewUser, CalibrationStarted, CalibrationFailed, CalibrationSuccess, UserLost);
		Debug.Log("Waiting for users to calibrate");
		
		// set default smoothing
		NiteWrapper.SetSkeletonSmoothing(0.6);
	}
	
	// Update is called once per frame
	void Update () 
	{
		NiteWrapper.Update(false);
	}
	
	public NiteWrapper.SkeletonJointTransformation getTransform(uint user_id, NiteWrapper.SkeletonJoint jointType)
	{
			NiteWrapper.SkeletonJointTransformation transformation = new NiteWrapper.SkeletonJointTransformation();
			NiteWrapper.GetJointTransformation(user_id, jointType, ref transformation);
			return transformation;
	}
	
	void OnGUI()
	{
		if(users.Count == 0) return;
		
		KinectUser user = users[0];
		
		Rect pos = new Rect(30,30,300,30);
		
		GUI.Label(pos, statusMsg);
		
		if(!user.calibrationCompleted) return;
		
		if(DisplayPosition)
			displayPositions(user);
		else if(DisplayOrientation)
			displayOrientations(user);
	}
	
	private void displayOrientations(KinectUser user)
	{
		Rect pos;
		NiteWrapper.SkeletonJointTransformation tr;

		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.HEAD);
		pos = new Rect(30,60,300,70);
		GUI.Label(pos, String.Format("Head\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.TORSO_CENTER);
		pos = new Rect(30,130,300,70);
		GUI.Label(pos, String.Format("Torso\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_HIP);
		pos = new Rect(30,200,300,70);
		GUI.Label(pos, String.Format("Left Hip\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_SHOULDER);
		pos = new Rect(30,270,300,70);
		GUI.Label(pos, String.Format("Left Shoulder\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22));

        Quaternion q = new Quaternion();
        q.w = (float)Math.Sqrt(1.0 + tr.ori.m00 + tr.ori.m11 + tr.ori.m22) / 2.0f;
        double w4 = (4.0 * q.w);
        q.x = (float)((tr.ori.m21 - tr.ori.m12) / w4);
        q.y = (float)((tr.ori.m02 - tr.ori.m20) / w4);
        q.z = (float)((tr.ori.m10 - tr.ori.m01) / w4);
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_HIP);
		pos = new Rect(30,340,300,80);
		GUI.Label(pos, String.Format("Right Hip\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}\n{9:0.00},{10:0.00},{11:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22,
            q.eulerAngles.x,q.eulerAngles.y,q.eulerAngles.z));

		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_SHOULDER);
		pos = new Rect(30,420,300,70);
		GUI.Label(pos, String.Format("Right Shoulder\n{0:0.00},{1:0.00},{2:0.00}\n{3:0.00},{4:0.00},{5:0.00}\n{6:0.00},{7:0.00},{8:0.00}" , tr.ori.m00, tr.ori.m01,tr.ori.m02, tr.ori.m10, tr.ori.m11,tr.ori.m12,tr.ori.m20, tr.ori.m21,tr.ori.m22));
	}
	
	private void displayPositions(KinectUser user)
	{
		Rect pos;
		NiteWrapper.SkeletonJointTransformation tr;
		GUIText caption;
		
		//caption = GameObject.Find("GUI Text").guiText;
		//caption.text =  String.Format("Head : {0:0.00},{1:0.00},{2:0.00}" , tr.pos.x, tr.pos.y,tr.pos.z);	
		
		/*if(tr.pos.y > 1000.0f)
		{
			caption.text = String.Format("Jump!!!!");
		}*/
        tr = getTransform(user.ID, NiteWrapper.SkeletonJoint.HEAD);		
		pos = new Rect(30,60,300,30);
		GUI.Label(pos, String.Format("Head!! : {0:0.00},{1:0.00},{2:0.00}" , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.NECK);
		pos = new Rect(30,90,300,30);
		GUI.Label(pos, String.Format("Neck : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.TORSO_CENTER);
		pos = new Rect(30,120,300,30);
		GUI.Label(pos, String.Format("Torso Center : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));

		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_SHOULDER);
		pos = new Rect(30,150,300,30);
		GUI.Label(pos, String.Format("Left Shoulder : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_ELBOW);
		pos = new Rect(30,180,300,30);
		GUI.Label(pos, String.Format("Left Elbow : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_HAND);
		pos = new Rect(30,210,300,30);
		GUI.Label(pos, String.Format("Left Hand : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_SHOULDER);
		pos = new Rect(30,240,300,30);
		GUI.Label(pos, String.Format("Right Shoulder : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_ELBOW);
		pos = new Rect(30,270,300,30);
		GUI.Label(pos, String.Format("Right Elbow : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_HAND);
		pos = new Rect(30,300,300,30);
		GUI.Label(pos, String.Format("Right Hand : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.WAIST);
		pos = new Rect(30,330,300,30);
		GUI.Label(pos, String.Format("Waist : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_HIP);
		pos = new Rect(30,360,300,30);
		GUI.Label(pos, String.Format("Left Hip : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_KNEE);
		pos = new Rect(30,390,300,30);
		GUI.Label(pos, String.Format("Left Knee : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.LEFT_FOOT);
		pos = new Rect(30,420,300,30);
		GUI.Label(pos, String.Format("Left Foot : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_HIP);
		pos = new Rect(30,450,300,30);
		GUI.Label(pos, String.Format("Right Hip : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_KNEE);
		pos = new Rect(30,480,300,30);
		GUI.Label(pos, String.Format("Right Knee: {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
		
		tr = getTransform(user.ID,NiteWrapper.SkeletonJoint.RIGHT_FOOT);
		pos = new Rect(30,510,300,30);
		GUI.Label(pos, String.Format("Right Foot : {0:0.00},{1:0.00},{2:0.00} " , tr.pos.x, tr.pos.y,tr.pos.z));
	}
	
	
	public string getName(NiteWrapper.SkeletonJoint jointType)
	{
		switch(jointType)
		{
			case NiteWrapper.SkeletonJoint.HEAD:
				return "Head";
			case NiteWrapper.SkeletonJoint.NECK:
				return "Neck";
			case NiteWrapper.SkeletonJoint.TORSO_CENTER:
				return "Torso Center";
			case NiteWrapper.SkeletonJoint.WAIST:
				return "Waist";

			case NiteWrapper.SkeletonJoint.LEFT_COLLAR:
				return "Left Collar";
			case NiteWrapper.SkeletonJoint.LEFT_SHOULDER:
				return "Left Shoulder";
			case NiteWrapper.SkeletonJoint.LEFT_ELBOW:
				return "Left Elbow";
			case NiteWrapper.SkeletonJoint.LEFT_WRIST:
				return "Left Wrist";
			case NiteWrapper.SkeletonJoint.LEFT_HAND:
				return "Left Hand";
			case NiteWrapper.SkeletonJoint.LEFT_FINGERTIP:
				return "Left Fingertip";

			case NiteWrapper.SkeletonJoint.RIGHT_COLLAR:
				return "Right Collar";
			case NiteWrapper.SkeletonJoint.RIGHT_SHOULDER:
				return "Right Shoulder";
			case NiteWrapper.SkeletonJoint.RIGHT_ELBOW:
				return "Right Elbow";
			case NiteWrapper.SkeletonJoint.RIGHT_WRIST:
				return "Right Wrist";
			case NiteWrapper.SkeletonJoint.RIGHT_HAND:
				return "Right Hand";
			case NiteWrapper.SkeletonJoint.RIGHT_FINGERTIP:
				return "Right Fingertip";

			case NiteWrapper.SkeletonJoint.LEFT_HIP:
				return "Left Hip";
			case NiteWrapper.SkeletonJoint.LEFT_KNEE:
				return "Left Knee";
			case NiteWrapper.SkeletonJoint.LEFT_ANKLE:
				return "Left Ankle";
			case NiteWrapper.SkeletonJoint.LEFT_FOOT:
				return "Left Foot";

			case NiteWrapper.SkeletonJoint.RIGHT_HIP:
				return "Right Hip";
			case NiteWrapper.SkeletonJoint.RIGHT_KNEE:
				return "Right Knee";
			case NiteWrapper.SkeletonJoint.RIGHT_ANKLE:
				return "Right Ankle";
			case NiteWrapper.SkeletonJoint.RIGHT_FOOT:
				return "Right Foot";
		}
		
		return "Unknown";
	}
	
	
	void OnApplicationQuit()
	{
		NiteWrapper.Shutdown();
	}
	
	//NITE callback functions
	
	string statusMsg = "";
	 void OnNewUser(uint UserId)
    {
		statusMsg = String.Format("[{0}] New user", UserId);
        Debug.Log(statusMsg);
		
		KinectUser kUser = new KinectUser(UserId);
        users.Add(kUser);
    }   

    void OnCalibrationStarted(uint UserId)
    {
		statusMsg = String.Format("[{0}] Calibration started", UserId);
		Debug.Log(statusMsg);
    }

    void OnCalibrationFailed(uint UserId)
    {
		statusMsg = String.Format("[{0}] Calibration failed", UserId);
        Debug.Log(statusMsg);
		
		foreach (KinectUser kUser in users)
			if(kUser.ID == UserId)
				kUser.calibrationCompleted = false;
    }

    void OnCalibrationSuccess(uint UserId)
    {
		statusMsg = String.Format("[{0}] Calibration success", UserId);
        Debug.Log(statusMsg);
		
		foreach (KinectUser kUser in users)
			if(kUser.ID == UserId)
				kUser.calibrationCompleted = true;
	}

    void OnUserLost(uint UserId)
    {
		statusMsg = String.Format("[{0}] User lost", UserId);
        Debug.Log(statusMsg);

        // remove from global users list
		for(int i=0 ; i<users.Count; ++i)
			if(users[i].ID == UserId)
			{
				users.RemoveAt(i);
				--i;
			}
    }
	
}
