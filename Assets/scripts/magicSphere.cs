using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicSphere : MonoBehaviour {

    float radius = 2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            talisman[] talismans = FindObjectsOfType<talisman>();
            int talismanIndex = Mathf.FloorToInt(UnityEngine.Random.value * (talismans.Length - 0.0001f));

            talismans[talismanIndex].startPing();
        }
	}

    public Vector3 GetWorldPositionFromLatLng(float lat, float lng)
    {
        //start at the "front" of the sphere (this is like the intersection of the equator and prime meridian)
        Vector3 position = Vector3.forward * radius;

        //rotate to the lat and lng from that point
        position = Quaternion.Euler(new Vector3(-lng, lat, 0)) * position;

        //convert to world coordinates
        position = transform.localToWorldMatrix * position;

        return position;
    }
}
