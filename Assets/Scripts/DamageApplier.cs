using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(EventsHandler))]
public class DamageApplier : MonoBehaviour
{
    [SerializeField] private float _damage;                     /// <summary>Damage to apply.</summary>
    [SerializeField] private GameObjectTag[] _affectedTags;     /// <summary>Tags of GameObjects that are affected by this DamageApplier.</summary>

    /// <summary>Gets and Sets damage property.</summary>
    public float damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    /// <summary>Gets and Sets affectedTags property.</summary>
    public GameObjectTag[] affectedTags
    {
        get { return _affectedTags; }
        set { _affectedTags = value; }
    }

    /// <summary>Applies Damage to given Health's Instance.</summary>
    /// <param name="_health">Health Instance to inflict damage to.</param>
    /// <param name="_damageScale">Damage's Scale [1.0f by default].</param>
    public void ApplyDamage(Health _health, float _damageScale = 1.0f)
    {
        if(_health != null) _health.GiveDamage(damage * _damageScale, true, gameObject);
    }
}
}