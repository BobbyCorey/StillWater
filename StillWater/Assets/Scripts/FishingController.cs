using System.Collections;
using UnityEngine;
using TMPro;

public class FishingController : MonoBehaviour
{
    public enum FishingState
    {
        Idle,
        Casting,
        Waiting,
        Catch
    }

    public FishingState state = FishingState.Idle;

    [Header("Fishing Settings")]
    public float minCatchTime = 2f;
    public float maxCatchTime = 5f;

    [Header("UI")]
    public TextMeshProUGUI fishingText;

    [Header("Bobber")]
    public GameObject bobber;

    public Vector3 castPosition =
        new Vector3(0f, 0.2f, 12f);

    private Vector3 hiddenPosition =
        new Vector3(0f, -100f, 0f);

    void Start()
    {
        UpdateUI();

        if (bobber != null)
        {
            bobber.transform.position =
                hiddenPosition;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && state == FishingState.Idle)
        {
            StartCoroutine(FishingRoutine());
        }
    }

    IEnumerator FishingRoutine()
    {
        state = FishingState.Casting;
        UpdateUI();

        yield return new WaitForSeconds(0.5f);

        // Place bobber in lake
        if (bobber != null)
        {
            bobber.transform.position =
                castPosition;
        }

        state = FishingState.Waiting;
        UpdateUI();

        float waitTime =
            Random.Range(minCatchTime, maxCatchTime);

        yield return new WaitForSeconds(waitTime);

        state = FishingState.Catch;
        UpdateUI();

        yield return new WaitForSeconds(1.5f);

        // Hide bobber again
        if (bobber != null)
        {
            bobber.transform.position =
                hiddenPosition;
        }

        state = FishingState.Idle;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (fishingText == null) return;

        switch (state)
        {
            case FishingState.Idle:
                fishingText.text =
                    "Idle - Click to Cast";
                break;

            case FishingState.Casting:
                fishingText.text =
                    "Casting Line...";
                break;

            case FishingState.Waiting:
                fishingText.text =
                    "Waiting for a bite...";
                break;

            case FishingState.Catch:
                fishingText.text =
                    "You caught a fish!";
                break;
        }
    }
}