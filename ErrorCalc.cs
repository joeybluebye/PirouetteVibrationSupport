using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ErrorCalc : MonoBehaviour
{
    StreamWriter writer;
    //public Object DataSpin;
    public GameObject Head; // tracker 3
    public GameObject LeftHand; //tag LeftHand
    public GameObject RightHand; //tag RightHand
    public GameObject LeftPuck; //tag tracker 1
    public GameObject RightPuck; //tag tracker 2

    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;
    public Vector3 P5;
    public Quaternion R1;
    public Quaternion R2;
    public Quaternion R3;
    public Quaternion R4;
    public Quaternion R5;
    public GameObject pseudoSpin;
    public Quaternion Qinitial;
    public Quaternion RR; //referenced rotation

    public int countSpin;

    public static float M; //magnitude
    public static float newTilt; //elevation 0 to 1 (unadjusted)
    public static float newTiltChange; //magnitude -1 to 1 
    public static float deltaM;
    public static float prevM;
    public static float velocityM;
    public static float velocityMedit;
    private float Mag2; // to print
    public static float A; //angle
    public static float deltaRvA;
    public static float prevRvA;
    public static float velocityRvA;
    private float Ang2; // to print
    private float Ax; //print
    public static float R; //rotation
    private float Rx; // to print
    public static float RvA; //error direction
    public static float newRvA; //error direction 1 to -1
    private float RvAx; //print
    private Vector2 xz;
    private Vector2 xz2;
    private Vector3 Pinitial;
    private float R33;
    private float totalR;

    private float prevR;
    private float deltaR;
    private float x;

    public float speedR;
    public float Rounds;
    public float timer;
    public Vector3 V1;
    public Vector3 V2;
    public Vector3 V3;
    public Vector3 V4;
    public Vector3 V5;
    public Vector3 V1lastPosition;
    public Vector3 V2lastPosition;
    public Vector3 V3lastPosition;
    public Vector3 V4lastPosition;
    public Vector3 V5lastPosition;

    bool SpinON;

    public static float errorangle;
    public static float errorangle2;
    public static float errorangle3;

    public Vector3 dest;
    public Vector3 dest2;
    public Vector3 dest3;
    public Vector3 topp;
    public Vector3 spinn;
    public Vector3 RealTop2;

    public static float B;

    public ArrayList DataS = new ArrayList();

    void Start()
    {
        //The writer part should track 6 DOF on the headset and 2 controllers of the vive VR system. It should send that info to a text file at > 50hz. 
        //Format Timestamp, (button press_on=1/button press_off=2/else=0), (Lhand=0/Rhand=1/Head=2), posX, posY, posZ, roti, rotj, rotk, rotl 

        // textfile should include head, r and left hand. names and values. 

        writer = new StreamWriter(Application.dataPath + "/Log.txt", true);

    }

    // Calculates all variables to be sent for haptic feedback or visualisation in unity
    void DoMath(Transform spin, Transform top, Transform RealTop)
    {
        topp.x = top.position.x;
        topp.y = 1;
        topp.z = top.position.z;
        spinn.x = spin.position.x;
        spinn.y = 1;
        spinn.z = spin.position.z;

        //Gives Magnitude, calculates the distance on the surface between 2 points (like head and waist/leg)
        prevM = M;
        M = Mathf.Sqrt(Mathf.Pow((spinn.x - topp.x), 2f) + Mathf.Pow((spinn.z - topp.z), 2f));

        newTilt = (((M - 0) * (1 - -1)) / (0.3f - 0)) + -1 ; 

        deltaM = M - prevM;
        velocityM = deltaM / Time.deltaTime; 

        newTiltChange = velocityM;
        

        //Gives angle, between location of head and waist/leg without taking into account height. 

        A = Mathf.Atan2((topp.x-spinn.x), (topp.z - spinn.z));

        // Gives number of spin / speed of spins
        // after resetting rotation axis through the tracker up, all rotation can be assumed to be in y.

        R33 = RR.eulerAngles.y;

        // trick to get angles ok
        if (R33 > 180) { R = R33 - 360; }

        else if (R33 < -180) { R = R33 + 360; }

        else { R = R33; }

        Ax = A * 180 / Mathf.PI;
        // Gives relative angle (RvA) between Angle of positions foot/(head/waist) A vs spin of Foot (R)
        //reference to R
        x = Ax - R; 

        // trick to get angles ok
        if (x > 180) { RvA = x - 360; }

        else if (x < -180) { RvA = x + 360; }
        else { RvA = x; }

        newRvA = (((RvA - -180) * (1 - -1)) / (180 - -180)) + -1;

        deltaRvA = RvA - prevRvA;
        velocityRvA = deltaRvA / Time.deltaTime;
        prevRvA = RvA;

        Rx = R;

        RvAx = RvA;
        Mag2 = velocityM;
        Ang2 = velocityRvA;
    }

    void GetData(Transform v1, Transform v2, Transform v3, Transform v4, Transform v5)
    {
        // count rotations and speed of them.
        deltaR = R33 - prevR;
        if (deltaR > 350) { deltaR = deltaR - 360; }
        if (deltaR < -350) { deltaR = deltaR + 360; }

        totalR += deltaR;
        prevR = R33;

        Rounds = totalR / 360;

        speedR = (Mathf.Abs(deltaR) / 360) / Time.deltaTime;

        V1 = (v1.localPosition - V1lastPosition) / Time.deltaTime;
        V1lastPosition = v1.localPosition;
        V2 = (v2.localPosition - V2lastPosition) / Time.deltaTime;
        V2lastPosition = v2.localPosition;
        V3 = (v3.localPosition - V3lastPosition) / Time.deltaTime;
        V3lastPosition = v3.localPosition;
        V4 = (v4.localPosition - V4lastPosition) / Time.deltaTime;
        V4lastPosition = v4.localPosition;
        V5 = (v5.localPosition - V5lastPosition) / Time.deltaTime;
        V5lastPosition = v5.localPosition;

        P1 = v1.transform.position;
        R1 = v1.transform.rotation;
        P2 = v2.transform.position;
        R2 = v2.transform.rotation;
        P3 = v3.transform.position;
        R3 = v3.transform.rotation;
        P4 = v4.transform.position;
        R4 = v4.transform.rotation;
        P5 = v5.transform.position;
        R5 = v5.transform.rotation;
    }

   

    void CreateText()
    {
        //writes in log file
        writer.Write("time," + timer + ",SpinNumber," + countSpin + ",Rounds," + Rounds + ",RotationSpeed," + speedR + ",Magnitude," + M + ",MagChange," + velocityM + ",refAngle," + RvA + ",velocityRvA," + velocityRvA);
        writer.Write(",p1," + P1.x + "," + P1.y + "," + P1.z +",p2," + P2.x + "," + P2.y + "," + P2.z + ",p3," + P3.x + "," + P3.y + "," + P3.z + ",p4," + P4.x + "," + P4.y + "," + P4.z + ",p5," + P5.x + "," + P5.y + "," + P5.z + ",v1," + V1.x + "," + V1.y + "," + V1.z + ",v2," + V2.x + "," + V2.y + "," + V2.z + ",v3," + V3.x + "," + V3.y + "," + V3.z + ",v4," + V4.x + "," + V4.y + "," + V4.z + ",v5," + V5.x + "," + V5.y + "," + V5.z); 
        writer.Write(",r1," + R1.x + "," + R1.y + "," + R1.z + "," + R1.w + ",r2," + R2.x + "," + R2.y + "," + R2.z + "," + R2.w + ",r3," + R3.x + "," + R3.y + "," + R3.z + "," + R3.w + ",r4," + R4.x + "," + R4.y + "," + R4.z + "," + R4.w + ",r5," + R5.x + "," + R5.y + "," + R5.z + "," + R5.w + "\n");
        writer.Flush();
    }

    // set ref at the start of the trial for accurate angels, feedback and recordings
    void RefBody(Transform spin, Transform RealSpin)
    {
        countSpin = ColorChanger.countSpin;
        spin.position = RealSpin.position;
        spin.rotation = RealSpin.rotation;

        // when user inputs the start of a spin livespin turns to one. this will cause the system to get a new reference (once as it turns spinON to true). 
        if (SpinON == false & ColorChanger.liveSpin == 1)
        {

            Qinitial = RealSpin.rotation;
            Pinitial = RealSpin.position;
            SpinON = true;
        }

        if (ColorChanger.liveSpin == 0)
        {
            // spins are off.
            SpinON = false;
            // kill the time
            timer = 0.0f;
            totalR = 0.0f;

            //turn off haptic feedback by sending 0 mag
            newTilt = 0;
        }

        if (SpinON == true) { timer += Time.deltaTime;
            CreateText();
        }

            // rotation is in reference 

            RR = RealSpin.rotation * Quaternion.Inverse(Qinitial);

        spin.position = spin.position - Pinitial;
        spin.rotation = RR;
    }

    void Update()
    {
        DoMath(LeftPuck.transform, Head.transform, H.transform);
        RefBody(pseudoSpin.transform, LeftPuck.transform);
        GetData(Head.transform, LeftHand.transform, RightHand.transform, LeftPuck.transform, RightPuck.transform);
    }
}
