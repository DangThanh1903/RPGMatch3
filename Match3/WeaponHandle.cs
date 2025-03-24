using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponHandle : MonoBehaviour{
    public Weapon type;

    public void SetType(Weapon type) {
        this.type = type;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }
    public void DestroyBlock() {
        Debug.Log($"Weapon Block Destroyed! Damage: {type.damage}");
        EventBus.TriggerWeaponDestroyed(type.damage); // Notify through MatchManager
        Destroy(gameObject);
    }

    public Weapon GetTypes() => type;
}
