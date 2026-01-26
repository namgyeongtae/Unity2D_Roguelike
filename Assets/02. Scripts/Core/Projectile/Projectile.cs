using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Entity owner;
    private Entity target;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Animator animator;

    private float elapsedTime = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Setup(Entity owner, Entity target, float damage)
    {
        this.owner = owner;
        this.target = target;
        this.damage = damage;
        this.targetPosition = target.transform.position;
        this.startPosition = owner.transform.position;

        // 로컬 x축이 Target을 향하도록
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x) * Mathf.Rad2Deg);
    }

    public void Update()
    {
        if (target == null)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
        }

        transform.position += (targetPosition - startPosition).normalized * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.Equals(target.gameObject))
        {
            Debug.Log("Hit");
            animator.SetTrigger("Hit");
            target.TakeDamage(owner, this, damage);
            Destroy(gameObject, 2f);
        }
    }
}
