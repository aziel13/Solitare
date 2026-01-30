using UnityEngine;

[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "Scriptable Objects/CardScriptableObject")]
public class CardScriptableObject : ScriptableObject
{
    [SerializeField] private CardIdentity.CardValue value;

    public CardIdentity.CardValue Value => value;

    public CardIdentity.CardSuite Suite => suite;

    public Sprite FaceSprite1 => faceSprite;

    public GameObject O => cardGameObject;

    [SerializeField] private CardIdentity.CardSuite suite;

    public override string ToString()
    {
        return $"{value} of {suite}";
    }
    
    public Sprite FaceSprite
    {
        get => faceSprite;
        set => faceSprite = value;
    }

    public GameObject CardGameObject
    {
        get => cardGameObject;
        set => cardGameObject = value;
    }

    [SerializeField] private Sprite faceSprite;
    
    [SerializeField]
    private GameObject cardGameObject;


    public int numericValue()
    {

        switch (value) 
        {
            case CardIdentity.CardValue.ace:
                return 1;
            case CardIdentity.CardValue.two:
                return 2;
            case CardIdentity.CardValue.three:
                return 3;
            case CardIdentity.CardValue.four:
                return 4;
            case CardIdentity.CardValue.five:
                return 5;
            case CardIdentity.CardValue.six:
                return 6;
            case CardIdentity.CardValue.seven:
                return 7;
            case CardIdentity.CardValue.eight:
                return 8;
            case CardIdentity.CardValue.nine:
                return 9;
            case CardIdentity.CardValue.ten:
                return 10;
            case CardIdentity.CardValue.jack:
                return 11;
            case CardIdentity.CardValue.queen:
                return 12;
            case CardIdentity.CardValue.king:
                return 13;
            default:
                return -1;
        }

    } 
    
    public CardIdentity.CardColour getSuiteColor()
    {

        if (suite == CardIdentity.CardSuite.Clubs || suite == CardIdentity.CardSuite.Spades)
        {
            return CardIdentity.CardColour.black;
        }

        return CardIdentity.CardColour.red;
    }


}
