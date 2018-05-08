using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour {

	public CollectionManager collectionManager;

	public Texture2D leftCursor, rightCursor;

	private float height;
	private float lRight;
	private float rLeft, rRight;

	private void Start(){
		height = GetComponent<RectTransform> ().rect.height;
		lRight = 300;
		rLeft = 1280;
		rRight = GetComponent<RectTransform> ().rect.width;

	}

	private void Update()
	{
		if (!collectionManager.currentPage.Equals(collectionManager.FirstPage()) && InLeft()) {
			Cursor.SetCursor (leftCursor, Vector2.zero, CursorMode.Auto);
			if (Input.GetMouseButtonDown (0)) {
				collectionManager.PreviousPage ();
			}
		} else if (!collectionManager.currentPage.Equals(collectionManager.LastPage()) && InRight()) {
			Cursor.SetCursor (rightCursor, Vector2.zero, CursorMode.Auto);
			if (Input.GetMouseButtonDown (0)) {
				collectionManager.NextPage ();
			}
		}
		else Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
	}

	private bool InLeft(){
		return 0 <= Input.mousePosition.x && Input.mousePosition.x <= lRight && 0 <= Input.mousePosition.y && Input.mousePosition.y <= height;
	}

	private bool InRight(){
		return rLeft <= Input.mousePosition.x && Input.mousePosition.x <= rRight && 0 <= Input.mousePosition.y && Input.mousePosition.y <= height;
	}
}
