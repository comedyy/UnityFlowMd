using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TextAsset textAsset;

    // Start is called before the first frame update
    void Start()
    {
        Flow flow = new Flow();
        flow.Init(textAsset.name, textAsset.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
