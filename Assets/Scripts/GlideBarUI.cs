using UnityEngine;
using UnityEngine.UI;

public class GlideBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The slider component that shows glide duration")]
    public Slider glideSlider;

    [Tooltip("Reference to the player (auto-found if not assigned)")]
    public Player player;

    [Header("Settings")]
    [Tooltip("Fade speed when showing/hiding the bar")]
    public float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;
    private float previousGlideDuration;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        if (glideSlider != null)
        {
            glideSlider.minValue = 0f;
            glideSlider.interactable = false; 
        }
    }

    void Update()
    {
        if (player == null || glideSlider == null)
            return;

        UpdateGlideBar();
    }

    void UpdateGlideBar()
    {
        float maxGlide = GetPlayerMaxGlideDuration();
        float currentGlide = GetPlayerGlideDurationLeft();
        bool isGliding = GetPlayerIsGliding();
        bool isGrounded = GetPlayerIsGrounded();

        glideSlider.maxValue = maxGlide;

        if (isGrounded && currentGlide >= maxGlide)
        {
            glideSlider.value = maxGlide;
            canvasGroup.alpha = 0f;
        }
        else if (isGliding)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            glideSlider.value = currentGlide;
        }
        else if (!isGrounded)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            canvasGroup.alpha = 0f;
            glideSlider.value = maxGlide;
        }

        previousGlideDuration = currentGlide;
    }



    float GetPlayerMaxGlideDuration()
    {
        var field = typeof(Player).GetProperty("maxGlideDuration",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public);

        if (field != null)
            return (float)field.GetValue(player);

        return 3.0f; 
    }

    float GetPlayerGlideDurationLeft()
    {
        var field = typeof(Player).GetField("glideDurationLeft",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public);

        if (field != null)
            return (float)field.GetValue(player);

        return 0f;
    }

    bool GetPlayerIsGliding()
    {
        var field = typeof(Player).GetField("isRealyGliding",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
            return (bool)field.GetValue(player);

        return false;
    }

    bool GetPlayerIsGrounded()
    {
        return player.isGrounded();
    }
}
