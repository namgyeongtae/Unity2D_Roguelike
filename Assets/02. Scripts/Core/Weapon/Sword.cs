using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    public override void Attack(Vector2 direction)
    {
        // Default State 애니메이션 재생
        animator.SetTrigger("Swing");
        
        StartCoroutine(SwingSword(direction));
    }

    private IEnumerator SwingSword(Vector2 direction)
    {
        // direction을 축으로 하여 -90~90도로 휘두르기
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector2 curDirection = Vector2.zero;
        float currentAngle = 0f;

        currentAngle = angle - 90f;
        curDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        yield return new WaitForSeconds(0.2f);

        currentAngle = angle + 90f;
        curDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Entity enemy = other.gameObject.GetComponent<Entity>();
            enemy.TakeDamage(owner, this, owner.Stats.GetStat(damageData).Value);
        }
    }
}
