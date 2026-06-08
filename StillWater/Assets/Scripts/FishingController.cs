using System.Collections;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    private bool isFishing = false;

    [Header("Fishing Settings")]
    public float minCatchTime = 2f;
    public float maxCatchTime = 5f;

    void Update()
    {
        // Left mouse click to start fishing
        if (Input.GetMouseButtonDown(0) && !isFishing)
        {
            StartCoroutine(FishingRoutine());
        }
    }

    IEnumerator FishingRoutine()
    {
        isFishing = true;

        Debug.Log("🎣 You cast your line...");

        float waitTime = Random.Range(minCatchTime, maxCatchTime);
        yield return new WaitForSeconds(waitTime);

        Debug.Log("🐟 You caught a fish!");

        isFishing = false;
    }
}