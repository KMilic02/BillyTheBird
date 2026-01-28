using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Complementary UI script that displays player health as hearts.
/// Attach this to the "Player Health" container GameObject in the scene.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the player (auto-found if not assigned)")]
    public Player player;

    [Header("Settings")]
    [Tooltip("How to hide lost health (Invisible keeps layout, Disable removes from layout)")]
    public HideMode hideMode = HideMode.Invisible;

    public enum HideMode
    {
        Invisible,  // Makes CanvasGroup alpha = 0 (keeps position)
        Disable     // Disables GameObject (may shift remaining hearts)
    }

    private CanvasGroup[] healthCanvasGroups;
    private GameObject[] healthObjects;
    private int lastHealth = -1;
    private int maxHealth = -1;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        int childCount = transform.childCount;
        healthCanvasGroups = new CanvasGroup[childCount];
        healthObjects = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            healthObjects[i] = transform.GetChild(i).gameObject;

            if (hideMode == HideMode.Invisible)
            {
                healthCanvasGroups[i] = healthObjects[i].GetComponent<CanvasGroup>();
                if (healthCanvasGroups[i] == null)
                {
                    healthCanvasGroups[i] = healthObjects[i].AddComponent<CanvasGroup>();
                }
            }
        }

        // Initialize health display
        if (player != null)
        {
            maxHealth = GetPlayerMaxHealth();
            lastHealth = GetPlayerHealth();
            UpdateHealthDisplay();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        int currentHealth = GetPlayerHealth();
        if (currentHealth != lastHealth)
        {
            UpdateHealthDisplay();
            lastHealth = currentHealth;
        }
    }

    void UpdateHealthDisplay()
    {
        int currentHealth = GetPlayerHealth();

        // Health display offset: show (health + 1) hearts (maybe?)
        
        int heartsToShow = currentHealth + 1;


        for (int i = 0; i < healthObjects.Length; i++)
        {
            bool shouldShow = i < heartsToShow;

            if (hideMode == HideMode.Invisible)
            {

                if (healthCanvasGroups[i] != null)
                {
                    healthCanvasGroups[i].alpha = shouldShow ? 1f : 0f;
                }
            }
            else
            {

                healthObjects[i].SetActive(shouldShow);
            }
        }
    }

    int GetPlayerHealth()
    {
        var property = typeof(Player).GetProperty("health",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (property != null)
            return (int)property.GetValue(player);

        return 0;
    }

    int GetPlayerMaxHealth()
    {

        if (player != null)
        {
            int currentHealth = GetPlayerHealth();

            return currentHealth;
        }

        return 3; 
    }
}
