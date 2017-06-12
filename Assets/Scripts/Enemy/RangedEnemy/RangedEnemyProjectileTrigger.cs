﻿using UnityEngine;

public class RangedEnemyProjectileTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Helpers.Tags.Player)
        {
            collider.gameObject.GetComponent<PlayerDamageManager>().TakeDamage(1);
            Destroy(this.gameObject);
        }
        else
            Debug.Log("The object that was hit wasn't tagged as player");
    }
}