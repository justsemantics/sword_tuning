using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class talismanSelector : MonoBehaviour {

    public enum selectionState
    {
        none, firstSelectionMade
    }

    public selectionState state { get; private set; }
    public talisman firstSelection { get; private set; }
    public talisman secondSelection { get; private set; }

    List<talisman> raycastHitHistory = new List<talisman>();

    [SerializeField]
    int holdTime = 30;
	// Use this for initialization
	void Start () {
        firstSelection = null;
        secondSelection = null;
	}
	
	// Update is called once per frame
	void Update () {
        performRaycast();
        handleSelection();
        checkInput();
	}

    void performRaycast()
    {
        talisman raycastHitTalisman = null;

        RaycastHit hitInfo = new RaycastHit();
        Ray ray = new Ray(transform.position, transform.localToWorldMatrix * Vector3.forward);
        Debug.DrawRay(transform.position, transform.forward);
        Physics.Raycast(new Ray(transform.position, transform.localToWorldMatrix * Vector3.forward), out hitInfo);

        if (hitInfo.collider != null)
        {
            raycastHitTalisman = hitInfo.collider.GetComponent<talisman>();
        }

        raycastHitHistory.Add(raycastHitTalisman);
    }

    void handleSelection()
    {
        if (raycastHitHistory.Count > holdTime)
            raycastHitHistory.RemoveAt(0);

        talisman raycastHitTalisman = null;

        for(int i = raycastHitHistory.Count - 1; i >=0; i--)
        {
            if(raycastHitHistory[i] != null)
            {
                raycastHitTalisman = raycastHitHistory[i];
                break;
            }

        }

        switch (state)
        {
            case selectionState.none:
                firstSelection = raycastHitTalisman;
                break;

            case selectionState.firstSelectionMade:
                if (raycastHitTalisman != firstSelection)
                    secondSelection = raycastHitTalisman;
                break;
        }
    }



    void checkInput()
    {
        if(Input.GetButtonDown("Fire1") && firstSelection != null)
        {
            state = selectionState.firstSelectionMade;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            state = selectionState.none;
            orderPing();

            firstSelection = null;
            secondSelection = null;
        }
    }

    void orderPing()
    {
        if (firstSelection != null && secondSelection != null)
        {
            firstSelection.Ping(secondSelection);
        }
    }
}
