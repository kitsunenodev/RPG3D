using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellCollision : MonoBehaviour {
    
    public float spellDamage;

    // Use this for initialization 
    void Start () {
        Destroy(gameObject, 10);
    }

    void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<EnemyAI>().ApplyDammage(spellDamage);
        }

        if (!col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}