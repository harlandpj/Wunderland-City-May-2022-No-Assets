using UnityEngine;

// Scriptable Object data container which defines all the COMMON data for both Player and NPC characters

[CreateAssetMenu(fileName ="New Character", menuName = "Character Creation/Character Stats", order =51)]
public class CharStats : ScriptableObject
{
    [SerializeField]
    private string charName; // character name
    [SerializeField]
    private int attackDamage; // damage dealt when attacking others
    [SerializeField]
    private int defenseResistance; // resistance to damage from attacks (maybe not used till later on in dev)
    [SerializeField]
    private float speed; // speed of movement
    [SerializeField]
    private float eyesightDistance; // distance character can see (not applicable to Player)
    [SerializeField]
    private int maxHealth; // maximum health points
    [SerializeField]
    private Sprite icon; // icon for display (maybe later on)

    // obvious member accessors
    public string CharName
    {
        get
        {
            return charName;
        }
    }

    public int AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

    public int DefenseResistance
    {
        get
        {
            return defenseResistance;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }
    }
    
    public float EyesightDistance
    {
        get
        {
            return eyesightDistance;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }


    public void PrintMessage()
    {
        Debug.Log("The " + charName + " character has been loaded.");
    }
}
