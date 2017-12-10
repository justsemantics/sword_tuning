using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour {

    public sword_fragment[] fragments;

    [SerializeField]
    float rotationSpeed, speedRange, distance, distanceRange;

	// Use this for initialization
	void Start () {
        Extend();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Extend()
    {
        foreach(sword_fragment fragment in fragments)
        {
            float fragmentRotationSpeed = Random.Range(0, speedRange) + rotationSpeed;
            float fragmentDistance = Random.Range(0, distanceRange) + distance;

            fragment.Extend(fragmentRotationSpeed, fragmentDistance);
        }
    }
}
