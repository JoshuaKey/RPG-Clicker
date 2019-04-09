using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] Transform EntityParent;

    [Header("Player")]
    [SerializeField]
    Vector3 PlayerPosition;
    [SerializeField] Character Player;
    [SerializeField] Character[] Allies;

    [Header("Enemy")]
    [SerializeField]
    Vector3 EnemyPosition;
    [SerializeField] Character Enemy;
    [SerializeField] Character[] EnemyPrefabs;

    [Header("UI")]
    [SerializeField]
    PlayerUI characterInfo;
    [SerializeField] EnemyUI enemyInfo;

    [SerializeField] private int masteryPoints;
    public int MasteryPoints { get { return masteryPoints; } }

    [SerializeField] private int round = 1;
    private int enemiesDefeated = 1;

    public delegate void EnemyDefeatHandler();
    public EnemyDefeatHandler OnEnemyDefeat;

    public static Game Instance;

    private bool isPlayerDead;
    private bool isEnemyDead;
    private WaitForSeconds DeathDelay = new WaitForSeconds(2.0f);

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        for (int i = 0; i < EnemyPrefabs.Length; i++) {
            EnemyPrefabs[i] = Instantiate(EnemyPrefabs[i]);
            EnemyPrefabs[i].transform.position = Vector3.up * 500.0f;
            EnemyPrefabs[i].transform.parent = EntityParent;
        }
        Player = Instantiate(Player);
        Player.transform.position = PlayerPosition;
        Player.transform.parent = EntityParent;

        int rand = Random.Range(0, EnemyPrefabs.Length); // New Enemy
        Enemy = EnemyPrefabs[rand];
        Enemy.transform.position = EnemyPosition;
        Enemy.gameObject.SetActive(true);

        characterInfo.Info = Player;
        enemyInfo.Info = Enemy;
        if (OnEnemyDefeat != null) {
            OnEnemyDefeat();
        }
    }

    void Update() {
        if (isPlayerDead || isEnemyDead) { return; }

        // Check for Player Input
        CheckPlayerInput();

        // Check for Enemy Input - Attack
        CheckEnemyInput();

        // Check for Ally Input - Attack
        CheckAllyInput();
    }

    private void CheckPlayerInput() {
        // Check if Alive
        if (!Player.IsAlive()) {
            OnPlayerDeath();
            return;
        }

        // Check If Attack
        if (Input.GetMouseButtonDown(0)) {

            // Special Attack First
            if (Player.HasSpecialAttack()) { // Check Attack
                int damage = Player.UseSpecialAttack(); // Use Attack

                Enemy.TakeDamage(damage); // Attack Enemy

                //print("Player is doing " + damage + " damage");
                //print("Enemy has " + Enemy.GetCurrHP() + " out of " + Enemy.GetMaxHP());
            }
            // Normal Attack
            else
            if (Player.HasNormalAttack()) { // Check Attack
                int damage = Player.UseNormalAttack(); // Use Attack

                Enemy.TakeDamage(damage); // Attack Enemy

                //print("Player is doing " + damage + " damage");
                //print("Enemy has " + Enemy.GetCurrHP() + " out of " + Enemy.GetMaxHP());
            }
        }
    }

    private void CheckEnemyInput() {
        // Check if Alive
        if (!Enemy.IsAlive()) {
            OnEnemyDeath();
            return;
        }


        // Special Attack First
        if (Enemy.HasSpecialAttack()) { // Check Attack
            int damage = Enemy.UseSpecialAttack(); // Use Attack


            Character target = null;    // Determine Target
            if (Allies.Length == 0) {   // No Allies
                target = Player;
            } else {                    // Try to attack Ally
                int randAlly = Random.Range(0, Allies.Length);
                int origin = randAlly;
                while (target != null) {
                    if (Allies[randAlly].IsAlive()) {
                        target = Allies[randAlly];
                    }
                    if (randAlly++ == origin) {
                        target = Player;
                    }
                }
            }

            target.TakeDamage(damage); // Take Damage

            //print("Enemy is doing " + damage + " damage");
            //print(target.name + " has " + target.GetCurrHP() + " out of " + target.GetMaxHP());
        }

        // Normal Attack
        else
        if (Enemy.HasNormalAttack()) {
            int damage = Enemy.UseNormalAttack(); // Use Attack

            Character target = null;    // Determine Target
            if (Allies.Length == 0) {   // No Allies
                target = Player;
            } else {                    // Try to attack Ally
                int randAlly = Random.Range(0, Allies.Length);
                int origin = randAlly;
                while (target != null) {
                    if (Allies[randAlly].IsAlive()) {
                        target = Allies[randAlly];
                    }
                    if (randAlly++ == origin) {
                        target = Player;
                    }
                }
            }

            target.TakeDamage(damage); // Take Damage

            //print("Enemy is doing " + damage + " damage");
            //print(target.name + " has " + target.GetCurrHP() + " out of " + target.GetMaxHP());
        }
    }

    private void CheckAllyInput() {
        // Check if Alive
        if (!Player.IsAlive()) {
            return;
        }

        foreach (Character ally in Allies) {
            if (!ally.IsAlive()) { // Check if Alive
                OnAllyDeath(ally);
                continue;
            }

            //// Special Attack First
            //if (ally.HasSpecialAttack()) { // Check Attack
            //    int damage = ally.UseSpecialAttack(); // Use Attack

            //    Enemy.TakeDamage(damage); // Attack Enemy

            //    print(ally.name + " is doing " + damage + " damage");
            //    print("Enemy has " + Enemy.GetCurrHP() + " out of " + Enemy.GetMaxHP());
            //} else 
            // Normal Attack
            if (ally.HasNormalAttack()) { // Check Attack
                int damage = ally.UseSpecialAttack(); // Use Attack

                Enemy.TakeDamage(damage); // Attack Enemy

                print(ally.name + " is doing " + damage + " damage");
                print("Enemy has " + Enemy.GetCurrHP() + " out of " + Enemy.GetMaxHP());
            }
        }
    }


    public IEnumerator GenerateEnemy() {
        yield return DeathDelay;
        Enemy.ResetCharacter(); // Reset
        Enemy.gameObject.SetActive(false);

        if (enemiesDefeated++ >= 10) {
            enemiesDefeated = 1;
            round++;
        }
        if (OnEnemyDefeat != null) {
            OnEnemyDefeat();
        }

        int i = Random.Range(0, EnemyPrefabs.Length); // New Enemy
        Enemy = EnemyPrefabs[i];
        Enemy.transform.position = EnemyPosition;
        Enemy.gameObject.SetActive(true);
        Enemy.AutoLevelUp(round * (enemiesDefeated == 10 ? 2 : 1));
        isEnemyDead = false;

        enemyInfo.Info = Enemy;
    }

    public IEnumerator GeneratePlayer() {
        yield return DeathDelay;

        Player.ResetCharacter(); // Reset
        isPlayerDead = false;

        // Set Round???
    }

    public IEnumerator GenerateAlly(Character ally) {
        yield return DeathDelay;

        ally.ResetCharacter(); // Reset
    }

    private void OnEnemyDeath() {
        print("Enemy is Dead");

        Enemy.Die();
        isEnemyDead = true;

        // Gain EXP
        int monsterExp = Enemy.GetExpWorth();
        Player.GainExp(monsterExp);

        foreach (var ally in Allies) {
            ally.GainExp(monsterExp);
        }

        StartCoroutine(GenerateEnemy()); // New Enemy
    }

    private void OnPlayerDeath() {
        print("Player is Dead");

        Player.Die();
        isPlayerDead = true;

        StartCoroutine(GeneratePlayer()); // New 'Player' aka round
    }

    private void OnAllyDeath(Character ally) {
        print(ally.name + " is Dead");

        ally.Die();

        // Gain EXP
        int allyExp = ally.GetExpWorth();
        Enemy.GainExp(allyExp);
    }

    public void UseMasteryPointOnStr(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpStrMastery();
        }
    }
    public void UseMasteryPointOnDef(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpDefMastery();
        }
    }
    public void UseMasteryPointOnVit(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpVitMastery();
        }
    }
    public void UseMasteryPointOnDex(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpDexMastery();
        }
    }
    public void UseMasteryPointOnInt(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpIntMastery();
        }
    }
    public void UseMasteryPointOnWis(Character c) {
        if (masteryPoints > 0) {
            masteryPoints--;
            c.LevelUpWisMastery();
        }
    }

    public int GetRound() {
        return round;
    }
    public int GetEnemiesDefeated() {
        return enemiesDefeated;
    }
}
// Damage correlate to Animaiton Events
// Round
// Training UI and Implementation?

