using UnityEngine;

[ExecuteInEditMode]
public class AutoCreate : MonoBehaviour {
    public GameObject copy;
    public int row;
    public int column;

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
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                GameObject obj = Instantiate(copy, transform);
                obj.name = "Card" + (x * column + y).ToString();
            }
        }
    }
}
