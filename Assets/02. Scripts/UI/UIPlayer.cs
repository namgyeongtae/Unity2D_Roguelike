using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : CanvasPanel
{
    [SerializeField] private Image playerHpBar;

    public override void Initialize()
    {
        
    }

    public void SetPlayerHp(float hp, float maxHp)
    {
        playerHpBar.fillAmount = hp / maxHp;
    }
}
