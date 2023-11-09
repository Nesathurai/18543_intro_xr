/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Newtonsoft.Json;


namespace Oculus.Interaction.Samples
{
    public class CreateVisuals : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHmd))]
        private UnityEngine.Object _hmd;
        private IHmd Hmd { get; set; }

        [SerializeField]
        private ActiveStateSelector[] _poses;

        [SerializeField]
        private Material[] _onSelectIcons;

        [SerializeField]
        private GameObject _poseActiveVisualPrefab;

        private GameObject[] _poseActiveVisuals;

        protected virtual void Awake()
        {
            Hmd = _hmd as IHmd;
        }
        public Compute compute; 
        IDictionary<string, IDictionary<string, BoneData>> allPoses = new Dictionary<string, IDictionary<string, BoneData>>();
        IDictionary<string, GameObject> activeVisuals = new Dictionary<string, GameObject>();
        void Start()
        {
            this.AssertField(Hmd, nameof(Hmd));
            this.AssertField(_poseActiveVisualPrefab, nameof(_poseActiveVisualPrefab));
            allPoses = compute.allPoses;
            Debug.Log("all pose size: " + allPoses.Count);

            // _poseActiveVisuals = new GameObject[allPoses.Count];
            foreach(KeyValuePair<string, IDictionary<string, BoneData>> pose in allPoses){
                if(allPoses.ContainsKey(pose.Key)){
                    // allPoses[pose.Key] = onePose;
                    // pose.Value, currentPose
                    // pose.Key
                }
                else{
                    activeVisuals.Add(pose.Key, new GameObject());
                }
                activeVisuals[pose.Key] = Instantiate(_poseActiveVisualPrefab);
                activeVisuals[pose.Key].GetComponentInChildren<TextMeshPro>().text = pose.Key;
                activeVisuals[pose.Key].SetActive(false);
                // _poseActiveVisuals[i] = Instantiate(_poseActiveVisualPrefab);
                // _poseActiveVisuals[i].GetComponentInChildren<TextMeshPro>().text = _poses[i].name;
                // _poseActiveVisuals[i].GetComponentInChildren<ParticleSystemRenderer>().material = _onSelectIcons[i];
                // _poseActiveVisuals[i].SetActive(false);

                // int poseNumber = i;
                // _poses[i].WhenSelected += () => ShowVisuals(poseNumber);
                // _poses[i].WhenUnselected += () => HideVisuals(poseNumber);
            }
        }
        void ShowVisuals(string poseName)
        {
            if (!Hmd.TryGetRootPose(out Pose hmdPose))
            {
                return;
            }

            Vector3 spawnSpot = hmdPose.position + hmdPose.forward;
            activeVisuals[poseName].transform.position = spawnSpot;
            activeVisuals[poseName].transform.LookAt(2 * activeVisuals[poseName].transform.position - hmdPose.position);
            // Vector3 spawnSpot = hmdPose.position + hmdPose.forward;
            // _poseActiveVisuals[poseNumber].transform.position = spawnSpot;
            // _poseActiveVisuals[poseNumber].transform.LookAt(2 * _poseActiveVisuals[poseNumber].transform.position - hmdPose.position);

            // var hands = _poses[poseNumber].GetComponents<HandRef>();
            // TODO: check that the hands are being grabbed properly 
            var hands = gameObject.GetComponentsInParent<HandRef>();
            Vector3 visualsPos = Vector3.zero;
            foreach (var hand in hands)
            {
                hand.GetRootPose(out Pose wristPose);
                Vector3 forward = hand.Handedness == Handedness.Left ? wristPose.right : -wristPose.right;
                visualsPos += wristPose.position + forward * .15f + Vector3.up * .02f;
            }
            activeVisuals[poseName].transform.position = visualsPos / hands.Length;
            activeVisuals[poseName].gameObject.SetActive(true);
        }

        void HideVisuals(string poseName)
        {
            activeVisuals[poseName].gameObject.SetActive(false);
        }
    }
}
