using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour {

    [Header("Text")]
    [SerializeField] TextMeshProUGUI strText;
    [SerializeField] TextMeshProUGUI dexText;
    [SerializeField] TextMeshProUGUI vitText;
    [SerializeField] TextMeshProUGUI defText;
    [SerializeField] TextMeshProUGUI intText;
    [SerializeField] TextMeshProUGUI wisText;

    [Header("Buttons")]
    [SerializeField] Button strBtn;
    [SerializeField] Button dexBtn;
    [SerializeField] Button vitBtn;
    [SerializeField] Button defBtn;
    [SerializeField] Button intBtn;
    [SerializeField] Button wisBtn;

    private Character info;
    public Character Info {
        get { return info; }
        set {
            Init(value);
        }
    } // Current Character Info

    private bool isShortened = false;
    public bool IsShortened {
        get { return isShortened; }
        set {
            isShortened = value;
            SetStatText();
        }
    } // Shortens the Text

    public void Init(Character c) {
        if(info != null) {
            info.StrChange -= SetStrText;
            info.DexChange -= SetDexText;
            info.VitChange -= SetVitText;
            info.DefChange -= SetDefText;
            info.IntChange -= SetIntText;
            info.WisChange -= SetWisText;
            info.LevelUpsChange -= SetLevelUp;
        }

        info = c;

        info.StrChange += SetStrText;
        info.DexChange += SetDexText;
        info.VitChange += SetVitText;
        info.DefChange += SetDefText;
        info.IntChange += SetIntText;
        info.WisChange += SetWisText;
        info.LevelUpsChange += SetLevelUp;

        SetStatText();
        SetLevelUp(info.GetLevelUps());
    }

    public void SetStatText() {
        SetStrText(info.GetStrength());
        SetDexText(info.GetDexterity());
        SetVitText(info.GetVitality());
        SetDefText(info.GetDefense());
        SetWisText(info.GetWisdom());
        SetIntText(info.GetIntelligence());
    }
    public void SetLevelUp(int levelUps) {
        bool val = levelUps > 0;
        strBtn.gameObject.SetActive(val);
        dexBtn.gameObject.SetActive( val);
        vitBtn.gameObject.SetActive( val);
        defBtn.gameObject.SetActive( val);
        intBtn.gameObject.SetActive( val);
        wisBtn.gameObject.SetActive( val);
    }

    public void SetStrText(int str) {
        string text;
        
        if (isShortened) {
            text = "Str: " + str;
        } else {
            text = "Strength: " + str;
        }
        strText.text = text;

        if (text.Length > 13) {
            IsShortened = true;
        }
    }
    public void SetDexText(int str) {
        string text;

        if (isShortened) {
            text = "Dex: " + str;
        } else {
            text = "Dexterity: " + str;
        }
        dexText.text = text;

        if (text.Length > 13) {
            IsShortened = true;
        }
    }
    public void SetVitText(int str) {
        string text;

        if (isShortened) {
            text = "Vit: " + str;
        } else {
            text = "Vitality: " + str;
        }
        vitText.text = text;

        if (text.Length > 13) {
            IsShortened = true;
        }
    }
    public void SetDefText(int str) {
        string text;

        if (isShortened) {
            text = "Def: " + str;
        } else {
            text = "Defense: " + str;
        }
        defText.text = text;

        if (text.Length > 13) {
            IsShortened = true;
        }
    }
    public void SetIntText(int str) {
        string text;

        if (isShortened) {
            text = "Int: " + str;
        } else {
            text = "Intelligence: " + str;
        }
        intText.text = text;

        if (text.Length > 15) {
            IsShortened = true;
        }
    }
    public void SetWisText(int str) {
        string text;

        if (isShortened) {
            text = "Wis: " + str;
        } else {
            text = "Wisdom: " + str;
        }
        wisText.text = text;

        if (text.Length > 13) {
            IsShortened = true;
        }
    }

    public void LevelUpStr() {
        info.LevelUpStr();
    }
    public void LevelUpDex() {
        info.LevelUpDex();
    }
    public void LevelUpVit() {
        info.LevelUpVit();
    }
    public void LevelUpDef() {
        info.LevelUpDef();
    }
    public void LevelUpInt() {
        info.LevelUpInt();
    }
    public void LevelUpWis() {
        info.LevelUpWis();
    }

}
