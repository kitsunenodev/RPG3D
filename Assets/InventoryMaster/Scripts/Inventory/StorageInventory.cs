﻿using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StorageInventory : MonoBehaviour
{

    [SerializeField]
    public GameObject inventory;

    [SerializeField]
    public List<Item> storageItems = new List<Item>();

    [SerializeField]
    private ItemDataBaseList itemDatabase;

    [SerializeField]
    public int distanceToOpenStorage;

    public float timeToOpenStorage;

    private InputManager inputManagerDatabase;

    float startTimer;
    float endTimer;
    bool showTimer;

    public int itemAmount;

    Tooltip tooltip;
    Inventory inv;

    GameObject player;

    static Image timerImage;
    static GameObject timer;

    bool closeInv;

    bool showStorage;

    public bool isChest;

    public void AddItemToStorage(int id, int value)
    {
        Item item = itemDatabase.getItemByID(id);
        item.itemValue = value;
        storageItems.Add(item);
    }

    void Start()
    {
        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");

        player = GameObject.FindGameObjectWithTag("Player");
        inv = inventory.GetComponent<Inventory>();
        ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        int creatingItemsForChest = 0;

        int randomItemAmount = Random.Range(1, itemAmount);

        while (creatingItemsForChest < randomItemAmount)
        {
            int randomItemNumber = Random.Range(1, inventoryItemList.itemList.Count - 1);
            int raffle = Random.Range(1, 100);

            if (raffle <= inventoryItemList.itemList[randomItemNumber].rarity)
            {
                int randomValue = Random.Range(1, inventoryItemList.itemList[randomItemNumber].getCopy().maxStack);
                Item item = inventoryItemList.itemList[randomItemNumber].getCopy();
                item.itemValue = randomValue;
                storageItems.Add(item);
                creatingItemsForChest++;
            }
        }

        if (GameObject.FindGameObjectWithTag("Timer") != null)
        {
            timerImage = GameObject.FindGameObjectWithTag("Timer").GetComponent<Image>();
            timer = GameObject.FindGameObjectWithTag("Timer");
            timer.SetActive(false);
        }
        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();

    }

    public void SetImportantVariables()
    {
        if (itemDatabase == null)
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }

    void Update()
    {

        float distance = Vector3.Distance(this.gameObject.transform.position, player.transform.position);

        if (showTimer)
        {
            if (timerImage != null)
            {
                timer.SetActive(true);
                float fillAmount = (Time.time - startTimer) / timeToOpenStorage;
                timerImage.fillAmount = fillAmount;
            }
        }

        if (distance <= distanceToOpenStorage && Input.GetKeyDown(inputManagerDatabase.StorageKeyCode))
        {
            showStorage = !showStorage;
            StartCoroutine(OpenInventoryWithTimer());
        }

        if (distance > distanceToOpenStorage && showStorage)
        {
            if (isChest)
            {
                gameObject.GetComponent<Animator>().SetTrigger("Activate");
            }

            showStorage = false;
            if (inventory.activeSelf)
            {
                storageItems.Clear();
                SetListOfStorage();
                inventory.SetActive(false);
                inv.DeleteAllItems();
            }
            tooltip.deactivateTooltip();
            timerImage.fillAmount = 0;
            timer.SetActive(false);
            showTimer = false;
        }
    }

    IEnumerator OpenInventoryWithTimer()
    {
        if (showStorage)
        {
            startTimer = Time.time;
            showTimer = true;
            yield return new WaitForSeconds(timeToOpenStorage);
            if (showStorage)
            {

                if (isChest)
                {
                    gameObject.GetComponent<Animator>().SetTrigger("Activate");
                }

                inv.ItemsInInventory.Clear();
                inventory.SetActive(true);
                AddItemsToInventory();
                showTimer = false;
                if (timer != null)
                    timer.SetActive(false);
            }
        }
        else
        {
            if (isChest)
            {
                gameObject.GetComponent<Animator>().SetTrigger("Activate");
            }

            storageItems.Clear();
            SetListOfStorage();
            inventory.SetActive(false);
            inv.DeleteAllItems();
            tooltip.deactivateTooltip();
        }


    }



    void SetListOfStorage()
    {
        Inventory storage = inventory.GetComponent<Inventory>();
        storageItems = storage.GetItemList();
    }

    void AddItemsToInventory()
    {
        Inventory iV = inventory.GetComponent<Inventory>();
        for (int i = 0; i < storageItems.Count; i++)
        {
            iV.AddItemToInventory(storageItems[i].itemID, storageItems[i].itemValue);
        }
        iV.StackableSettings();
    }






}
