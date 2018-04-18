using UnityEngine;
using UnityEngine.UI;
using System;

public class UpdateTime : MonoBehaviour {

    public Text time;

    private void FixedUpdate()
    {
        time.text = DateTime.Now.ToString("h:mm tt");
    }
}
