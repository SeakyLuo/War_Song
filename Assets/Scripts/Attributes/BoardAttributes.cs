using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attributes", menuName = "Board")]
public class BoardAttributes : ScriptableObject {

    public string Name;
    public bool available = true;
    public Sprite allyFieldImage, completeImage;
    public int boardWidth = 9;
    public int boardHeight = 10;
    public int allyField = 4; // from 0 to allyfield
    public int enemyField = 5; // from enemyField to boardHeight-1
    [TextArea(2, 3)]
    public string description;

    // Index not height
    public int palaceLeft = 3;
    public int palaceRight = 5;
    public int palaceUp = 2;
    public int palaceDown = 0;
    public int epalaceLeft = 3;
    public int epalaceRight = 5;
    public int epalaceUp = 9;
    public int epalaceDown = 7;

    public Location asloc1 = new Location(0, 3);
    public Location asloc2 = new Location(2, 3);
    public Location asloc3 = new Location(4, 3);
    public Location asloc4 = new Location(6, 3);
    public Location asloc5 = new Location(8, 3);
    public Location acloc1 = new Location(1, 2);
    public Location acloc2 = new Location(7, 2);
    public Location arloc1 = new Location(0, 0);
    public Location arloc2 = new Location(8, 0);
    public Location ahloc1 = new Location(1, 0);
    public Location ahloc2 = new Location(7, 0);
    public Location aeloc1 = new Location(2, 0);
    public Location aeloc2 = new Location(6, 0);
    public Location aaloc1 = new Location(3, 0);
    public Location aaloc2 = new Location(5, 0);
    public Location agloc = new Location(4, 0);

    public Location esloc1 = new Location(0, 6);
    public Location esloc2 = new Location(2, 6);
    public Location esloc3 = new Location(4, 6);
    public Location esloc4 = new Location(6, 6);
    public Location esloc5 = new Location(8, 6);
    public Location ecloc1 = new Location(1, 7);
    public Location ecloc2 = new Location(7, 7);
    public Location erloc1 = new Location(0, 9);
    public Location erloc2 = new Location(8, 9);
    public Location ehloc1 = new Location(1, 9);
    public Location ehloc2 = new Location(7, 9);
    public Location eeloc1 = new Location(2, 9);
    public Location eeloc2 = new Location(6, 9);
    public Location ealoc1 = new Location(3, 9);
    public Location ealoc2 = new Location(5, 9);
    public Location egloc = new Location(4, 9);

    public List<Location> AdvisorCastle() { return new List<Location> { aaloc1, aaloc2 }; }
    public List<Location> ElephantCastle() { return new List<Location> { aeloc1, aeloc2 }; }
    public List<Location> HorseCastle() { return new List<Location> { ahloc1, ahloc2 }; }
    public List<Location> ChariotCastle() { return new List<Location> { arloc1, arloc2 }; }
    public List<Location> CannonCastle() { return new List<Location> { acloc1, acloc2 }; }
    public List<Location> SoldierCastle() { return new List<Location> { asloc1, asloc2, asloc3, asloc4, asloc5 }; }
    public List<Location> AllyCastles() { return new List<Location> { aaloc1, aaloc2, aeloc1, aeloc2, ahloc1, ahloc2, arloc1, arloc2, acloc1, acloc2, asloc1, asloc2, asloc3, asloc4, asloc5, agloc }; }
    public List<Location> EnemyAdvisorCastle() { return new List<Location> { ealoc1, aaloc2 }; }
    public List<Location> EnemyElephantCastle() { return new List<Location> { eeloc1, aeloc2 }; }
    public List<Location> EnemyHorseCastle() { return new List<Location> { ehloc1, ahloc2 }; }
    public List<Location> EnemyChariotCastle() { return new List<Location> { erloc1, arloc2 }; }
    public List<Location> EnemyCannonCastle() { return new List<Location> { ecloc1, acloc2 }; }
    public List<Location> EnemySoldierCastle() { return new List<Location> { esloc1, asloc2, asloc3, asloc4, asloc5 }; }
    public List<Location> EnemyCastles() { return new List<Location> { ealoc1, ealoc2, eeloc1, eeloc2, ehloc1, ehloc2, erloc1, erloc2, ecloc1, ecloc2, esloc1, esloc2, esloc3, esloc4, esloc5, egloc }; }
    public bool InAllyField(int x, int y) { return 0 <= x && x < boardWidth && 0 <= y && y <= allyField; }
    public bool InPalace(int x, int y) { return palaceLeft <= x && x <= palaceRight && palaceDown <= y && y <= palaceUp; }
    public bool InEnemyPalace(int x, int y) { return epalaceLeft <= x && x <= epalaceRight && epalaceDown <= y && y <= epalaceUp; }
    public bool InEnemyRegion(int x, int y) { return 0 <= x && x < boardWidth && enemyField <= y && y < boardHeight; }
    public bool InEnemyCastle(int x, int y) { return EnemyCastles().Contains(new Location(x, y)); }
    public bool AtEnemyBottom(int x, int y) { return 0 <= x && x < boardWidth && y == boardHeight; }
    public bool InBoard(int x, int y) { return 0 <= x && x < boardWidth && 0 <= y && y < boardHeight; }
}
