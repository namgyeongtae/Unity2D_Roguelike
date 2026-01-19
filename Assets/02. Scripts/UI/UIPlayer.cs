using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayer : CanvasPanel
{
    [SerializeField] private Image playerHpBar;
    [SerializeField] private TextMeshProUGUI playerHpText;

    private PlayerController playerController;

    public override void Initialize()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();
        playerController.Entity.onHpChanged += SetPlayerHp;

        playerHpText.text = $"{playerController.Entity.Stats.HPStat.Value} / {playerController.Entity.Stats.MaxHpStat.Value}"; 
    }

    public override void Release()
    {
        playerController.Entity.onHpChanged -= SetPlayerHp;
    }

    public void SetPlayerHp(Entity entity, float curHp, float prevHp)
    {
        if (!entity.Stats.IsDamageable)
        {
            Debug.LogError("Player is not damageable");
            return;
        }

        float maxHp = entity.Stats.MaxHpStat.Value;

        playerHpBar.fillAmount = curHp / maxHp;
        playerHpText.text = $"{curHp} / {maxHp}";
    }
}
