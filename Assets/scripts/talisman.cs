using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class talisman : MonoBehaviour, IComparer {

    static float nearbyDistance = 0.001f;
    static float continueChance = 0.9f;

    public bool startingPing = false;

    public magicSphere MagicSphere;

    [SerializeField]
    GameObject magicLine;

    [SerializeField][ColorUsage(false, true, 0, 8, 0.125f, 3)]
    Color flashColor, lastFlashColor;

    Color currentFlashColor;

    [SerializeField]
    AudioClip normalPing, lastPing;

    Color initialColor;

    public float Lat { get; private set; }
    public float Lng { get; private set; }

    MeshRenderer mRenderer;
    AudioSource aSource;

    Quaternion rotation = new Quaternion();

    // Use this for initialization
    void Start () {
        Lat = UnityEngine.Random.value * 360;
        Lng = UnityEngine.Random.value * 60 + 10;

        transform.position = MagicSphere.GetWorldPositionFromLatLng(Lat, Lng);

        mRenderer = GetComponent<MeshRenderer>();
        initialColor = mRenderer.material.GetColor("_EmissionColor");

        aSource = GetComponent<AudioSource>();
        aSource.clip = normalPing;
        //create a UnityEngine.Random rotation to look cool
        Vector3 axis = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        float angle = UnityEngine.Random.value * 0.6f + 0.1f;
        rotation = Quaternion.AngleAxis(angle, axis);

        if(startingPing)
            StartCoroutine(pingRandomly(2, 10));
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion newRotation = transform.rotation * rotation;
        transform.rotation = newRotation;
	}

    void playSound()
    {
        float pitch = UnityEngine.Random.value * 0.07f + 1;
        aSource.pitch = pitch;
        aSource.Stop();
        aSource.Play();
    }

    public void Ping(talisman target)
    {
        Flash();
        StartCoroutine(lerpMagicLineTimed(1.5f, target));
    }

    public void startPing()
    {
        StartCoroutine(pingRandomly(0, 0));
    }

    public void ReceivePing()
    {
        if(UnityEngine.Random.value < continueChance)
        {
            StartCoroutine(pingRandomly(0, 0));
        }
        else
        {
            lastFlash();
        }
    }

    void TimedPing(talisman target)
    {

    }

    void lastFlash()
    {
        aSource.clip = lastPing;
        playSound();

        StopCoroutine("emissiveFlash");
        currentFlashColor = lastFlashColor;
        StartCoroutine(emissiveFlash(3));
    }

    public void Flash()
    {
        aSource.clip = normalPing;
        playSound();

        StopCoroutine("emissiveFlash");
        currentFlashColor = flashColor;
        StartCoroutine(emissiveFlash(1));
    }

    IEnumerator emissiveFlash(float time)
    {
        float startTime = Time.time;

        while (Time.time - startTime < time)
        {
            float progress = (Time.time - startTime) / time;

            Color currentColor = Color.Lerp(currentFlashColor, initialColor, progress);

            mRenderer.material.SetColor("_EmissionColor", currentColor);

            yield return null;
        }

        mRenderer.material.SetColor("_EmissionColor", initialColor);
    }

    IEnumerator lerpMagicLine(float speed, talisman target)
    {
        float startTime = Time.time;
        GameObject line = Instantiate(magicLine, transform.position, transform.rotation, null);
        TrailRenderer trail = line.GetComponent<TrailRenderer>();
        float delay = trail.time;
        bool countingDown = false;
        while(delay > 0)
        {
            if(!countingDown)
                if(Vector3.SqrMagnitude(target.transform.position - line.transform.position) < nearbyDistance)
                {
                    countingDown = true;
                    target.ReceivePing();
                }
            if (countingDown)
            {
                delay -= Time.deltaTime * 5;
                trail.time = delay;
            }
            Vector3 position = Vector3.MoveTowards(line.transform.position, target.transform.position, speed * Time.deltaTime);
            line.transform.position = position;
            yield return null;
        }
    }

    IEnumerator lerpMagicLineTimed(float time, talisman target)
    {
        float progress = 0;
        GameObject line = Instantiate(magicLine, transform.position, transform.rotation, null);
        TrailRenderer trail = line.GetComponent<TrailRenderer>();
        float delay = trail.time;
        bool countingDown = false;
        while (delay > 0) //while the line has a reason to maintain its position
        {
            if (!countingDown) //if the line has not yet reached the target talisman
            {
                if (progress >= 1) //if the line actually has though
                {
                    progress = 1;
                    countingDown = true; //start retracting the tail of the line
                    target.ReceivePing(); //tell the target that we made it
                }
                else //otherwise keep increasing progress until it > 1
                {
                    progress += Time.deltaTime / time;
                }
            }
                
            if (countingDown) //if we are retracting the tail of the line
            {
                delay -= Time.deltaTime * 5;
                trail.time = delay;
                if (trail.time <= 0)
                    Destroy(trail.gameObject);
            }

            Vector3 position = Vector3.Lerp(transform.position, target.transform.position, progress);
            line.transform.position = position;
            yield return null;
        }
    }

    IEnumerator pingRandomly(float minDelay, float maxDelay)
    {
        yield return new WaitForSeconds(UnityEngine.Random.value * (maxDelay - minDelay) + minDelay);

        talisman[] talismanArray = FindObjectsOfType<talisman>();
        List<talisman> talismans = new List<talisman>();

        foreach(talisman t in talismanArray)
        {
            if(t != this)
                talismans.Add(t);
        }

        
        talismans.Sort(Compare);

        int talismanIndex = Mathf.FloorToInt(UnityEngine.Random.value * 4);

        Ping(talismans[talismanIndex]);
    }

    public int Compare(object x, object y)
    {
        talisman talismanX = (talisman)x;
        talisman talismanY = (talisman)y;

        float distSqrX = Vector3.SqrMagnitude(talismanX.transform.position - transform.position);
        float distSqrY = Vector3.SqrMagnitude(talismanY.transform.position - transform.position);

        if (distSqrX < distSqrY)
            return -1;

        if(distSqrX == distSqrY)
            return 0;

        return 1;
    }
}


