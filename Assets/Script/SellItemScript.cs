using UnityEngine;

public class SellItemScript : MonoBehaviour {

    CharacterMotor charMotor;
    PlayerInventory playerInv;
    Tooltip tooltip;

    // Use this for initialization
    void Start () {
        charMotor = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMotor>();
        playerInv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
    }
	
    public void SellItem(){
        if (Input.GetKey(KeyCode.LeftShift) && charMotor.isInShop)
        {
            playerInv.goldCoins += GetComponent<ItemOnObject>().item.itemPrice;
            tooltip.deactivateTooltip();
            Destroy(gameObject);
        }
    }
}