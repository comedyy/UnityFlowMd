using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TextAsset textAsset;
    FlowMgr _flowMgr = new FlowMgr();

    public Flow Flow { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Flow = _flowMgr.AddFlow(textAsset.name, textAsset.text);
    }

    // Update is called once per frame
    void Update()
    {
        _flowMgr.Update();
    }
}
