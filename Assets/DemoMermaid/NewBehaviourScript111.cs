using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript111 : MonoBehaviour
{
    public string str;
    public bool x;
    public TextAsset asset;
    Flow flow;
    // Start is called before the first frame update
    void Start()
    {
        flow = FlowMgr.Instance.AddFlow(asset, "xxx");
        flow.Inject(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(x)
        {
            x = false;
            flow.SetParam(str);
        }
    }
}
