using UnityEngine;
using System.Collections;

public class WorkingStation : MonoBehaviour
{

    public KeyCode openInventory;
    public GameObject craftSystem;
    public int distanceToOpenWorkingStation = 3;
    bool showCraftSystem;
    Inventory craftInventory;
    CraftSystem cS;


    // Use this for initialization
    void Start()
    {
        if (craftSystem != null)
        {
            craftInventory = craftSystem.GetComponent<Inventory>();
            cS = craftSystem.GetComponent<CraftSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(this.gameObject.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (Input.GetKeyDown(openInventory) && distance <= distanceToOpenWorkingStation)
        {
            showCraftSystem = !showCraftSystem;
            if (showCraftSystem)
            {
                craftInventory.OpenInventory();
            }
            else
            {
                cS.backToInventory();
                craftInventory.CloseInventory();
            }
        }
        if (showCraftSystem && distance > distanceToOpenWorkingStation)
        {
            cS.backToInventory();
            craftInventory.CloseInventory();
        }


    }
}
