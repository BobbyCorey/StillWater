using System.Collections;
using UnityEngine;
using TMPro;

public class FishingController : MonoBehaviour
{

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip ambientSound;
    public AudioClip castSound;
    public AudioClip biteSound;

    public enum FishingState
    {
        Idle,
        Casting,
        Waiting,
        Biting,
        Reeling,
        Catch,
        Miss
    }

    public FishingState state = FishingState.Idle;

    [Header("Fishing Settings")]
    public float minCatchTime = 2f;
    public float maxCatchTime = 5f;
    public float biteReactionTimee = 1.5f;

    [Header("Reeling")]
    public float reelStrength = 0.7f;
    public float catchDistance = 2f;

    [Header("Fish Struggle")]
    public float struggleDistance = 0.5f;
    public float struggleDuration = 1.5f;

    public float biteReactionTime = 1.5f;

    [Header("UI")]
    public TextMeshProUGUI fishingText;

    [Header("Bobber")]
    public GameObject bobber;

    public Vector3 castPosition =
        new Vector3(0f, 0.2f, -78f);

    private Vector3 bobberPocketPosition;

    void Start()
    {

        audioSource.clip = ambientSound;
        audioSource.loop = true;
        audioSource.Play();

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

        audioSource.PlayOneShot(castSound);

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
        audioSource.PlayOneShot(biteSound);

        yield return StartCoroutine(
            BobberDip()
        );

        yield return StartCoroutine(
            FishStruggle()
            );

        UpdateUI();

        float timer = 0f;
        bool startedReeling = false;

        while (timer < biteReactionTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startedReeling = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (!startedReeling)
        {
            state = FishingState.Miss;
            UpdateUI();
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            state = FishingState.Reeling;
            UpdateUI();

            while (Vector3.Distance(
                bobber.transform.position,
                transform.position) > catchDistance)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 direction =
                        (transform.position -
                         bobber.transform.position).normalized;

                    bobber.transform.position +=
                        direction * reelStrength;
                }

                yield return null;
            }

            state = FishingState.Catch;
            UpdateUI();

            yield return new WaitForSeconds(1.5f);
        }

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

    IEnumerator FishStruggle()
    {
        Vector3 basePosition =
            bobber.transform.position;

        float timer = 0f;

        while (timer < struggleDuration)
        {
            Vector3 targetPosition =
                basePosition +
                new Vector3(
                    Random.Range(
                        -struggleDistance,
                        struggleDistance
                    ),
                    Random.Range(-0.05f, -0.02f),
                    Random.Range(
                        -struggleDistance,
                        struggleDistance
                    )
                );

            float moveTime = 0f;
            float moveDuration = 0.25f;

            Vector3 startPosition =
                bobber.transform.position;

            while (moveTime < moveDuration)
            {
                bobber.transform.position =
                    Vector3.Lerp(
                        startPosition,
                        targetPosition,
                        moveTime / moveDuration
                    );

                moveTime += Time.deltaTime;
                yield return null;
            }

            timer += moveDuration;
        }

        bobber.transform.position =
            basePosition;
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

            case FishingState.Reeling:
                fishingText.text =
                    "Fish Hooked! Click Fast!";
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