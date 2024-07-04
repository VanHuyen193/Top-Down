using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set { actualStats = value; }
    }

    float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value; // Cập nhật giá trị heatlh trong thời gian thực
                UpdateExpBar();
            }
        }
    }
    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect; // If damage is dealt.
    public ParticleSystem blockedEffect; // If armor completely blocks damage

    // Kinh nghiệm và level của người chơi
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class levelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<levelRange> levelRanges;

    PlayerCollector collector;
    PlayerInventory inventory;
    public int weaponIndex = 0;
    public int passiveItemIndex = 0;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMPro.TextMeshProUGUI levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();

        if(CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        baseStats = actualStats = characterData.stats;
        collector.SetRadious(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    void Start()
    {
        inventory.Add(characterData.StartingWeapon);

        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if(isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        // Start with the base stats of the player
        actualStats = baseStats;

        // Iterate through all passive slots in the player's inventory
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            // Check if the slot contains a Passive item
            Passive p = s.item as Passive;
            if (p)
            {
                // If a Passive item is found, add its boosts to the actual stats
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadious(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach(levelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LV " + level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            // Take armor into account before dealing the damage
            dmg -= actualStats.armor;

            if(dmg > 0)
            {
                // Deal the damage
                CurrentHealth -= dmg;

                // If there is a damage effect assigned, play it
                if (damageEffect)
                {
                    Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 2f);
                }

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // If there is a blocked effect assigned, play it
                if (blockedEffect)
                    Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            UpdateHealthBar();
        }
    }

    void UpdateHealthBar()
    {
        // Update the health bar
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if(CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    void Recover()
    {
        if(CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            
            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    
}
