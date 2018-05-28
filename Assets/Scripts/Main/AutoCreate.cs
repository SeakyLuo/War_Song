using System.IO;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AutoCreate : MonoBehaviour {
    public GameObject copy;
    public int row;
    public int column;
    public GameObject gridPanel;
    public GameObject grid;
    public GameObject piece;

	// Use this for initialization
	void Start () {
        //for (int x = 0; x < row; x++)
        //{
        //    for (int y = 0; y < column; y++)
        //    {
        //        GameObject obj = Instantiate(copy);
        //        obj.name = y.ToString() + x.ToString();
        //        obj.transform.position = new Vector3(y, x, 0);
        //    }
        //}
        //BoardGenerator(Database.FindBoardAttributes("Standard Board"));
        LineupBoardGenerator(Database.FindBoardAttributes("Standard Board"));

    }

    public void LineupBoardGenerator(BoardAttributes board)
    {
        string address = "Board/" + board.Name + "/Images/";
        for (int x = 0; x <= board.allyField; x++)
        {
            for (int y = 0; y < board.boardWidth; y++)
            {
                Location location = new Location(y, x);
                GameObject obj = Instantiate(gridPanel, transform);
                string image = address;
                if (location == new Location(board.palaceLeft, board.palaceDown))
                    image += "pdl";
                else if (location == new Location(board.palaceRight, board.palaceUp))
                    image += "pur";
                else if (location == new Location(board.palaceRight, board.palaceDown))
                    image += "prd";
                else if (location == new Location(board.palaceLeft, board.palaceUp))
                    image += "plu";
                else if (location.Between(new Location(board.palaceLeft, board.palaceDown), new Location(board.palaceRight, board.palaceDown), "X"))
                    image += "pd";
                else if (location.Between(new Location(board.palaceLeft, board.palaceUp), new Location(board.palaceRight, board.palaceUp), "X"))
                    image += "pu";
                else if (location.Between(new Location(board.palaceRight, board.palaceDown), new Location(board.palaceRight, board.palaceUp), "Y"))
                    image += "pr";
                else if (location.Between(new Location(board.palaceLeft, board.palaceDown), new Location(board.palaceLeft, board.palaceUp), "Y"))
                    image += "pl";
                else if (board.AllyCastles().Contains(location))
                    image += "castle";
                else
                    image += "grid";
                if (board.AllyCastles().Contains(location)) obj.AddComponent<MouseOverPiece>();
                obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(image);
                obj.name = location.ToString();
            }
        }
    }

    public void BoardGenerator(BoardAttributes board)
    {
        string address = "Board/" + board.Name + "/Images/";
        for (int x = 0; x < board.boardHeight; x++)
        {
            for (int y = 0; y < board.boardWidth; y++)
            {
                Location location = new Location(y, x);
                GameObject obj = Instantiate(grid, transform);
                obj.transform.localPosition = new Vector3(y, x);
                string image = address;
                if (location == new Location(board.palaceLeft, board.palaceDown) || location == new Location(board.epalaceLeft, board.epalaceDown))
                    image += "pdl";
                else if (location == new Location(board.palaceRight, board.palaceUp) || location == new Location(board.epalaceRight, board.epalaceUp))
                    image += "pur";
                else if (location == new Location(board.palaceRight, board.palaceDown) || location == new Location(board.epalaceRight, board.epalaceDown))
                    image += "prd";
                else if (location == new Location(board.palaceLeft, board.palaceUp) || location == new Location(board.epalaceLeft, board.epalaceUp))
                    image += "plu";
                else if (location.Between(new Location(board.palaceLeft, board.palaceDown), new Location(board.palaceRight, board.palaceDown), "X") ||
                    location.Between(new Location(board.epalaceLeft, board.epalaceDown), new Location(board.epalaceRight, board.epalaceDown), "X"))
                    image += "pd";
                else if (location.Between(new Location(board.palaceLeft, board.palaceUp), new Location(board.palaceRight, board.palaceUp), "X") ||
                    location.Between(new Location(board.epalaceLeft, board.epalaceUp), new Location(board.epalaceRight, board.epalaceUp), "X"))
                    image += "pu";
                else if (location.Between(new Location(board.palaceRight, board.palaceDown), new Location(board.palaceRight, board.palaceUp), "Y") ||
                    location.Between(new Location(board.epalaceRight, board.epalaceDown), new Location(board.epalaceRight, board.epalaceUp), "Y"))
                    image += "pr";
                else if (location.Between(new Location(board.palaceLeft, board.palaceDown), new Location(board.palaceLeft, board.palaceUp), "Y") ||
                    location.Between(new Location(board.epalaceLeft, board.epalaceDown), new Location(board.epalaceLeft, board.epalaceUp), "Y"))
                    image += "pl";
                else if (board.AllyCastles().Contains(location) || board.EnemyCastles().Contains(location))
                    image += "castle";
                else
                    image += "grid";
                obj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(image);
                obj.name = location.ToString();
                if (board.AllyCastles().Contains(location) || board.EnemyCastles().Contains(location))
                {
                    GameObject piececlone = Instantiate(piece, obj.transform);
                    piececlone.name = "Piece";
                    piececlone.transform.localScale = new Vector3(grid.transform.localScale.x * 0.8f, grid.transform.localScale.y * 0.8f, grid.transform.localScale.z);
                }

            }
        }
    }
}
