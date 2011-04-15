using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectTranslationControl : MonoBehaviour {

	public NiteWrapper.SkeletonJoint ControlJoint;
	public float MovementThreshold= 2f;
	public float Smoothing = 0.0005f;
    public float crouchThreshold = 50;
	Kinect kin;
	
	public static bool kinectJump = false;	
	public static bool kinectCrouch = false;
    public bool kinectGrenade = false;
	
	private bool walking = false;
	private int timer_cnt = 0;
	
	public int wip_timer = 5;
    public int jumpThreshold = 175;

    private double leftKneeSum = 0.0f;
    private double leftKneeCount = 0;
    private double rightKneeSum = 0.0f;
    private double rightKneeCount = 0;
    private double leftHipHeightSum = 0.0f;
    private double leftHipHeightCount = 0.0f;
    private double rightHipHeightSum = 0.0f;
    private double rightHipHeightCount = 0.0f;
    public double showMe = 0.0f;

    public struct BodyPosition
    {
        public NiteWrapper.SkeletonJointPosition[] currentBody;
        public NiteWrapper.SkeletonJointPosition[] positionDeltas;
        public NiteWrapper.SkeletonJointPosition[] positionAverages;
        public double currentCount;
    };

    private BodyPosition playerPosition;
	private CharacterController contr;

	
	private double jumpTimer = 0.0f;
    public double grenadeTimer = 0.0f;
	
	private bool jumping = false;
    private double headVal = double.PositiveInfinity;

	// Use this for initialization
	void Start () {
		GameObject kinectContainer = GameObject.Find("Kinect");
		
		if(kinectContainer != null)
			kin = kinectContainer.GetComponent<Kinect>();

        playerPosition.currentCount = 0.0f;
        playerPosition.currentBody = new NiteWrapper.SkeletonJointPosition[26];
        playerPosition.positionDeltas = new NiteWrapper.SkeletonJointPosition[26];
        playerPosition.positionAverages = new NiteWrapper.SkeletonJointPosition[26];
		
		contr = GetComponent<CharacterController>();
	}
	
	Vector3 previousPosition = Vector3.zero;
	
	// Update is called once per frame
	void Update () {
		
		if(kin == null || kin.users.Count < 1) return;

		if(jumpTimer > 0.0f)
		{
			jumpTimer -= Time.deltaTime;
		}

        if (grenadeTimer > 0.0f)
        {
            grenadeTimer -= Time.deltaTime;
            if (grenadeTimer <= 0.0f)
            {
                kinectGrenade = false;
            }
        }
				
		KinectUser kUser = kin.users[0];

        for (int i = 0; i < 26; i++)
        {
            /*Get the full body, used for gesture recognition*/
            playerPosition.currentBody[i] = kin.getTransform(kUser.ID, (NiteWrapper.SkeletonJoint)i).pos;

            if (!kinectCrouch && !kinectJump && jumpTimer <= 0)
            {
                if (playerPosition.currentCount != 0)
                {
                    playerPosition.positionDeltas[i].x = playerPosition.positionDeltas[i].x - playerPosition.currentBody[i].x;
                    playerPosition.positionDeltas[i].y = playerPosition.positionDeltas[i].y - playerPosition.currentBody[i].y;
                    playerPosition.positionDeltas[i].z = playerPosition.positionDeltas[i].z - playerPosition.currentBody[i].z;
                    playerPosition.positionAverages[i].x = playerPosition.positionAverages[i].x + playerPosition.currentBody[i].x;
                    playerPosition.positionAverages[i].y = playerPosition.positionAverages[i].y + playerPosition.currentBody[i].y;
                    playerPosition.positionAverages[i].z = playerPosition.positionAverages[i].z + playerPosition.currentBody[i].z;

                    /*If our summation approaches the max value we have to reset, but can keep the average as it stands*/
                    if ((float.MaxValue - playerPosition.positionAverages[i].x) < 1000 ||
                        (float.MaxValue - playerPosition.positionAverages[i].y) < 1000 ||
                        (float.MaxValue - playerPosition.positionAverages[i].z) < 1000)
                    {
                        playerPosition.positionAverages[i].x = (float) playerPosition.positionAverages[i].x / (float) playerPosition.currentCount;
                        playerPosition.positionAverages[i].y = (float)playerPosition.positionAverages[i].y / (float)playerPosition.currentCount;
                        playerPosition.positionAverages[i].z = (float)playerPosition.positionAverages[i].z / (float)playerPosition.currentCount;
                        playerPosition.currentCount = 0;
                    }
                }
                else
                {
                    playerPosition.positionAverages[i] = playerPosition.currentBody[i];
                }
                playerPosition.currentCount++;
            }
        }

		NiteWrapper.SkeletonJointTransformation trans = kin.getTransform(kUser.ID,ControlJoint);
		NiteWrapper.SkeletonJointTransformation trans2;
        NiteWrapper.SkeletonJointTransformation leftHip;
        NiteWrapper.SkeletonJointTransformation rightHip;

		if(trans.pos.confidence > 0.5)
		{
			Vector3 newControlJointPos = new Vector3(trans.pos.x,trans.pos.y,trans.pos.z);
			Vector3 offset = (newControlJointPos - previousPosition) * Smoothing;
			
			if(offset.sqrMagnitude <= MovementThreshold)
			{
				Vector3 movement = Vector3.zero;
				movement  = movement + Camera.main.transform.forward * -offset.z;
				movement  = movement + Camera.main.transform.right * -offset.x;
				movement  = movement + Camera.main.transform.up * -offset.y;
				
				//GetComponent<CharacterController>().height += offset.y / 2;
				
				//transform.position += movement;
				contr.Move(movement);
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
			contr.Move(movement);
			//transform.position += movement;
		}

		trans = kin.getTransform(kUser.ID,NiteWrapper.SkeletonJoint.LEFT_KNEE);
		trans2 = kin.getTransform(kUser.ID,NiteWrapper.SkeletonJoint.RIGHT_KNEE);
        leftHip = kin.getTransform( kUser.ID, NiteWrapper.SkeletonJoint.LEFT_HIP );
        rightHip = kin.getTransform( kUser.ID, NiteWrapper.SkeletonJoint.RIGHT_HIP );

        //Works off the assumption that confidence is similar for all joints
		if(trans.pos.confidence > 0.2)
		{
            double dly, dry;
            double leftavg=0.0f;
            double lefthavg = 0.0f;
            double rightavg = 0.0f;
            double righthavg = 0.0f;

            //Get the distance between hit and knee for both feet
            dly = trans.pos.y - leftHip.pos.y;
            dry = trans2.pos.y - rightHip.pos.y;

            //Determine the running average distance
            if ( leftKneeCount != 0 )
            {
                leftavg = leftKneeSum / leftKneeCount;
                lefthavg = leftHipHeightSum / leftHipHeightCount;
            }
            if ( rightKneeCount != 0 )
            {
                rightavg = rightKneeSum / rightKneeCount;
                righthavg = rightHipHeightSum / rightHipHeightCount;
            }

            //If the distance is 50% the average on the left
            //But NOT 75% of the average on the right (right is normal, left is moving)
            //Also make sure the hip has not lowered (crouching or prep for a jump)
            if ( dly <= ( leftavg / 2.0f ) && dry >= ( rightavg * 0.75f ) && leftHip.pos.y >= ( leftHipHeightSum * 0.85f ) )
            {
                walking = true;
                timer_cnt = wip_timer;
            }

            //Vice Versa
            if ( dly >= ( leftavg * 0.75f ) && dry <= ( rightavg / 2.0f ) && rightHip.pos.y >= ( rightHipHeightSum * 0.85f ) )
            {
                walking = true;
                timer_cnt = wip_timer;
            }

            if ((kinectJump || kinectCrouch) && walking)
            {
                walking = false;
                timer_cnt = 0;
            }

            //If we are not walking, add the values to the running average
            if ( !walking )
            {
                leftKneeSum += dly;
                leftKneeCount++;
                leftHipHeightSum = trans.pos.y;
                leftHipHeightCount++;
                rightKneeSum += dry;
                rightKneeCount++;
                rightHipHeightSum = trans2.pos.y;
                rightHipHeightCount++;
            }
		}

        if (playerPosition.currentBody[(int) NiteWrapper.SkeletonJoint.HEAD].confidence > 0.5)
        {
            double headY = playerPosition.positionDeltas[(int)NiteWrapper.SkeletonJoint.HEAD].y;
            double lkneeY = playerPosition.positionDeltas[(int)NiteWrapper.SkeletonJoint.LEFT_KNEE].y;
            double rkneeY = playerPosition.positionDeltas[(int)NiteWrapper.SkeletonJoint.RIGHT_KNEE].y;
            double avghY = playerPosition.positionAverages[(int)NiteWrapper.SkeletonJoint.HEAD].y / playerPosition.currentCount;
            double poshy = playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.HEAD].y;


            if (/*headY < crouchThreshold && (lkneeY < crouchThreshold || rkneeY < crouchThreshold) &&*/ poshy < (avghY - crouchThreshold) && !kinectJump)
            {
                kinectCrouch = true;
            }
            else
            {
                kinectCrouch = false;
            }

            /*If head and both knees have increased in Y*/
            if (/*headY > jumpThreshold && lkneeY > jumpThreshold && rkneeY > jumpThreshold &&*/ poshy > (avghY + jumpThreshold) && !kinectCrouch)
            {
                //INSERT JUMP HERE
                if (jumpTimer <= 0.0f)
                {
                    kinectJump = true;
                    jumpTimer = 1.0f;
                }
                else
                {
                    kinectJump = false;
                }
            }
            else
            {
                //jumping = false;
                kinectJump = false;
            }
        }

        /*Determine if the hand is on the hip (if so you dip, I dip, we dip)*/
        Vector3 rhandVec = new Vector3(playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HAND].x,
                        playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HAND].y,
                        playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HAND].z);
        Vector3 rhipVec = new Vector3(playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HIP].x,
                        playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HIP].y,
                        playerPosition.currentBody[(int)NiteWrapper.SkeletonJoint.LEFT_HIP].z);
        /*Also we will check if the hand is now over the head*/
        double headAvgY = playerPosition.positionAverages[(int)NiteWrapper.SkeletonJoint.HEAD].y / playerPosition.currentCount;
        double handToHipDistance = Mathf.Sqrt(Mathf.Pow(rhandVec.x - rhipVec.x, 2) + Mathf.Pow(rhandVec.y - rhipVec.y, 2) + Mathf.Pow(rhandVec.z - rhipVec.z, 2));

        if (handToHipDistance < 200)
        {
            grenadeTimer = 2.0f;
        }

        if (grenadeTimer > 0.0f && rhandVec.y > headAvgY && !kinectJump)
        {
            kinectGrenade = true;
        }

        for (int i = 0; i < 26; i++)
        {
            playerPosition.positionDeltas[i] = playerPosition.currentBody[i];
        }
	}
}
