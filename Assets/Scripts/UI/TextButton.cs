using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TextButton : MonoBehaviour
{
    public Text text = null;
    public Button button = null;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
}
