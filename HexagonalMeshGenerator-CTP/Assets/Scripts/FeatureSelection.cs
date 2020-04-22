using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureSelection : MonoBehaviour
{
    public HexMapEditor hme;

    public Dictionary<FeaturesType, bool> featureDictionary;

    private void Start()
    {
        featureDictionary = new Dictionary<FeaturesType, bool> 
        {
            {FeaturesType.TREES, false },
            {FeaturesType.HARBOUR, false },
            {FeaturesType.BUILDINGS, false }
        };
    }


    public void ToggleFeature(int _type)
    {
        featureDictionary[(FeaturesType)_type] = !featureDictionary[(FeaturesType)_type];
        CreateString();
    }

    void CreateString()
    {
        int count = 0;
        string tempName = "";
        foreach(KeyValuePair<FeaturesType, bool> entry in featureDictionary)
        {
            if(entry.Value != false)
            {
                if(count == 0)
                {
                    tempName = entry.Key.ToString();
                    count++;
                }
                else
                {
                    tempName += ("_" + entry.Key.ToString());
                    count++;
                }
            }
        }
        Debug.Log(tempName);
        hme.activePrefab = tempName;
    }

}
