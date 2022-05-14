using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffRollItem : MonoBehaviour
{
    [SerializeField] private Text titleText = null;
    //[SerializeField] private Text nameText = null;

    Color color;
    public void Init(string _title, TextAnchor textAlignment)//, string _name = "")
    {
        titleText.text = _title;
        titleText.alignment = textAlignment;
        //nameText.text = _name;
        //if (string.IsNullOrEmpty(_name))
        //{
        //    nameText.gameObject.SetActive(false);
        //    titleText.alignment = TextAnchor.MiddleCenter;
        //}
        //else
        //{
        //    nameText.gameObject.SetActive(true);
        //    titleText.alignment = TextAnchor.MiddleRight;
        //}
        color = titleText.color;
        SetAlpha(0f);
    }

    public void SetAlpha(float alpha)
    {
        color.a = alpha;
        titleText.color = color;
        //nameText.color = color;
    }
}
