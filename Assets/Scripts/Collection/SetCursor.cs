using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public static bool cursorSwitched = false;

    public CollectionManager collectionManager;
    public GameObject selectBoardPanel, createLineupPanel;
    public BoardManager boardManager;
    public RectTransform board;
	public Texture2D leftCursor, rightCursor, dragCursor;

    private float lRight, rLeft, rRight;
    private float lowerBound, upperBound, newLowerBound;

	private void Start(){
		lRight = 150;
		rLeft = 1280;
		rRight = GetComponent<RectTransform> ().rect.width;
        lowerBound = transform.position.y + GetComponent<RectTransform>().rect.y;
        upperBound = transform.position.y - GetComponent<RectTransform>().rect.y;
        newLowerBound = lowerBound + board.rect.height;
    }

	private void Update()
	{
        if (selectBoardPanel.activeSelf)
        {
            if (boardManager.currentBoard > 0 && InLeft())
            {
                Cursor.SetCursor(leftCursor, Vector2.zero, CursorMode.Auto);
                if (Input.GetMouseButtonDown(0)) boardManager.PreviousBoard();
            }
            else if (boardManager.currentBoard < boardManager.boardAttributes.Count-1 && InRight())
            {
                Cursor.SetCursor(rightCursor, Vector2.zero, CursorMode.Auto);
                if (Input.GetMouseButtonDown(0)) boardManager.NextBoard();
            }
            else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }
		if (!collectionManager.currentPage.Equals(collectionManager.notFound) && !collectionManager.currentPage.Equals(collectionManager.FirstPage()) && InLeft()) {
			Cursor.SetCursor (leftCursor, Vector2.zero, CursorMode.Auto);
			if (Input.GetMouseButtonDown (0)) {
                cursorSwitched = true;
				collectionManager.PreviousPage ();
			}
		}
        else if (!collectionManager.currentPage.Equals(collectionManager.notFound) && !collectionManager.currentPage.Equals(collectionManager.LastPage()) && InRight()) {
			Cursor.SetCursor (rightCursor, Vector2.zero, CursorMode.Auto);
			if (Input.GetMouseButtonDown (0)) {
                cursorSwitched = true;
                collectionManager.NextPage ();
			}
		}
        else if (createLineupPanel.activeSelf && (TacticGestureHandler.dragBegins || CollectionGestureHandler.dragBegins || LineupBoardGestureHandler.dragBegins))
        {
            cursorSwitched = true;
            Cursor.SetCursor(dragCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            cursorSwitched = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
	}

	private bool InLeft(){
        if(createLineupPanel.activeSelf)
            return 0 <= Input.mousePosition.x && Input.mousePosition.x <= lRight && Input.mousePosition.y <= upperBound && newLowerBound <= Input.mousePosition.y;
        return 0 <= Input.mousePosition.x && Input.mousePosition.x <= lRight && lowerBound <= Input.mousePosition.y && Input.mousePosition.y <= upperBound;
    }

	private bool InRight(){
        if (createLineupPanel.activeSelf)
            return rLeft <= Input.mousePosition.x && Input.mousePosition.x <= rRight && newLowerBound <= Input.mousePosition.y && Input.mousePosition.y <= upperBound;
        return rLeft <= Input.mousePosition.x && Input.mousePosition.x <= rRight && lowerBound <= Input.mousePosition.y && Input.mousePosition.y <= upperBound;
    }
}
