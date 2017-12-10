using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword_fragment : MonoBehaviour {

    [SerializeField]
    Transform fragmentModel;

    public void Extend(float _rotationSpeed, float _distance)
    {
        float y, z, theta;
        theta = Random.Range(0, Mathf.PI / 2) - Mathf.PI / 6;
        y = Mathf.Sin(theta) * _distance;
        z = Mathf.Cos(theta) * _distance;

        extendedPosition = new Vector3(0, y, z);

        rotationSpeed = _rotationSpeed;

        extended = true;
    }

    public void Retract()
    {

    }

    bool extended = false;
    float rotationSpeed = 0;
    Vector3 extendedPosition;
    Quaternion spin = Quaternion.identity;
    Quaternion initialSpin = Quaternion.identity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (extended)
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));

            fragmentModel.localPosition = Vector3.Lerp(fragmentModel.localPosition, extendedPosition, 0.03f);
        }
	}


}
