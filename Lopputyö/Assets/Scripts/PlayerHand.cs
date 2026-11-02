using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [Header("Setup")]
    public Transform handSlot;            
    public int maxItems = 6;

    [Header("Keys")]
    public KeyCode dropKey = KeyCode.G;

    public List<Item> inventory = new List<Item>();
    public Item CurrentItem { get; private set; }
    int currentIndex = -1;

    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) ToggleEquip(i);
        }

        // Mouse wheel cycles items
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) Cycle(1);
        else if (scroll < 0f) Cycle(-1);

        if (Input.GetKeyDown(dropKey)) DropCurrent();


    }

    public bool AddItem(Item item)
    {
        if (inventory.Count >= maxItems) { Debug.Log("Inventory full"); return false; }

        inventory.Add(item);
        item.OnPickedUp();

        if (CurrentItem == null) Equip(inventory.Count - 1);
        return true;
    }

    public void Equip(int index)
    {
        if (index < 0 || index >= inventory.Count) return;

        if (CurrentItem != null) CurrentItem.OnUnequipped();

        currentIndex = index;
        CurrentItem = inventory[index];
        CurrentItem.OnEquipped(handSlot);
    }

    void ToggleEquip(int index)
    {
        if (index >= inventory.Count) return;

        if (index == currentIndex)
        {
            CurrentItem.OnUnequipped();
            CurrentItem = null;
            currentIndex = -1;
        }
        else Equip(index);
    }

    void Cycle(int dir)
    {
        if (inventory.Count == 0) return;
        int next = (currentIndex + dir + inventory.Count) % inventory.Count;
        Equip(next);
    }

    void DropCurrent()
    {
        if (CurrentItem == null) return;

        Item dropped = CurrentItem;
        inventory.RemoveAt(currentIndex);
        CurrentItem = null;
        currentIndex = -1;

        dropped.OnDropped(handSlot.position, handSlot.forward * 3f);

        if (inventory.Count > 0) Equip(0);
    }
}
