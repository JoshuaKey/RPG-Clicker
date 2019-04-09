using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private int Str = 1;
    [SerializeField] private int Dex = 1;
    [SerializeField] private int Vit = 1;
    [SerializeField] private int Def = 1;
    [SerializeField] private int Int = 1;
    [SerializeField] private int Wis = 1;

    [Header("Curves")]
    [SerializeField] AnimationCurve strCurve;
    [SerializeField] AnimationCurve dexCurve;
    [SerializeField] AnimationCurve vitCurve;
    [SerializeField] AnimationCurve defCurve;
    [SerializeField] AnimationCurve intCurve;
    [SerializeField] AnimationCurve wisCurve;

    private int StrMastery = 0;
    private int DexMastery = 0;
    private int VitMastery = 0;
    private int DefMastery = 0;
    private int IntMastery = 0;
    private int WisMastery = 0;

    private int maxHP; // Vit
    private int currHP;

    private int normalDmg; // Str
    private int dmgReduction; // Def
    private float normalAttackSpeed; // Dex
    private float nextNormalAttack;

    private int specialDmg; // Wis
    private float specialAttackSpeed; // Int
    private float nextSpecialAttack;

    private int expWorth = 1;
    private int currExp = 0;
    private int levelExp = 10;
    private int level = 1;
    private int levelUps = 0;

    private Animator m_anim;

    private void Start() {
        m_anim = GetComponent<Animator>();
        m_anim.SetFloat("AttackSpeed", 1.0f);

        //print(name + " has " + Str + " Strength");
        //print(name + " has " + Dex + " Dexterity");
        //print(name + " has " + Vit + " Vitality");
        //print(name + " has " + Def + " Defense");
        //print(name + " has " + Int + " Intelligence");
        //print(name + " has " + Wis + " Wisdom");
        Init();
    }

    public delegate void StatChangeHandler(int val);
    public delegate void StatFChangeHandler(float val);
    public StatChangeHandler StrChange;
    public StatChangeHandler DexChange;
    public StatChangeHandler VitChange;
    public StatChangeHandler DefChange;
    public StatChangeHandler IntChange;
    public StatChangeHandler WisChange;
    public StatChangeHandler LevelChange;
    public StatChangeHandler LevelUpsChange;
    public StatFChangeHandler HealthChange;
    public StatFChangeHandler ExpChange;


    /// Combat -----------------------------------------------------------------------------------------
    public int UseNormalAttack() {
        if (HasNormalAttack()) {
            SetNextNormalAttack();

            m_anim.SetTrigger("Attack"); // Animation

            return normalDmg; // Damage
        }
        return 0;
    }
    public void SetNextNormalAttack() {
        nextNormalAttack = Time.time + normalAttackSpeed; 
    }

    public int UseSpecialAttack() {
        if (HasSpecialAttack()) {
            SetNextSpecialAttack();

            m_anim.SetTrigger("Attack"); // Animation

            return specialDmg; // Damage
        }
        return 0;
    }
    public void SetNextSpecialAttack() {
        nextSpecialAttack = Time.time + specialAttackSpeed;
    }

    public void TakeDamage(int amo, bool normalAttack = true) {
        if(amo > 0) {
            if (normalAttack) {
                amo = (amo - dmgReduction);
            }
           
            if(amo < 1) { amo = 1; } // Deal at Least 1 damage

            currHP -= amo; 
            if (currHP < 0) { currHP = 0; } // Cant go below 0 HP 

            if(HealthChange != null) {
                HealthChange(GetCurrHP() / (float)GetMaxHP());
            }
        }
    }
    public void Die() {
        m_anim.SetTrigger("Death");
    }
    public void Heal(int amo) {
        if(amo < 0) {
            currHP += amo;
            if(currHP > maxHP) { currHP = maxHP; }

            if (HealthChange != null) {
                HealthChange(GetCurrHP() / (float)GetMaxHP());
            }
        }
    }
    public void FullHeal() {
        currHP = maxHP;

        if (HealthChange != null) {
            HealthChange(GetCurrHP() / (float)GetMaxHP());
        }
    }

    public void ResetCharacter() { // Pass in Rounds?

        m_anim.SetTrigger("Reset");
        Init();
    }

    public void Init() {
        GenerateStats();
        FullHeal();
    }

    public bool HasNormalAttack() {
        return nextNormalAttack <= Time.time;
    }
    public bool HasSpecialAttack() {
        return nextSpecialAttack <= Time.time;
    }


    /// Levels -----------------------------------------------------------------
    public bool GainExp(int exp) {
        bool rtnVal = false;
        currExp += exp;

        if (currExp > levelExp) {
            level++;
            levelUps++;
            currExp = currExp - levelExp;
            float bonusMulti = level / 10.0f;
            levelExp = (int)((level + bonusMulti) * 10);

            if(LevelChange != null) {
                LevelChange(level);
            }
            if(LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
            //print(name + " has Leveled Up!");

            FullHeal();
            rtnVal = true;
        }

        if (ExpChange != null) {
            ExpChange(currExp / (float)levelExp);
        }
        //print(name + " has gained " + exp + ". They have " + currExp + " out of " + levelExp);

        return rtnVal;
    }

    private void GenerateStats() {
        expWorth = level + Str + Dex + Vit + Def + Int + Wis;

        maxHP = Mathf.RoundToInt(vitCurve.Evaluate(Vit * (VitMastery + 1)));
        currHP = maxHP;

        normalDmg = Mathf.RoundToInt(strCurve.Evaluate(Str * (StrMastery + 1)));
        dmgReduction = Mathf.RoundToInt(defCurve.Evaluate(Def * (DefMastery + 1)));
        float attackSpeed = dexCurve.Evaluate(Dex * (DexMastery + 1));
        if(attackSpeed >= 3.0f) {
            normalAttackSpeed = .333f;
            m_anim.SetFloat("AttackSpeed", 3.0f);
            normalDmg *= (int)(attackSpeed / 3.0f);
        } else {
            normalAttackSpeed = 1 / attackSpeed;
            m_anim.SetFloat("AttackSpeed", attackSpeed < 1.0f ? 1.0f : attackSpeed);
        }

        specialDmg = Mathf.RoundToInt(wisCurve.Evaluate(Wis * (WisMastery + 1)));
        attackSpeed = intCurve.Evaluate(Int * (IntMastery + 1));
        if (attackSpeed >= .333f) {
            specialAttackSpeed = 3.0f;
            specialDmg *=  (int)(attackSpeed / .333f);
        } else {
            specialAttackSpeed = 1 / attackSpeed;
        } 

        SetNextNormalAttack();
        SetNextSpecialAttack();

        //print("Stats: \nStr: " + Str + "\nDex: " + Dex + "\nVit: " + Vit + "\nDef: " + Def + "\nInt: " + Int + "\nWis: " + Wis
        //    + "\n\nNormal Dmg: " + normalDmg + "\nNormalAttackSpeed: " + normalAttackSpeed + "\nDamageReducetion: " + dmgReduction
        //    + "\n\nSpecial Dmg: " + specialDmg + "\nSpecialAttackSpeed: " + specialAttackSpeed);
    }
    public void AutoLevelUp(int val) {
        Str = 1;
        Dex = 1;
        Vit = 1;
        Def = 1;
        Int = 1;
        Wis = 1;

        level = val;
        levelUps = val - 1;
        while(levelUps > 0) {
            int randStat = Random.Range(0, 6);
            switch (randStat) {
                case 0:
                    Str++;
                    break;
                case 1:
                    Dex++;
                    break;
                case 2:
                    Vit++;
                    break;
                case 3:
                    Def++;
                    break;
                case 4:
                    Int++;
                    break;
                case 5:
                    Wis++;
                    break;
            }
            levelUps--;
        }
        GenerateStats();
    }


    public void LevelUpStr() {
        if(levelUps > 0) {
            levelUps--;
            Str++;
            GenerateStats();

            if(StrChange != null) {
                StrChange(Str);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }
    public void LevelUpDex() {
        if (levelUps > 0) {
            levelUps--;
            Dex++;
            GenerateStats();

            if (DexChange != null) {
                DexChange(Dex);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }
    public void LevelUpVit() {
        if (levelUps > 0) {
            levelUps--;
            Vit++;
            GenerateStats();

            if (VitChange != null) {
                VitChange(Vit);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }
    public void LevelUpDef() {
        if (levelUps > 0) {
            levelUps--;
            Def++;
            GenerateStats();

            if (DefChange != null) {
                DefChange(Def);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }
    public void LevelUpInt() {
        if (levelUps > 0) {
            levelUps--;
            Int++;
            GenerateStats();

            if (IntChange != null) {
                IntChange(Int);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }
    public void LevelUpWis() {
        if (levelUps > 0) {
            levelUps--;
            Wis++;
            GenerateStats();

            if (WisChange != null) {
                WisChange(Wis);
            }
            if (LevelUpsChange != null) {
                LevelUpsChange(levelUps);
            }
        }
    }

    public void LevelUpStrMastery() {
        StrMastery++;
        GenerateStats();
    }
    public void LevelUpDexMastery() {
        DexMastery++;
        GenerateStats();
    }
    public void LevelUpVitMastery() {
        VitMastery++;
        GenerateStats();
    }
    public void LevelUpDefMastery() {
        DefMastery++;
        GenerateStats();
    }
    public void LevelUpIntMastery() {
        IntMastery++;
        GenerateStats();
    }
    public void LevelUpWisMastery() {
        WisMastery++;
        GenerateStats();
    }

    /// Getters--------------------------------------------------------------------------------------------
    public bool IsAlive() {
        return currHP > 0;
    }

    public int GetLevel() {
        return level;
    }
    public int GetLevelUps() {
        return levelUps;
    }
    public int GetExp() {
        return currExp;
    }
    public int GetLevelExp() {
        return levelExp;
    }
    public int GetExpWorth() {
        return expWorth;
    }

    public int GetCurrHP() {
        return currHP;
    }
    public int GetMaxHP() {
        return maxHP;
    }

    public int GetStrength() {
        return Str;
    }
    public int GetDexterity() {
        return Dex;
    }
    public int GetVitality() {
        return Vit;
    }
    public int GetDefense() {
        return Def;
    }
    public int GetIntelligence() {
        return Int;
    }
    public int GetWisdom() {
        return Wis;
    }
}
