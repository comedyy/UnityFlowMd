using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TextAsset textAsset;
    Flow _flow = new Flow();

    // Start is called before the first frame update
    void Start()
    {
        _flow.Init(textAsset.name, textAsset.text);
    }

    // Update is called once per frame
    void Update()
    {
        _flow.Update();
    }
}
