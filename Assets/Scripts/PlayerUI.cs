using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField] Image icon;
    [SerializeField] Image healthBar;
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI dpsText;
    [SerializeField] StatUI statInfo;
    private RectTransform parent;
    private Button m_button;

    private Character info;
    public Character Info {
        get { return info; }
        set {
            Init(value);
        }
    }

    private bool hasExpanded;
    private static Vector3 SHOW_POS = new Vector3(.0f, 200.0f, .0f);
    private static Vector3 HIDE_POS = new Vector3(.0f, .0f, .0f);

    private void Start() {
        parent = transform.parent.GetComponent<RectTransform>();
        m_button = GetComponent<Button>();

        statInfo.gameObject.SetActive(false);
    }

    private void Init(Character c) {
        if(info != null) {
            info.HealthChange -= SetHealthRatio;
            info.ExpChange -= SetExpRatio;
            info.LevelChange -= SetLevel;
            info.LevelUpsChange -= SetLevelUps;

            SetHealthRatio(c.GetCurrHP() / (float)c.GetMaxHP());
            SetExpRatio(c.GetExp() / (float)c.GetLevelExp());
            SetLevel(c.GetLevel());
            SetDPS(0);
        } else {
            SetHealthRatio(1.0f);
            SetExpRatio(.0f);
            levelText.text = "Level: 1 (0)";
            SetDPS(0);
        }
        info = c;

        SpriteRenderer renderer = info.GetComponent<SpriteRenderer>(); // How do We get Default Sprite? Animator?
        SetImage(renderer.sprite);

        info.HealthChange += SetHealthRatio;
        info.ExpChange += SetExpRatio;
        info.LevelChange += SetLevel;
        info.LevelUpsChange += SetLevelUps;
        //info.DPS

        statInfo.Info = c;
    }
    

    public void SetImage(Sprite i) {
        icon.sprite = i;
    }
    public void SetHealthRatio(float val) {
        var scale = healthBar.rectTransform.localScale;
        scale.x = val;
        healthBar.rectTransform.localScale = scale;
    }
    public void SetExpRatio(float val) {
        var scale = expBar.rectTransform.localScale;
        scale.x = val;
        expBar.rectTransform.localScale = scale;
    }


    public void SetLevel(int level) {
        levelText.text = "Level: " + level + "(" + info.GetLevelUps() + ")";
    }
    public void SetLevelUps(int levelUps) {
        levelText.text = "Level: " + info.GetLevel() + "(" + levelUps + ")";
    }
    public void SetDPS(int DPS) {
        dpsText.text = "DPS: " + DPS;
    }

    public void Expand() {
        if (hasExpanded) {
            StartCoroutine(HideStats());
        } else {
            StartCoroutine(ShowStats());
        }
    }

    private IEnumerator ShowStats() {
        m_button.enabled = false;
        statInfo.gameObject.SetActive(true);

        float timeToFade = .5f;
        float currTime = .0f;
        while (currTime < timeToFade) {
            currTime += Time.deltaTime;

            float t = currTime / timeToFade;

            parent.anchoredPosition = Vector3.Lerp(HIDE_POS, SHOW_POS, t);
            yield return null;
        }

        parent.anchoredPosition = Vector3.Lerp(HIDE_POS, SHOW_POS, 1.0f);
        hasExpanded = true;
        m_button.enabled = true;
    }

    private IEnumerator HideStats() {
        m_button.enabled = false;

        float timeToFade = .5f;
        float currTime = .0f;
        while (currTime < timeToFade) {
            currTime += Time.deltaTime;

            float t = currTime / timeToFade;

            parent.anchoredPosition = Vector3.Lerp(SHOW_POS, HIDE_POS, t);
            yield return null;
        }

        parent.anchoredPosition = Vector3.Lerp(SHOW_POS, HIDE_POS, 1.0f);
        hasExpanded = false;
        m_button.enabled = true;

        statInfo.gameObject.SetActive(false);
    }

}
