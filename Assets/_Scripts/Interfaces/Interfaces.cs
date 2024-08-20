using UnityEngine;

public interface IDamageable
{
    void TakeDamage (int Damage);
}

public interface IInvincible
{
    void Invincible ();
}

public interface ITargeted
{
    void Targeted (RaycastHit raycastHit);
}

public interface IShoot
{
    void Shoot ();
}

public interface ISpawn
{
    void Spawn ();
}

public interface ICollectible
{
    void Collected ();
}