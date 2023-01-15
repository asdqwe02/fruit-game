using UnityEngine;
using UnityEngine.UI;
//using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Events;

public class KinectHandPosition : MonoBehaviour
{
    private KinectManager kinectManager;
    private List<long> playerUserIDList = new List<long>();
    public static KinectHandPosition Instance;

    public bool leftHandInteraction = true;
    public bool rightHandInteraction = true;

    private bool isleftIboxValid;
    private bool isRightIboxValid;

    // Dictionary Fields
    // Left hand
    private Dictionary<Int64, Vector3> leftHandPosDict = new Dictionary<Int64, Vector3>();
    private Dictionary<Int64, Vector3> leftHandScreenPosDict = new Dictionary<Int64, Vector3>();
    private Dictionary<Int64, Vector3> leftIboxLeftBotBackDict = new Dictionary<Int64, Vector3>();
    private Dictionary<Int64, Vector3> leftIboxRightTopFrontDict = new Dictionary<Int64, Vector3>();

    // Right hand
    private Dictionary<Int64, Vector3> rightHandPosDict = new Dictionary<Int64, Vector3>();
    private Dictionary<Int64, Vector3> rightHandScreenPosDict = new Dictionary<Int64, Vector3>();
    private Dictionary<Int64, Vector3> rightIboxLeftBotBackDict = new Dictionary<Int64, Vector3>();

    private Dictionary<Int64, Vector3> rightIboxRightTopFrontDict = new Dictionary<Int64, Vector3>();

    // Events
    public UnityEvent<Int64, bool> UserIdUpdate;

    void Start()
    {
        kinectManager = KinectManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (kinectManager && kinectManager.IsInitialized())
        {
            # region Two-Person-HandPos

            playerUserIDList = kinectManager.GetAllUserIds();
            if (playerUserIDList.Count > 0)
            {
                foreach (var id in playerUserIDList)
                {
                    if (!leftHandPosDict.ContainsKey(id) || !rightHandPosDict.ContainsKey(id))
                    {
                        // left hand
                        leftHandPosDict.Add(id, Vector3.zero);
                        leftIboxLeftBotBackDict.Add(id, Vector3.zero);
                        leftIboxRightTopFrontDict.Add(id, Vector3.zero);
                        // UserIdUpdate?.Invoke(id, false);

                        // right right hand
                        rightHandPosDict.Add(id, Vector3.zero);
                        rightIboxLeftBotBackDict.Add(id, Vector3.zero);
                        rightIboxRightTopFrontDict.Add(id, Vector3.zero);
                        UserIdUpdate?.Invoke(id, false);
                    }
                }

                // Debug.Log(leftHandPosDict.Count);
                List<Int64> removeId = new List<Int64>();
                foreach (var key in leftHandPosDict.Keys.ToList())
                {
                    if (playerUserIDList.Contains(key))
                    {
                        // Int64 key = key.Key;
                        // update left hand position dictionary by key

                        Vector3 leftIboxLeftBotBackTemp = leftIboxLeftBotBackDict[key];
                        Vector3 leftIboxRightTopFrontTemp = leftIboxRightTopFrontDict[key];
                        isleftIboxValid = kinectManager.GetLeftHandInteractionBox(
                            key,
                            ref leftIboxLeftBotBackTemp,
                            ref leftIboxRightTopFrontTemp,
                            isleftIboxValid
                        );
                        leftIboxLeftBotBackDict[key] = leftIboxLeftBotBackTemp;
                        leftIboxRightTopFrontDict[key] = leftIboxRightTopFrontTemp;

                        if (isleftIboxValid && leftHandInteraction && //bLeftHandPrimaryNow &&
                            kinectManager.GetJointTrackingState(key, (int)KinectInterop.JointType.HandLeft) != KinectInterop.TrackingState.NotTracked)
                        {
                            Vector3 leftHandJoinPos = kinectManager.GetJointPosition(key, (int)KinectInterop.JointType.HandLeft);
                            leftHandPosDict[key] = leftHandJoinPos;
                            // update left hand screen position dictionary by key
                            leftHandScreenPosDict[key] =
                                new Vector3(
                                    Mathf.Clamp01((leftHandPosDict[key].x - leftIboxLeftBotBackDict[key].x) /
                                                  (leftIboxRightTopFrontDict[key].x - leftIboxLeftBotBackDict[key].x)),
                                    Mathf.Clamp01((leftHandPosDict[key].y - leftIboxLeftBotBackDict[key].y) /
                                                  (leftIboxRightTopFrontDict[key].y - leftIboxLeftBotBackDict[key].y)),
                                    Mathf.Clamp01((leftIboxLeftBotBackDict[key].z - leftHandPosDict[key].z) /
                                                  (leftIboxLeftBotBackDict[key].z - leftIboxLeftBotBackDict[key].z))
                                );
                        }
                    }
                    else
                    {
                        if (!removeId.Contains(key))
                        {
                            removeId.Add(key);
                        }

                        // UserIdChange?.Invoke(leftHandPos.Key, true);
                    }
                }

                foreach (var key in rightHandPosDict.Keys.ToList())
                {
                    if (playerUserIDList.Contains(key))
                    {
                        // Int64 key = key;
                        Vector3 rightIboxLeftBotBackTemp = rightIboxLeftBotBackDict[key];
                        Vector3 rightIboxRightTopFrontTemp = rightIboxRightTopFrontDict[key];
                        isRightIboxValid = kinectManager.GetLeftHandInteractionBox(
                            key,
                            ref rightIboxLeftBotBackTemp,
                            ref rightIboxRightTopFrontTemp, isRightIboxValid
                        );
                        rightIboxLeftBotBackDict[key] = rightIboxLeftBotBackTemp;
                        rightIboxRightTopFrontDict[key] = rightIboxRightTopFrontTemp;

                        if (isRightIboxValid && rightHandInteraction && //bRightHandPrimaryNow &&
                            kinectManager.GetJointTrackingState(key, (int)KinectInterop.JointType.HandRight) != KinectInterop.TrackingState.NotTracked)
                        {
                            Vector3 rightHandJoinPos = kinectManager.GetJointPosition(key, (int)KinectInterop.JointType.HandRight);
                            rightHandPosDict[key] = rightHandJoinPos;
                            rightHandScreenPosDict[key] =
                                new Vector3(
                                    Mathf.Clamp01((rightHandPosDict[key].x - rightIboxLeftBotBackDict[key].x) /
                                                  (rightIboxRightTopFrontDict[key].x - rightIboxLeftBotBackDict[key].x)),
                                    Mathf.Clamp01((rightHandPosDict[key].y - rightIboxLeftBotBackDict[key].y) /
                                                  (rightIboxRightTopFrontDict[key].y - rightIboxLeftBotBackDict[key].y)),
                                    Mathf.Clamp01((rightIboxLeftBotBackDict[key].z - rightHandPosDict[key].z) /
                                                  (rightIboxLeftBotBackDict[key].z - rightIboxLeftBotBackDict[key].z))
                                );
                        }
                    }
                    else
                    {
                        if (!removeId.Contains(key))
                        {
                            removeId.Add(key);
                        }
                    }
                }

                foreach (var id in removeId)
                {
                    rightHandPosDict.Remove(id);
                    rightIboxRightTopFrontDict.Remove(id);
                    rightIboxLeftBotBackDict.Remove(id);
                    leftHandPosDict.Remove(id);
                    rightIboxRightTopFrontDict.Remove(id);
                    leftIboxLeftBotBackDict.Remove(id);
                    UserIdUpdate?.Invoke(id, true);
                }
            }

            # endregion
        }
    }
}