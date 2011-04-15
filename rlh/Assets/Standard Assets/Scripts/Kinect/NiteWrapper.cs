using UnityEngine;

using System;
using System.Text; 
using System.Runtime.InteropServices;


public class NiteWrapper
{
	public enum SkeletonJoint
	{ 
		NONE = 0,
		HEAD = 1,
        NECK = 2,
        TORSO_CENTER = 3,
		WAIST = 4,

		LEFT_COLLAR = 5,
		LEFT_SHOULDER = 6,
        LEFT_ELBOW = 7,
        LEFT_WRIST = 8,
        LEFT_HAND = 9,
        LEFT_FINGERTIP = 10,

        RIGHT_COLLAR = 11,
		RIGHT_SHOULDER = 12,
		RIGHT_ELBOW = 13,
		RIGHT_WRIST = 14,
		RIGHT_HAND = 15,
        RIGHT_FINGERTIP = 16,

        LEFT_HIP = 17,
        LEFT_KNEE = 18,
        LEFT_ANKLE = 19,
        LEFT_FOOT = 20,

        RIGHT_HIP = 21,
		RIGHT_KNEE = 22,
        RIGHT_ANKLE = 23,
		RIGHT_FOOT = 24,

		END 
	};

    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointPosition
    {
        public float x, y, z;
        public float confidence;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointOrientation
    {
        public float    m00, m01, m02,
                        m10, m11, m12,
                        m20, m21, m22;
        public float confidence;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointTransformation
    {
        public SkeletonJointPosition pos;
        public SkeletonJointOrientation ori;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XnVector3D
    {
        public float x, y, z;
    }

	[DllImport("UnityInterface.dll")]
	public static extern uint Init(StringBuilder strXmlPath);
	[DllImport("UnityInterface.dll")]
	public static extern void Update(bool async);
	[DllImport("UnityInterface.dll")]
	public static extern void Shutdown();
	
	[DllImport("UnityInterface.dll")]
	public static extern IntPtr GetStatusString(uint rc);
	[DllImport("UnityInterface.dll")]
	public static extern int GetDepthWidth();
	[DllImport("UnityInterface.dll")]
	public static extern int GetDepthHeight();
	[DllImport("UnityInterface.dll")]
	public static extern IntPtr GetUsersLabelMap();
    [DllImport("UnityInterface.dll")]
    public static extern IntPtr GetUsersDepthMap();

	[DllImport("UnityInterface.dll")]
    public static extern void SetSkeletonSmoothing(double factor);
    [DllImport("UnityInterface.dll")]
    public static extern bool GetJointTransformation(uint userID, SkeletonJoint joint, ref SkeletonJointTransformation pTransformation);

    [DllImport("UnityInterface.dll")]
    public static extern void StartLookingForUsers(IntPtr NewUser, IntPtr CalibrationStarted, IntPtr CalibrationFailed, IntPtr CalibrationSuccess, IntPtr UserLost);
    [DllImport("UnityInterface.dll")]
    public static extern void StopLookingForUsers();
    [DllImport("UnityInterface.dll")]
    public static extern void LoseUsers();
    [DllImport("UnityInterface.dll")]
    public static extern bool GetUserCenterOfMass(uint userID, ref XnVector3D pCenterOfMass);
    [DllImport("UnityInterface.dll")]
    public static extern float GetUserPausePoseProgress(uint userID);

    public delegate void UserDelegate(uint userId);

    public static void StartLookingForUsers(UserDelegate NewUser, UserDelegate CalibrationStarted, UserDelegate CalibrationFailed, UserDelegate CalibrationSuccess, UserDelegate UserLost)
    {
        StartLookingForUsers(
            Marshal.GetFunctionPointerForDelegate(NewUser),
            Marshal.GetFunctionPointerForDelegate(CalibrationStarted),
            Marshal.GetFunctionPointerForDelegate(CalibrationFailed),
            Marshal.GetFunctionPointerForDelegate(CalibrationSuccess),
            Marshal.GetFunctionPointerForDelegate(UserLost));
    }
}

