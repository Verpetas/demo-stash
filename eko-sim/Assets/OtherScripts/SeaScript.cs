using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaScript : MonoBehaviour
{
    public float minSize = 1420f;
    public float maxSize = 1430f;
    public float currentSize;
    public bool biggening = true;
    public float changeSpeed = 0.5f;
    public AnimationCurve curve;
    public float curveVal;
    private void Start()
    {
        currentSize = minSize;
        transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }
    private void Update()
    {
        curveVal = curve.Evaluate((maxSize - currentSize) / (maxSize - minSize));
        float currSpeed = changeSpeed * curveVal;
        if(currentSize < maxSize && biggening)
        {
            transform.localScale += new Vector3(currSpeed, currSpeed, currSpeed);
            currentSize += currSpeed;
        }else if(currentSize >= maxSize && biggening)
        {
            biggening = false;
        }else if(currentSize > minSize && !biggening)
        {
            transform.localScale -= new Vector3(currSpeed, currSpeed, currSpeed);
            currentSize -= currSpeed;
        }else if(currentSize <= minSize && !biggening)
        {
            biggening = true;
        }
        
    }
}