// Add Ally, and Training UI
// Better Background / Display / World/ Positioning
// Pop up Display
// Add Specialties. (Animation Curve)?
// Add Round
// Add Enemy Count
// Add Level Up Selection
// Add allies

// Tap Titan expresses the Enemy as a huge 'Background'
// The Player is pretty small in front of it
// Allies are placed on areas to the side. these are added as necessary for allies.

//Cleicker Heros, Allies are simply bought,  not dislayed.


//Typical clickers are played Portait
// Two halvees. Top is the game. Bttom is UI and Selection
// Player, Allies, Train... Sections
// Player will showcase the Player, Hp, Exp, Level, Stats, etc/
// Ally will show the allies and then each ally like the Player
// Train will show a mini scene of a mount and have a button. Also shows the results.
//      Should also containscreen for using Mastery Points

// Font
// 2 Bars (Exp, Hp)
// Icon for Stats
// Sword
// Hand
// Plus
// Sheild
// Brain
// Electric Power

// Icons for Entities
// General Layout
// Text
// Button
// DropDown
// Hide


//You can click on it to hide/ show it - Widen the Screen
// Allies will show all allies First
// Bars should Shimmer if there is something new
// A Character should have - Icon on Left Level and Levelups below, Hp and Exp (labeled) on Right
//  Below it should should show in order, Stats and 