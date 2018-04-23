using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

	public void Back()
    {
        gameObject.SetActive(false);
    }

    public void ShowCredits()
    {
        gameObject.SetActive(true);
    }
}
