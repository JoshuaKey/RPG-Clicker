using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {

    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI roundText;

    private Character info;
    public Character Info {
        get { return info; }
        set {
            Init(value);
        }
    }

    private void Start() {
        Game.Instance.OnEnemyDefeat += SetRoundText;
    }


    public void Init(Character c) {
        if(info != null) {
            info.HealthChange -= SetHealthRatio;

            SetHealthRatio(c.GetCurrHP() / (float)c.GetMaxHP());
        } else {
            SetHealthRatio(1.0f);
        }

        info = c;

        info.HealthChange += SetHealthRatio;
    }


    public void SetHealthRatio(float val) {
        var scale = healthBar.rectTransform.localScale;
        scale.x = val;
        healthBar.rectTransform.localScale = scale;
    }

    public void SetRoundText() {
        roundText.text = "Round: " + Game.Instance.GetRound() + " (" + Game.Instance.GetEnemiesDefeated() + "/10)";
    }
}
