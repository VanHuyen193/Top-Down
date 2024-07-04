using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        // Hiển thị UI inventory
        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if(item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            //Debug.Log(string.Format("Assigned {0} to player"), item);
        }

        // Xóa ô bằng cách loại bỏ item được gán và vô hiệu hóa UI
        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        // Kiểm tra xem ô có trống (không có item nào được gán) hay không
        public bool IsEmpty()
        {
            return item == null;
        }
    }

    // Danh sách các ô vũ khí
    public List<Slot> weaponSlots = new List<Slot>(6);
    // Danh sách các ô vật phẩm
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    // Danh sách các dữ liệu vũ khí có sẵn
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    // Danh sách các dữ liệu vật phẩm có sẵn
    public List<PassiveData> availablePassives = new List<PassiveData>();
    // Danh sách các tùy chọn UI nâng cấp
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public bool Has(ItemData type)
    {
        // Kiểm tra xem có item loại type hay không
        return Get(type);
    }
    public Item Get(ItemData type)
    {
        // Kiểm tra loại dữ liệu item và gọi phương thức tương ứng
        if (type is WeaponData)
        {
            return Get(type as WeaponData);
        }
        else if(type is PassiveData)
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    // Find a passive of certain type in the inventory
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if(p && p.data == type)
            {
                return p; 
            }
        }
        return null;
    }

    // Find a weapon of certain type in the inventory
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
            {
                return w; 
            }
        }
        return null;
    }

    // Remove a weapon of particular type
    public bool Remove(WeaponData data, bool removeUpgradeAvailablility = false)
    {
        // Remove this weapon from the upgrade pool
        if (removeUpgradeAvailablility)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear(); // Xóa vũ khí khỏi ô trong danh sách vũ khí của người chơi
                w.OnUnequip();
                Destroy(w.gameObject); // Hủy game object của vũ khí
                return true;
            }
        }
        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailablility = false)
    {
        // Remove this passive from the upgrade pool
        if (removeUpgradeAvailablility)
        {
            availablePassives.Remove(data);
        }
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear(); // Xóa vật phẩm khỏi ô trong danh sách vật phẩm của người chơi
                p.OnUnequip();
                Destroy(p.gameObject); // Hủy game object của vật phẩm
                return true;
            }
        }
        return false;
    }

    // Xóa UI và game object của item
    public bool Remove(ItemData data, bool removeUpgradeAvailablility = false)
    {
        if(data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailablility);
        }
        else if(data is WeaponData)
        {
            Remove(data as WeaponData, removeUpgradeAvailablility);
        }
        return false;
    }

    public int Add(WeaponData data)
    {
        // Tìm vị trí trống trong danh sách weaponSlots
        int slotNum = -1;
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // Nếu không có vị trí trống, trả về -1
        if (slotNum < 0)
        {
            return slotNum;
        }

        // Lấy loại vũ khí từ behaviour
        Type weaponType = Type.GetType(data.behaviour);

        // Nếu loại vũ khí hợp lệ, tiến hành tạo vũ khí
        if (weaponType != null)
        {
            // Tạo một game object mới để chứa vũ khí
            GameObject go = new GameObject(data.baseStats.name + " Controller");

            // Thêm component Weapon vào game object và khởi tạo
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            // Hiển thị UI inventory
            weaponSlots[slotNum].Assign(spawnedWeapon);

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
            return slotNum;
        }
        else
        {
            Debug.Log("Invailed weapon type specified");
        }

        return -1;
    }

    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Tìm kiếm một slot trống trong danh sách passiveSlots
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // Nếu không tìm thấy slot trống, trả về -1
        if (slotNum < 0)
        {
            return slotNum;
        }

        // Tạo game object mới cho Passive item
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        // Hiển thị UI inventory
        passiveSlots[slotNum].Assign(p);

        // Nếu GameManager đang ở trạng thái chọn nâng cấp, kết thúc quá trình chọn nâng cấp
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        // Tính toán lại chỉ số của người chơi để phản ánh các thay đổi do passive item mới
        player.RecalculateStats();

        // Trả về số slot đã được sử dụng để lưu passive item mới
        return slotNum;
    }

    // If we don't know what item is being added, this function will determine that
    public int Add(ItemData data)
    {
        if (data is WeaponData)
            return Add(data as WeaponData);

        else if (data is PassiveData)
            return Add(data as PassiveData);

        return -1;
    }

    //public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    //{
    //    if(weaponSlots.Count > slotIndex)
    //    {
    //        Weapon weapon = weaponSlots[slotIndex].item as Weapon;
    //
    //        if (!weapon.DoLevelUp())
    //        {
    //            Debug.LogWarning("Failed to level up");
    //            return;
    //        }
    //    }
    //
    //    if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
    //    {
    //        GameManager.instance.EndLevelUp();
    //    }
    //}

    // Overload so that we can use both ItemData or Item to level up an item in the inventory
    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if (item) return LevelUp(item);
        return false;
    }

    // Levels up a selected weapon in the player inventory
    public bool LevelUp(Item item)
    {
        // Tries to level up the item
        if (!item.DoLevelUp())
        {
            Debug.LogWarning("Failed to level up");
            return false;
        }

        // Close the level up screen afterwards.
        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        // If it is as passive, recaculate player stats.
        if (item is Passive) player.RecalculateStats();
        return true;

    }

    //public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    //{
    //    if (passiveSlots.Count > slotIndex)
    //    {
    //        Passive p = passiveSlots[slotIndex].item as Passive;
    //
    //        // Don't level up the weapon if it is already at max level
    //        if (!p.DoLevelUp())
    //        {
    //            Debug.LogWarning("Failed to level up");
    //            return;
    //        }
    //    }
    //
    //    if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
    //    {
    //        GameManager.instance.EndLevelUp();
    //    }
    //    player.RecalculateStats();
    //}

    // Check a list of slots to see if there are any slots left.
    int GetSlotsLeft(List<Slot> slots)
    {
        int count = 0;
        foreach(Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    // void ApplyUpgradeOptions()
    // {
    //     List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
    //     List<PassiveData> availablePassiveUpgrades = new List<PassiveData>(availablePassives);
    // 
    //     foreach (UpgradeUI upgradeOption in upgradeUIOptions)
    //     {
    //         if (availableWeaponUpgrades.Count == 0 && availablePassiveUpgrades.Count == 0)
    //         {
    //             return;
    //         }
    // 
    //         int upgradeType;
    // 
    //         if (availableWeaponUpgrades.Count == 0)
    //         {
    //             upgradeType = 2;
    //         }
    //         else if (availablePassiveUpgrades.Count == 0)
    //         {
    //             upgradeType = 1;
    //         }
    //         else
    //         {
    //             upgradeType = UnityEngine.Random.Range(1, 3);
    //         }
    // 
    //         if (upgradeType == 1)
    //         {
    //             WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
    //             availableWeaponUpgrades.Remove(chosenWeaponUpgrade);
    // 
    //             if (chosenWeaponUpgrade != null)
    //             {
    //                 EnableUpgradeUI(upgradeOption);
    // 
    //                 bool isLevelUp = false;
    //                 for (int i = 0; i < weaponSlots.Count; i++)
    //                 {
    //                     Weapon w = weaponSlots[i].item as Weapon;
    // 
    //                     if (w != null && w.data == chosenWeaponUpgrade)
    //                     {
    //                         if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
    //                         {
    //                             DisableUpgradeUI(upgradeOption);
    //                             isLevelUp = false;
    //                             break; 
    //                         }
    // 
    //                         isLevelUp = true;
    //                         upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
    //                         Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
    //                         upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
    //                         upgradeOption.upgradeNameDisplay.text = nextLevel.name;
    //                         upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
    //                         break;
    //                     }
    //                 }
    // 
    //                 if (!isLevelUp)
    //                 {
    //                     upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
    //                     upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
    //                     upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
    //                     upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
    //                 }
    //             }
    //         }
    //         else if (upgradeType == 2)
    //         {
    //             PassiveData chosenPassiveUpgrade = availablePassiveUpgrades[UnityEngine.Random.Range(0, availablePassiveUpgrades.Count)];
    //             availablePassiveUpgrades.Remove(chosenPassiveUpgrade);
    // 
    //             if (chosenPassiveUpgrade != null)
    //             {
    //                 EnableUpgradeUI(upgradeOption);
    // 
    //                 bool isLevelUp = false;
    //                 for (int i = 0; i < passiveSlots.Count; i++)
    //                 {
    //                     Passive p = passiveSlots[i].item as Passive;
    // 
    //                     if (p != null && p.data == chosenPassiveUpgrade)
    //                     {
    //                         if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
    //                         {
    //                             DisableUpgradeUI(upgradeOption);
    //                             isLevelUp = false;
    //                             break; 
    //                         }
    // 
    //                         isLevelUp = true;
    //                         upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
    //                         Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
    //                         upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
    //                         upgradeOption.upgradeNameDisplay.text = nextLevel.name;
    //                         upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
    //                         break;
    //                     }
    //                 }
    // 
    //                 if (!isLevelUp)
    //                 {
    //                     upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
    //                     upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveUpgrade.baseStats.description;
    //                     upgradeOption.upgradeNameDisplay.text = chosenPassiveUpgrade.baseStats.name;
    //                     upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
    //                 }
    //             }
    //         }
    //     }
    // }

    // Detemines what upgrade options should appear
    void ApplyUpgradeOptions()
    {
        // <availableUpgrade> is an empty list that will be filtered from
        // <allUpgrade>, which is the list of All upgrades in PlayerInventory
        List<ItemData> availableUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        // We need to know how many weapon / passive slots are left
        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        // Filters through the available weapons and passives and add those that can possibly be an option
        foreach(ItemData data in allUpgrades)
        {
            // If a weapon of this type exists, allow for the upgrade if the level of the weapon is not already maxed out.
            Item obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel) availableUpgrades.Add(data);
            }
            else
            {
                // If we don't have this item in the inventory yet, check if we still have enough slots to take new item.
                if (data is WeaponData && weaponSlotsLeft > 0) availableUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotsLeft > 0) availableUpgrades.Add(data);
            }
        }

        // Iterate through each slot in the upgrade UI and populate the options.
        foreach(UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades, them we abort
            if (availableUpgrades.Count <= 0) return;

            // Pick an upgrade, then remove it so that we don't get it twice
            ItemData chosenUpgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
            availableUpgrades.Remove(chosenUpgrade);

            // Ensure that selected weapon data is valid
            if(chosenUpgrade != null)
            {
                // Turns on the UI slot
                EnableUpgradeUI(upgradeOption);

                // If our inventory already has the upgrade, we will make it a level up
                Item item = Get(chosenUpgrade);
                if (item)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(item));
                    if(item is Weapon)
                    {
                        Weapon.Stats nextLevel = ((WeaponData)chosenUpgrade).GetLevelData(item.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                    else
                    {
                        Passive.Modifier nextLevel = ((PassiveData)chosenUpgrade).GetLevelData(item.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                }
                else
                {
                    if(chosenUpgrade is WeaponData)
                    {
                        WeaponData data = chosenUpgrade as WeaponData;
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.name;
                        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = data.icon;
                    }
                    else
                    {
                        PassiveData data = chosenUpgrade as PassiveData;
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.name;
                        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = data.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach(UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }
    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }
    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
