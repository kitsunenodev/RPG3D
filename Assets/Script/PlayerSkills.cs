using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour {
    
    public GameObject UIPannel;
    public Text pointsText;

    public int availablePoints;
    public string openKey;

    private bool isOpen;
    private PlayerInventory playerinv;

    // Use this for initialization
    void Start () {
        playerinv = gameObject.GetComponent<PlayerInventory>();
    }
	
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(openKey))
        {
            isOpen = !isOpen;
        }

        if (isOpen)
        {
            UIPannel.SetActive(true);
            pointsText.text = "Points disponibles : " + availablePoints;
        }
        else
        {
            UIPannel.SetActive(false);
        }
    }

    public void AddMaxHealth(float amountHp)
    {
        if(availablePoints >= 1)
        {
            playerinv.maxHealth += amountHp;
            playerinv.currentHealth = playerinv.maxHealth;
            availablePoints -= 1;
        }
    }
    
    public void AddMaxMana(float amountMana)
    {
        if(availablePoints >= 1)
        {
            playerinv.maxMana += amountMana;
            playerinv.currentMana = playerinv.maxMana;
            availablePoints -= 1;
        }
    }
}