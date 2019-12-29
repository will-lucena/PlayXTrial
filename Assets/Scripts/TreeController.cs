﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    //*
    public static Func<Vector3, Vector3, float, AnimationCurve, IEnumerator> onAnimate;
    public static Action onDestroy;
    
    [SerializeField] private GameObject trunkPrefab;
    [SerializeField] private float trunkOffset;
    [SerializeField] private AnimationSO popup;
    [SerializeField] private AnimationSO bounce;
    [SerializeField] private AnimationSO collapse;

    private Queue<GameObject> trunks;
    
    public void initTree(int height)
    {
        trunks = new Queue<GameObject>();
        for (int i = 0; i < height; i++)
        {
            GameObject trunk = Instantiate(trunkPrefab, new Vector3(0, i * trunkOffset, 0), Quaternion.Euler(90, 0, 0), transform);
            trunks.Enqueue(trunk);
        }
        StopAllCoroutines();
        StartCoroutine(growthAnimation(height));
    }

    private IEnumerator growthAnimation(int height)
    {
        if (onAnimate != null)
        {
            Vector3 initialPosition = new Vector3(transform.localPosition.x, -trunkOffset * height, transform.localPosition.z);
            Vector3 finalPosition = new Vector3(transform.localPosition.x, trunkOffset /2f, transform.localPosition.z);
            yield return onAnimate.Invoke(initialPosition, finalPosition, popup.durattion,
                popup.curve);
        }

        yield return bounceAnimation();
    }

    private IEnumerator bounceAnimation()
    {
        if (onAnimate != null)
        {
            Vector3 finalPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 2, transform.localPosition.z);
            yield return onAnimate.Invoke(transform.localPosition, finalPosition, bounce.durattion,
                bounce.curve);
        }
    }

    private IEnumerator collapseAnimation()
    {
        if (onAnimate != null)
        {
            Vector3 finalPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - trunkOffset, transform.localPosition.z);
            yield return onAnimate.Invoke(transform.localPosition, finalPosition, collapse.durattion,
                collapse.curve);
        }
    }
    
    public void dequeueTrunk()
    {
        Destroy(trunks.Dequeue());
        StopAllCoroutines();
        StartCoroutine(collapseAnimation());
        if (trunks.Count == 0)
        {
            onDestroy?.Invoke();
        }
    }
    /**/
}
