using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class UDPsend : MonoBehaviour
{
    //string IP = "10.100.2.103";
    //int PORT = 5151;

    string IP = "127.0.0.1";
    //int PORT = 9000;
    //string IP = "192.168.10.200"; //edit 070220 to get it to work form desktop
    int PORT = 9001;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    long currentTime = CurrentTimeMillis();
    long lastTime = CurrentTimeMillis();
    long periodMs = 100;

    //float azimuth, elevation, tilt, magnitude;

    void Start()
    {
        init();
    }

    // init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        Debug.Log("UDPSend.init()");

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
        client = new UdpClient();
    }

    void Update()
    {
        currentTime = CurrentTimeMillis();
        long delta = currentTime - lastTime;

        if (delta > periodMs)
        {
            lastTime = currentTime;
            sendData(ErrorCalc.newRvA, ErrorCalc.newTilt, ErrorCalc.newTiltChange);
        }
    }

    const int nbParams = 6;

    // sendData
    private void sendData(float angle, float tilt, float tiltChange)
    {
        try
        {
            // map algorithm output to the vest vecor params within ranges

            float azimuth = angle; //works perfectly
            float elevation = tiltChange; // works
            float magnitude = tilt; //works well

            // INPUT
            var floatArray = new float[3];
            floatArray[0] = azimuth; // azimuth -1 to 1 clockwise (positive)
            floatArray[1] = elevation; // elevation -1 to 1 up (positive)
            floatArray[2] = magnitude; // magnitude 0 to 1 

            Debug.Log("The Value of azimuth = " + floatArray[0] + " elevation = " + floatArray[1] + " and Magnitude = " + floatArray[2]);
            // create a byte array and copy the floats into it...
            var data = new byte[floatArray.Length * 4];
            Buffer.BlockCopy(floatArray, 0, data, 0, data.Length);
   
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    private static readonly DateTime Jan1st1970 = new DateTime
   (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

}
