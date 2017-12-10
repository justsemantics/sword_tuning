using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectionCircle : MonoBehaviour {

    bool hidden = true;

    [SerializeField][Range(0, 1)]
    public float opacity = 1;

    talisman selection;
    public talisman Selection { get { return selection; }
    set
        {
            selection = value;
            if (selection == null && !hidden)
            {
                hide();
            }
            else if (selection != null && hidden)
            {
                show();
            }
        }
    }

    bool selectionLocked = false;
    public bool SelectionLocked { get { return selectionLocked; }
    set
        {
            selectionLocked = value;

            if (selectionLocked)
            {
                image.color = lockedColor;
            }
            else
            {
                image.color = selectedColor;
            }
        }
    }

    [SerializeField]
    Image image;

    RectTransform imageTransform;

    [SerializeField]
    Color selectedColor, lockedColor;

    public Camera camera;

    [SerializeField]
    Animator anim;

    private void Awake()
    {
        
        imageTransform = image.GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
        //StartCoroutine(rotate(5f));
	}
	
	// Update is called once per frame
	void Update () {
        positionOverSelectedTalisman();
        setColor();
	}

    public void show()
    {
        hidden = false;
        anim.SetTrigger("show");
        //StartCoroutine(show(0.2f));
    }

    public void hide()
    {
        hidden = true;
        anim.SetTrigger("hide");
        //StartCoroutine(hide(0.2f));
    }

    void positionOverSelectedTalisman()
    {
        if (Selection != null)
        {
            Vector3 position = camera.WorldToScreenPoint(Selection.transform.position);

            imageTransform.anchoredPosition = position;
        }
    }

    void setColor()
    {
        selectedColor.a = opacity;
        lockedColor.a = opacity;
        if (SelectionLocked)
        {
            image.color = lockedColor;
        }
        else
        {
            image.color = selectedColor;
        }
    }

    IEnumerator rotate(float speed)
    {
        while (true)
        {
            imageTransform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
            yield return null;
        }
    }
    IEnumerator show(float duration)
    {
        StopCoroutine("hide");
        float startTime = Time.time;
        float progress = 0;
        while(progress < 1)
        {
            progress = Mathf.Clamp01((Time.time - startTime) / duration);

            opacity = progress;

            imageTransform.localScale = Vector3.one * Mathf.Lerp(3, 1, progress);

            yield return null;
        }
    }

    IEnumerator hide(float duration)
    {
        StopCoroutine("show");
        float startTime = Time.time;
        float progress = 0;
        while (progress < 1)
        {
            progress = Mathf.Clamp01((Time.time - startTime) / duration);

            opacity = 1 - progress;

            imageTransform.localScale = Vector3.one * Mathf.Lerp(1, 0.000001f, progress);

            yield return null;
        }
    }
}
