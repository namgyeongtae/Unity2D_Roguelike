using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    private int swingDir = 1;

    private Coroutine moveOwnerCoroutine;

    public override void Setup()
    {
        base.Setup();

        onAttack += UpdateSwordRotation;
        onAttack += MoveOwner;
    }

    public override void Attack(Vector2 direction)
    {
        StartCoroutine(SwingSword(direction));

        base.Attack(direction);
    }

    private IEnumerator SwingSword(Vector2 direction)
    {
        direction = direction.normalized;

        float swingHalfAngle = 60f;
        float duration = 0.1f;

        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float startAngle  = baseAngle + swingHalfAngle * swingDir; // CCW
        float targetAngle = baseAngle - swingHalfAngle * swingDir; // CW

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 여기만 바꾸면 속도 감이 완전히 달라짐
            float eased = MathUtils.EaseInExpo(t, 6f);     // 혹은 EaseInPower(t, 4f)

            float ang = Mathf.LerpAngle(startAngle, targetAngle, eased);
            Vector2 swordLookDir = MathUtils.AngleToDir(ang);

            owner.SocketPivot.transform.up = MathUtils.Rotate(swordLookDir * -1f, +45f);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        owner.GetComponent<PlayerController>().AttackEnd(direction);
    }

    private void UpdateSwordRotation(Vector2 direction)
    {
        if (swingDir == 1) animator.SetTrigger("SlashLeft");
        else animator.SetTrigger("SlashRight");

        swingDir *= -1;
    }

    private void MoveOwner(Vector2 direction)
    {
        StopMoveOwner();

        moveOwnerCoroutine = StartCoroutine(AddForceOwner(direction));
    }

    private IEnumerator AddForceOwner(Vector2 direction)
    {
        var rb = owner.GetComponent<Rigidbody2D>();
        direction = direction.normalized;

        // 설정값(튜닝 포인트)
        float distance = 0.15f;
        float duration = 0.25f;

        // 거리/시간으로 초기 속도 계산 (대략적으로 맞추기 쉬움)
        float v0 = (2f * distance) / duration; // 감속 이동(삼각형 속도 프로필) 근사
        Vector2 startVel = direction * v0;

        float originalDrag = rb.linearDamping;

        // 빙판 느낌: drag는 낮게 시작 → 점점 크게 (마찰이 점점 잡는 느낌)
        float dragStart = 0.5f;
        float dragEnd   = 8f;

        rb.linearVelocity = startVel;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // 점점 더 감속되게(= 마찰이 점점 세지게) 만들려면 drag 자체를 증가시키기
            float dragT = Mathf.Pow(t, 2.5f); // 후반으로 갈수록 증가폭 커짐
            rb.linearDamping = Mathf.Lerp(dragStart, dragEnd, dragT);

            yield return null;
        }

        // 마지막 스냅(잔속 제거)
        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = originalDrag;
    }

    private void StopMoveOwner()
    {
        if (moveOwnerCoroutine != null)
        {
            StopCoroutine(moveOwnerCoroutine);
            moveOwnerCoroutine = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Entity enemy = other.gameObject.GetComponent<Entity>();
            enemy.TakeDamage(owner, this, owner.Stats.GetStat(damageData).Value);
        }
        
        StopMoveOwner();

        moveOwnerCoroutine = StartCoroutine(AddForceOwner(owner.transform.position - other.transform.position));
    }
}
