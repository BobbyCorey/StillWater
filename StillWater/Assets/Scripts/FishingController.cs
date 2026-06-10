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
        Biting,
        Catch,
        Miss
    }

    public FishingState state = FishingState.Idle;

    [Header("Fishing Settings")]
    public float minCatchTime = 2f;
    public float maxCatchTime = 5f;

    public float biteReactionTime = 1.5f;

    [Header("UI")]
    public TextMeshProUGUI fishingText;

    [Header("Bobber")]
    public GameObject bobber;

    public Vector3 castPosition =
        new Vector3(0f, 0.2f, 12f);

    private Vector3 bobberPocketPosition;

    void Start()
    {
        UpdateUI();

        if (bobber != null)
        {
            // Save bobber's pocket location
            bobberPocketPosition =
                bobber.transform.localPosition;
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

        bobber.transform.SetParent(null);

        // Place bobber in lake
        yield return StartCoroutine(
            MoveBobber(

                bobber.transform.position,
                castPosition,
                2f

                )
            );

        state = FishingState.Waiting;
        UpdateUI();

        float waitTime =
    Random.Range(minCatchTime, maxCatchTime);

        yield return new WaitForSeconds(waitTime);

        // Fish bites
        state = FishingState.Biting;

        yield return StartCoroutine(
            BobberDip()
        );

        UpdateUI();

        float timer = 0f;
        bool caughtFish = false;

        while (timer < biteReactionTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                caughtFish = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (caughtFish)
        {
            state = FishingState.Catch;
        }
        else
        {
            state = FishingState.Miss;
        }

        UpdateUI();

        yield return new WaitForSeconds(1.5f);

        // Hide bobber again
        if (bobber != null)
        {
            bobber.transform.SetParent(transform);

            bobber.transform.localPosition =
                bobberPocketPosition;
        }

        state = FishingState.Idle;
        UpdateUI();
    }

    IEnumerator MoveBobber(
        Vector3 startPos,
        Vector3 endPos,
        float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (bobber != null)
            {
                bobber.transform.position =
                    Vector3.Lerp(
                        startPos,
                        endPos,
                        elapsedTime / duration
                        );
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bobber != null)
        {
            bobber.transform.position =
                endPos;
        }
    }

    IEnumerator BobberDip()
    {
        Vector3 originalPosition =
            bobber.transform.position;

        // Small nibble 1
        bobber.transform.position =
            originalPosition +
            new Vector3(0f, -0.08f, 0f);

        yield return new WaitForSeconds(0.15f);

        bobber.transform.position =
            originalPosition;

        yield return new WaitForSeconds(0.5f);

        // Small nibble 2
        bobber.transform.position =
            originalPosition +
            new Vector3(0f, -0.08f, 0f);

        yield return new WaitForSeconds(0.15f);

        bobber.transform.position =
            originalPosition;

        yield return new WaitForSeconds(0.5f);

        // BIG bite
        bobber.transform.position =
            originalPosition +
            new Vector3(0f, -0.25f, 0f);

        yield return new WaitForSeconds(0.2f);

        bobber.transform.position =
            originalPosition;
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

            case FishingState.Biting:
                fishingText.text =
                    "FISH BITING! CLICK!";
                break;

            case FishingState.Catch:
                fishingText.text =
                    "You caught a fish!";
                break;

            case FishingState.Miss:
                fishingText.text =
                    "The fish got away...";
                break;
        }
    }
}