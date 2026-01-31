
using UnityEngine;


public class Tableau : MonoBehaviour
{
    public enum TableauColumn
    {
        ColumnOne,
        ColumnTwo,
        ColumnThree,
        ColumnFour,
        ColumnFive,
        ColumnSix,
        ColumnSeven,
        
    }
    
    [SerializeField] private TableauColumn tableauColumn;
    
    public override string ToString()
    {
        return $"Tableau {tableauColumn}";
    }
    
    public TableauColumn GetTableauPileValue()
    {
        return tableauColumn;
    }
    
    private void Start()
    { 
        GameInput.Instance.OnTableauClicked += OnTableauClicked;
    }
    
    private void OnTableauClicked(object sender, GameInput.OnTableauClickedEventArgs e)
    {
       if (e.currentGameObject == gameObject)
        {
            if (canBeStacked(e.previousObject))
            {
                if (e.previousObject.TryGetComponent(out Card card) && (card.IsTopCard() || GameManager.Instance.IsInTripOnDisplay(card.CardScriptableObject)))
                {
                    e.previousObject.GetComponent<Card>().MoveCard(e.currentGameObject,e.previousObject);
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
    
    private bool canBeStacked(GameObject gameobject)
    {
        gameobject.TryGetComponent(out Card card);
        
        Debug.Log($"Card: {card.CardScriptableObject.ToString()} gameobject.transform.childCount: {gameobject.transform.childCount}");
        
        if (card != null)
        {
            if ( card.CardScriptableObject.Value == CardIdentity.CardValue.king && gameobject.transform.childCount == 0)
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
        
        switch (tableauColumn)
        {
            default:
            case Tableau.TableauColumn.ColumnOne:
                columnNumber = 0;
                break;
            case Tableau.TableauColumn.ColumnTwo:
                columnNumber = 1;
                break;
            case Tableau.TableauColumn.ColumnThree:
                columnNumber = 2;
                break;
            case Tableau.TableauColumn.ColumnFour:
                columnNumber = 3;
                break;
            case Tableau.TableauColumn.ColumnFive:
                columnNumber = 4;
                break;
            case Tableau.TableauColumn.ColumnSix:
                columnNumber = 5;
                break;
            case Tableau.TableauColumn.ColumnSeven:
                columnNumber = 6;
                break;
        }
        
        return columnNumber;

    }
    
}
