using System.Collections.Generic;
using UnityEngine;

// Put this on the player. It holds the inventory and the item in hand.
public class PlayerHand : MonoBehaviour
{
    [Header("Setup")]
    public Transform handSlot;            // empty child of your camera/arm where items are held
    public int maxItems = 6;

    [Header("Keys")]
    public KeyCode dropKey = KeyCode.G;

    public List<Item> inventory = new List<Item>();
    public Item CurrentItem { get; private set; }
    int currentIndex = -1;

    void Update()
    {
        // 1-9 equips that inventory slot (press again to put away)
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) ToggleEquip(i);
        }

        // Mouse wheel cycles items
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) Cycle(1);
        else if (scroll < 0f) Cycle(-1);

        if (Input.GetKeyDown(dropKey)) DropCurrent();

        // Debug: tells us what happens when you click
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentItem == null)
                Debug.Log("PlayerHand: clicked but nothing is in hand. Inventory count: " + inventory.Count);
            else
                Debug.Log("PlayerHand: clicked, holding " + CurrentItem.name + " (script type: " + CurrentItem.GetType().Name + ")");
        }

        // Use the held item
        if (CurrentItem != null)
        {
            if (Input.GetMouseButtonDown(0))
                CurrentItem.Use();
        }
    }

    public bool AddItem(Item item)
    {
        if (inventory.Count >= maxItems) { Debug.Log("Inventory full"); return false; }

        inventory.Add(item);
        item.OnPickedUp();

        // Auto-equip if hands are empty
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