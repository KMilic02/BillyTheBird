using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BossHealthUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("TextMeshPro component for displaying boss name")]
    public TextMeshProUGUI bossNameText;

    [Tooltip("Container where boss health hearts will be spawned")]
    public Transform bossHealthContainer;

    [Tooltip("Prefab for boss health hearts")]
    public GameObject bossHealthPrefab;

    [Header("Settings")]
    [Tooltip("Maximum number of hearts to display (default: 9)")]
    public int maxHearts = 9;

    [Tooltip("How to hide lost health")]
    public HideMode hideMode = HideMode.Invisible;

    public enum HideMode
    {
        Invisible,  // Makes CanvasGroup alpha = 0 (keeps position)
        Disable     // Disables GameObject (may shift remaining hearts)
    }

    private List<GameObject> spawnedHearts = new List<GameObject>();
    private List<CanvasGroup> heartCanvasGroups = new List<CanvasGroup>();
    private BossTag currentBoss;
    private int lastBossHealth = -1;
    private int bossMaxHealth = 0;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
      
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        BossTag activeBoss = BossTag.activeBoss;

        // Safety check: if currentBoss is destroyed but not null, clean up
        if (currentBoss != null && currentBoss.gameObject == null)
        {
            CleanupBossUI();
            return;
        }

        if (activeBoss != null && currentBoss == null)
        {
            InitializeBossUI(activeBoss);
        }
        else if (activeBoss == null && currentBoss != null)
        {
            CleanupBossUI();
        }
        else if (currentBoss != null)
        {
            int currentHealth = currentBoss.GetHealth();
            if (currentHealth != lastBossHealth)
            {
                UpdateHealthDisplay(currentHealth);
                lastBossHealth = currentHealth;
            }
        }
    }

    void InitializeBossUI(BossTag boss)
    {
        currentBoss = boss;
        bossMaxHealth = boss.GetMaxHealth();
        lastBossHealth = boss.GetHealth();

        Debug.Log($"[BossHealthUI] Initializing Boss UI. Max Health: {bossMaxHealth}, Current Health: {lastBossHealth}");

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (bossNameText != null)
        {
            string bossName = boss.GetBossName();
            bossNameText.text = bossName;
            Debug.Log($"[BossHealthUI] Boss name set to: {bossName}");
        }

        ClearHearts();

        int heartsToSpawn = Mathf.Min(bossMaxHealth, maxHearts);
        Debug.Log($"[BossHealthUI] Spawning {heartsToSpawn} hearts. Container: {bossHealthContainer}, Prefab: {bossHealthPrefab}");
        SpawnHearts(heartsToSpawn);

        UpdateHealthDisplay(lastBossHealth);
    }

    void CleanupBossUI()
    {
        currentBoss = null;
        lastBossHealth = -1;
        bossMaxHealth = 0;

        ClearHearts();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void ClearHearts()
    {
        foreach (GameObject heart in spawnedHearts)
        {
            if (heart != null)
                Destroy(heart);
        }

        spawnedHearts.Clear();
        heartCanvasGroups.Clear();
    }

    void SpawnHearts(int count)
    {
        if (bossHealthContainer == null || bossHealthPrefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            GameObject heart = Instantiate(bossHealthPrefab, bossHealthContainer);
            spawnedHearts.Add(heart);

            if (hideMode == HideMode.Invisible)
            {
                CanvasGroup canvasGroup = heart.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = heart.AddComponent<CanvasGroup>();
                }
                heartCanvasGroups.Add(canvasGroup);
            }
            else
            {
                heartCanvasGroups.Add(null);
            }
        }
    }

    void UpdateHealthDisplay(int currentHealth)
    {
        for (int i = 0; i < spawnedHearts.Count; i++)
        {
            bool shouldShow = i < currentHealth;

            if (hideMode == HideMode.Invisible)
            {
                if (heartCanvasGroups[i] != null)
                {
                    heartCanvasGroups[i].alpha = shouldShow ? 1f : 0f;
                }
            }
            else
            {
                if (spawnedHearts[i] != null)
                {
                    spawnedHearts[i].SetActive(shouldShow);
                }
            }
        }
    }
}
