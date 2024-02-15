using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject characterSystem;
    public GameObject craftSystemGO;
    private Inventory craftSystemInventory;
    private CraftSystem craftSystem;
    private Inventory mainInventory;
    private Inventory characterSystemInventory;
    private Tooltip toolTip;
    private InputManager inputManagerDatabase;

    Image hpImage;
    Image manaImage;

    [HideInInspector]
    public float maxHealth = 100;
    public float maxMana = 100;
    float maxDamage = 0;
    float maxArmor = 0;

    public float currentHealth = 60;
    public float currentMana = 100;
    public float currentDamage = 0;
    public float currentArmor = 0;

    // Gold section 
    public int goldCoins;
    Text goldText;

    int normalSize = 3;

    // experience section
    Image experienceBar;
    Text playerLevelTxt;
    public int playerLevel = 1;
    public float currentXP;
    public float maxXP = 100;
    public float rateXP;
    private PlayerSkills playerSkills;
    public CharacterMotor characterMotor;
    public Animation playerAnimations;

    public void OnEnable()
    {
        Inventory.ItemEquip += OnBackpack;
        Inventory.UnEquipItem += UnEquipBackpack;

        Inventory.ItemEquip += OnGearItem;
        Inventory.ItemConsumed += OnConsumeItem;
        Inventory.UnEquipItem += OnUnEquipItem;

        Inventory.ItemEquip += EquipWeapon;
        Inventory.UnEquipItem += UnEquipWeapon;
    }

    public void OnDisable()
    {
        Inventory.ItemEquip -= OnBackpack;
        Inventory.UnEquipItem -= UnEquipBackpack;

        Inventory.ItemEquip -= OnGearItem;
        Inventory.ItemConsumed -= OnConsumeItem;
        Inventory.UnEquipItem -= OnUnEquipItem;

        Inventory.UnEquipItem -= UnEquipWeapon;
        Inventory.ItemEquip -= EquipWeapon;
    }

    void EquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //add the weapon if you unequip the weapon
        }
    }

    void UnEquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //delete the weapon if you unequip the weapon
        }
    }

    void OnBackpack(Item item)
    {
        if (item.itemType == ItemType.Backpack)
        {
            for (int i = 0; i < item.itemAttributes.Count; i++)
            {
                if (mainInventory == null)
                    mainInventory = inventory.GetComponent<Inventory>();
                mainInventory.SortItems();
                if (item.itemAttributes[i].attributeName == "Slots")
                    changeInventorySize(item.itemAttributes[i].attributeValue);
            }
        }
    }

    void UnEquipBackpack(Item item)
    {
        if (item.itemType == ItemType.Backpack)
            changeInventorySize(normalSize);
    }

    void changeInventorySize(int size)
    {
        dropTheRestItems(size);

        if (mainInventory == null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (size == 3)
        {
            mainInventory.width = 3;
            mainInventory.height = 1;
            mainInventory.UpdateSlotAmount();
            mainInventory.AdjustInventorySize();
        }
        if (size == 6)
        {
            mainInventory.width = 3;
            mainInventory.height = 2;
            mainInventory.UpdateSlotAmount();
            mainInventory.AdjustInventorySize();
        }
        else if (size == 12)
        {
            mainInventory.width = 4;
            mainInventory.height = 3;
            mainInventory.UpdateSlotAmount();
            mainInventory.AdjustInventorySize();
        }
        else if (size == 16)
        {
            mainInventory.width = 4;
            mainInventory.height = 4;
            mainInventory.UpdateSlotAmount();
            mainInventory.AdjustInventorySize();
        }
        else if (size == 24)
        {
            mainInventory.width = 6;
            mainInventory.height = 4;
            mainInventory.UpdateSlotAmount();
            mainInventory.AdjustInventorySize();
        }
    }

    void dropTheRestItems(int size)
    {
        if (size < mainInventory.ItemsInInventory.Count)
        {
            for (int i = size; i < mainInventory.ItemsInInventory.Count; i++)
            {
                GameObject dropItem = (GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemModel);
                dropItem.AddComponent<PickUpItem>();
                dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i];
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
            }
        }
    }

    void Start()
    {
        hpImage = GameObject.Find("currentHP").GetComponent<Image>();
        manaImage = GameObject.Find("currentMana").GetComponent<Image>();
        playerSkills = gameObject.GetComponent<PlayerSkills>();
        experienceBar = GameObject.Find("currentXP").GetComponent<Image>();
        playerLevelTxt = GameObject.Find("PlayerLevelText").GetComponent<Text>();
        goldText = GameObject.Find("GoldText").GetComponent<Text>();
        characterMotor = gameObject.GetComponent<CharacterMotor>();
        playerAnimations = gameObject.GetComponent<Animation>();

        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");

        if (craftSystemGO != null)
            craftSystem = craftSystemGO.GetComponent<CraftSystem>();

        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            toolTip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
        if (inventory != null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (characterSystem != null)
            characterSystemInventory = characterSystem.GetComponent<Inventory>();
        if (craftSystemGO != null)
            craftSystemInventory = craftSystemGO.GetComponent<Inventory>();
    }

    public void ApplyDamage(float TheDamage)
    {
        if (!characterMotor.isDead)
        {
            // la fameuse équation : PDV = PDV -(damage - ((armor * damage) / 100))
            currentHealth -= (TheDamage - ((currentArmor * TheDamage) / 100));

            if (currentHealth <= 0)
            {
                Dead();
            }
        }
    }

    public void Dead() {
        // On désactive la possibilité de déplacer son personnage lorsqu'il meurt
        characterMotor.isDead = true;
        playerAnimations.Play("diehard");
    }

    public void OnConsumeItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
            {
                if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth)
                    currentHealth = maxHealth;
                else
                    currentHealth += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Mana")
            {
                if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana)
                    currentMana = maxMana;
                else
                    currentMana += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Armor")
            {
                if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
                    currentArmor = maxArmor;
                else
                    currentArmor += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Damage")
            {
                if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage)
                    currentDamage = maxDamage;
                else
                    currentDamage += item.itemAttributes[i].attributeValue;
            }
        }

    }

    public void OnGearItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
                currentHealth += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mana")
                currentMana += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Armor")
                currentArmor += item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Damage")
                currentDamage += item.itemAttributes[i].attributeValue;
        }

    }

    public void OnUnEquipItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attributeName == "Health")
                currentHealth -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Mana")
                currentMana -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Armor")
                currentArmor -= item.itemAttributes[i].attributeValue;
            if (item.itemAttributes[i].attributeName == "Damage")
                currentDamage -= item.itemAttributes[i].attributeValue;
        }

    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = "Gold : " + goldCoins;

        // + 50 xp 
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            currentXP += 50;
        }*/

        // Si on a assez d'XP
        
        if(currentXP >= maxXP)
        {
            float reste = currentXP - maxXP;
            playerLevel += 1;
            playerSkills.availablePoints += 1;
            playerLevelTxt.text = "Player Level : " + playerLevel;
            currentXP = 0 + reste;
            maxXP *= rateXP;
        }

        // Pour la barre d'XP
        float percentageXP = ((currentXP * 100) / maxXP) / 100;
        Debug.Log(percentageXP);
        Debug.Log(experienceBar);
        experienceBar.fillAmount = percentageXP;

        // empecher la vie actuelle d'etre supérieur à la vie max
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Pour la barre de vie
        float percentageHP = ((currentHealth * 100) / maxHealth) / 100;
        hpImage.fillAmount = percentageHP;

        // Pour la barre de mana
        float percentageMana = ((currentMana * 100) / maxMana) / 100;
        manaImage.fillAmount = percentageMana;

        if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
        {
            if (!characterSystem.activeSelf)
            {
                characterSystemInventory.OpenInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                characterSystemInventory.CloseInventory();
            }
        }

        if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
        {
            if (!inventory.activeSelf)
            {
                mainInventory.OpenInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                mainInventory.CloseInventory();
            }
        }

        if (Input.GetKeyDown(inputManagerDatabase.CraftSystemKeyCode))
        {
            if (!craftSystemGO.activeSelf)
                craftSystemInventory.OpenInventory();
            else
            {
                if (craftSystem != null)
                    craftSystem.backToInventory();
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                craftSystemInventory.CloseInventory();
            }
        }

    }

}
