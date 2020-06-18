using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianManager : MonoBehaviour
{

    public List<GameObject> civilianList;


    void Start()
    {
        civilianList = new List<GameObject>();  //Add all civilians to list
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject != this.gameObject)
            {
                civilianList.Add(this.transform.GetChild(i).gameObject);
            }

        }

    }


    public void DeleteCiv(GameObject inObject)
    {
        civilianList.Remove(inObject);
        Destroy(inObject);
    }

    public List<GameObject> UpdateCivilianList()
    {
        return civilianList;
    }


    void Update()
    {
        
    }
}
