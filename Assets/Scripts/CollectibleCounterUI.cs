using UnityEngine;
using TMPro;
using System.Collections;


public class CollectibleCounterUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("TextMeshPro component for displaying feather count")]
    public TextMeshProUGUI featherCountText;

    [Tooltip("TextMeshPro component for displaying seed count")]
    public TextMeshProUGUI seedCountText;

    [Tooltip("TextMeshPro component for displaying notifications")]
    public TextMeshProUGUI notificationText;

    [Header("Notification Settings")]
    [Tooltip("How long notifications stay visible (seconds)")]
    public float notificationDuration = 3f;

    [Tooltip("How fast notifications fade in/out")]
    public float fadeSpeed = 2f;

    private int lastFeatherCount = -1;
    private int lastSeedCount = -1;

    static bool shownFirstSeedNotification = false;
    static bool shownFirstFeatherNotification = false;
    static bool shownSecondFeatherNotification = false;

    private Coroutine currentNotificationCoroutine;

    void Start()
    {
        UpdateCounters();
 
        if (notificationText != null)
        {
            notificationText.alpha = 0f;
            notificationText.text = "";
        }
    }

    void Update()
    {
        int currentFeathers = GameManager.feathers;
        int currentSeeds = GameManager.seeds;

        if (currentFeathers != lastFeatherCount || currentSeeds != lastSeedCount)
        {
            CheckAndShowNotifications(currentSeeds, currentFeathers);
            UpdateCounters();
        }
    }

    void UpdateCounters()
    {
        lastFeatherCount = GameManager.feathers;
        lastSeedCount = GameManager.seeds;

        if (featherCountText != null)
        {
            featherCountText.text = lastFeatherCount.ToString();
        }

        if (seedCountText != null)
        {
            seedCountText.text = lastSeedCount.ToString();
        }
    }

    void CheckAndShowNotifications(int seeds, int feathers)
    {
        // First seed notification
        if (seeds >= 1 && !shownFirstSeedNotification)
        {
            ShowNotification("Seeds increase Billy's speed");
            shownFirstSeedNotification = true;
        }
        else if (feathers >= 1 && !shownFirstFeatherNotification)
        {
            ShowNotification("Longer glide unlocked");
            shownFirstFeatherNotification = true;
        }
        else if (feathers >= 2 && !shownSecondFeatherNotification)
        {
            ShowNotification("Dash unlocked, use shift while in the air to dash");
            shownSecondFeatherNotification = true;
        }
    }

    void ShowNotification(string message)
    {
        if (notificationText == null)
            return;

        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }

        currentNotificationCoroutine = StartCoroutine(DisplayNotification(message));
    }

    IEnumerator DisplayNotification(string message)
    {
        notificationText.text = message;

        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            notificationText.alpha = Mathf.Clamp01(alpha);
            yield return null;
        }

        yield return new WaitForSeconds(notificationDuration);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            notificationText.alpha = Mathf.Clamp01(alpha);
            yield return null;
        }

        notificationText.text = "";
        currentNotificationCoroutine = null;
    }
}
