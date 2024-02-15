using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    public Inventory inventoryPlayer;
    public PlayerInventory playerInventory;
    public CharacterMotor characterMotor;
    public GameObject shopPanel;
    public ItemDataBaseList itemDb;

    [Header("ID des items du shop")]
    public int item1Id;
    public int item2Id;
    public int item3Id;

    public Text textItem1;
    public Text textItem2;
    public Text textItem3;

    public Image iconItem1;
    public Image iconItem2;
    public Image iconItem3;

    private int amountSlots;
    private int slotsChecked;
    private bool transactionDone;

    // Tous les paramètres des objets du shop
    Item item1;
    Item item2;
    Item item3;

    // Use this for initialization
    void Start () {
        characterMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMotor>();
        shopPanel.SetActive(false);
	}

    void PrepareShop()
    {
        item1 = itemDb.getItemByID(item1Id);
        item2 = itemDb.getItemByID(item2Id);
        item3 = itemDb.getItemByID(item3Id);

        textItem1.text = item1.itemName + " (Prix : " + item1.itemPrice + ") ";
        textItem2.text = item2.itemName + " (Prix : " + item2.itemPrice + ") ";
        textItem3.text = item3.itemName + " (Prix : " + item3.itemPrice + ") ";

        iconItem1.sprite = item1.itemIcon;
        iconItem2.sprite = item2.itemIcon;
        iconItem3.sprite = item3.itemIcon;

        iconItem1.transform.GetComponent<Button>().onClick.AddListener(delegate { BuyItem(item1); });
        iconItem2.transform.GetComponent<Button>().onClick.AddListener(delegate { BuyItem(item2); });
        iconItem3.transform.GetComponent<Button>().onClick.AddListener(delegate { BuyItem(item3); });

        shopPanel.SetActive(true);
    }

    void BuyItem(Item finalItem) {
        amountSlots = inventoryPlayer.transform.GetChild(1).childCount;
        transactionDone = false;
        slotsChecked = 0;

        foreach (Transform child in inventoryPlayer.transform.GetChild(1))
        {
            if(child.childCount == 0 || (child.childCount != 0 && child.GetChild(0).GetComponent<ItemOnObject>().item.itemValue 
                   < child.GetChild(0).GetComponent<ItemOnObject>().item.maxStack))
            {
                if (playerInventory.goldCoins >= finalItem.itemPrice)
                {
                    inventoryPlayer.AddItemToInventory(finalItem.itemID);
                    playerInventory.goldCoins -= finalItem.itemPrice;
                    transactionDone = true;
                    print("Le joueur a acheté l'objet : " + finalItem.itemName);
                    break;
                }
                print("Transaction refusée, le joueur n'a pas assez d'argent.");
            }
            slotsChecked++;
        }

        if (slotsChecked == amountSlots && transactionDone == false) {
            print("Transaction annulée, pas de place dans l'inventaire.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            characterMotor.isInShop = true;
            PrepareShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            iconItem1.GetComponent<Button>().onClick.RemoveAllListeners();
            iconItem2.GetComponent<Button>().onClick.RemoveAllListeners();
            iconItem3.GetComponent<Button>().onClick.RemoveAllListeners();

            shopPanel.SetActive(false);
            characterMotor.isInShop = false;
        }
    }
}
