using System;
using UnityEngine;
using UnityEngine.Serialization;


public class Foundation : MonoBehaviour
{
    public enum FoundationColumns
    {
        ColumnOne,
        ColumnTwo,
        ColumnThree,
        ColumnFour,
        
    }
    
    
    
    [SerializeField] private FoundationColumns foundationColumns;
    public override string ToString()
    {
        return $"Foundation {foundationColumns}";
    }
    
    public FoundationColumns GetFoundationPileValue()
    {
        return foundationColumns;
    }
    
    private void Start()
    {

        GameInput.Instance.OnFoundationClicked  += Foundation_OnFoundationClicked;

    }
 
    private void Foundation_OnFoundationClicked(object sender, GameInput.OnFoundationClickedEventArgs e)
    {
        
        if (e.currentGameObject == gameObject)
        {
            if (canBeStacked(e.previousObject))
            {
                if (e.previousObject.TryGetComponent(out Card card) && (card.IsTopCard() || GameManager.Instance.IsInTripOnDisplay(card.CardScriptableObject)))
                {
                    
                    card.MoveCard(e.currentGameObject,e.previousObject);
                    
                }
            }
            else
            {
                if (e.previousObject.TryGetComponent(out Card card))
                {

                    e.previousObject.GetComponent<UpdateSprite>().SetCardSelected(false);

                }

                GameInput.Instance.PreviouslySelected = gameObject;
            }
        }
    }
    
    private bool canBeStacked(GameObject previousObject)
    {
        if (previousObject.TryGetComponent(out Card card))
        {
            if (GameManager.Instance.Foundations[GetColumnNumberListIndexFormat()].Count == 0 && card.CardScriptableObject.Value == CardIdentity.CardValue.ace)
            {
                Debug.Log("Stackable");
                return true;
            }
        }

        Debug.Log("Not Stackable");
        return false;
    }

    public int GetColumnNumberListIndexFormat()
    {
        int columnNumber;
        
        switch (foundationColumns)
        {
            default:
            case Foundation.FoundationColumns.ColumnOne:
                columnNumber = 0;
                break;
            case Foundation.FoundationColumns.ColumnTwo:
                columnNumber = 1;
                break;
            case Foundation.FoundationColumns.ColumnThree:
                columnNumber = 2;
                break;
            case Foundation.FoundationColumns.ColumnFour:
                columnNumber = 3;
                break;
        }
        
        return columnNumber;

    }
    
    
}

        


