using UnityEngine;

[System.Serializable]
public class ProjectileAttackAction : AttackAction
{
    [SerializeField] private GameObject _projectilePrefab;

    public override void Apply(Entity entity, Entity target, float damage)
    {
        var projectile = GameObject.Instantiate(_projectilePrefab, entity.transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Setup(entity, target, damage);
    }
}
