using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TextAsset textAsset;
    public bool x;
    public bool y;

    public Flow Flow { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // Flow = FlowMgr.Instance.AddFlow(textAsset, "newOne");
    }

    int i  = 0;
    // Update is called once per frame
    void Update()
    {
        if(x)
        {
            x = false;
            Flow.SetParam(null);
        }

        if(y)
        {
            y = false;
            Flow = FlowMgr.Instance.AddFlow<FlowScript_log>(textAsset, (i++).ToString());
            Flow.Inject(Flow.Title);
        }
    }
}
