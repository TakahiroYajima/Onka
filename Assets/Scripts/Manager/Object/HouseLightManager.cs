using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HouseLightManager : SingletonMonoBehaviour<HouseLightManager>
{
    private List<Light> stageLightList = new List<Light>();
    // Start is called before the first frame update
    void Start()
    {
        stageLightList = GetComponentsInChildren<Light>().ToList();
        foreach(var l in stageLightList)
        {
            l.enabled = false;
        }
    }

}
