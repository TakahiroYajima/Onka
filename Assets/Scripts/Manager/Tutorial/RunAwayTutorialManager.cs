using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayTutorialManager : MonoBehaviour
{
    [SerializeField] private TextureAnimation[] tutorialObjects = null;
    [SerializeField] private FloatingAnimation lastArrowObject = null;

    public void SetUp()
    {
        foreach (var obj in tutorialObjects)
        {
            obj.SetUp();
            obj.gameObject.SetActive(false);
        }
        lastArrowObject.SetUp();
        lastArrowObject.gameObject.SetActive(false);
    }

    public void StartTutorial()
    {
        foreach(var obj in tutorialObjects)
        {
            obj.gameObject.SetActive(true);
            obj.StartAction();
        }
        lastArrowObject.gameObject.SetActive(true);
        lastArrowObject.StartAction();
    }

    public void EndTutorial()
    {
        foreach (var obj in tutorialObjects)
        {
            obj.gameObject.SetActive(false);
        }
        lastArrowObject.gameObject.SetActive(false);
    }
}
