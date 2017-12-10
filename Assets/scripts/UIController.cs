using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    talismanSelector selector;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    selectionCircle selectionCirclePrefab;

    selectionCircle firstSelectionCircle, secondSelectionCircle;

    [SerializeField]
    Camera camera;

    [SerializeField]
    RectTransform connectorLine;

    int count = 0;

	// Use this for initialization
	void Start () {
        firstSelectionCircle = createSelectionCircle();
        secondSelectionCircle = createSelectionCircle();
	}

    selectionCircle createSelectionCircle()
    {
        selectionCircle circle = Instantiate(selectionCirclePrefab, canvas.transform);
        circle.camera = camera;
        circle.name = count.ToString();

        count++;

        return circle;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(selector.firstSelection != firstSelectionCircle.Selection)
        {
                firstSelectionCircle.hide();
                Destroy(firstSelectionCircle.gameObject, 1f);
                firstSelectionCircle = createSelectionCircle();
                firstSelectionCircle.Selection = selector.firstSelection;
        }

        if(selector.secondSelection != secondSelectionCircle.Selection)
        {
            secondSelectionCircle.hide();
            Destroy(secondSelectionCircle.gameObject, 1);
            secondSelectionCircle = createSelectionCircle();
            secondSelectionCircle.Selection = selector.secondSelection;
        }
        if(selector.state == talismanSelector.selectionState.none)
        {
            firstSelectionCircle.SelectionLocked = false;
            secondSelectionCircle.SelectionLocked = false;
        }
        else
        {
            firstSelectionCircle.SelectionLocked = true;
            secondSelectionCircle.SelectionLocked = false;
        }

        drawConnectorLine();
    }

    void drawConnectorLine()
    {
        if(selector.state == talismanSelector.selectionState.firstSelectionMade)
        {
            Vector3 pointA, pointB;

            pointA = camera.WorldToScreenPoint(firstSelectionCircle.Selection.transform.position);
            
            if(selector.secondSelection != null)
            {
                pointB = camera.WorldToScreenPoint(secondSelectionCircle.Selection.transform.position);
            }
            else
            {
                pointB = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
            }

            float adjacent = pointB.x - pointA.x;
            float opposite = pointB.y - pointA.y;

            float hypotenuse = Mathf.Sqrt(adjacent * adjacent + opposite * opposite);

            float theta = Mathf.Asin(opposite / hypotenuse);
            theta = Mathf.Rad2Deg * theta;

            if(adjacent < 0)
            {
                theta = 180 - theta;
            }

            connectorLine.sizeDelta = new Vector2(hypotenuse - 120, 2);
            Quaternion rotation = Quaternion.Euler(0, 0, theta);
            connectorLine.rotation = rotation;

            connectorLine.anchoredPosition = ((pointB - pointA) / 2) + pointA;
        }
        else
        {
            connectorLine.anchoredPosition = new Vector3(1e8f, 0, 0);
        }
    }
}
