using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TextAsset textAsset;
    FlowMgr _flowMgr = new FlowMgr();
    List<Flow> _allFlow = new List<Flow>();

    // Start is called before the first frame update
    void Start()
    {
        _allFlow.Add(_flowMgr.AddFlow(textAsset.name, textAsset.text));
    }

    // Update is called once per frame
    void Update()
    {
        _flowMgr.Update();

        if(Random.value > 0.5f)
        {
            _flowMgr.AddFlow(textAsset.name, textAsset.text);
        }
        else if(Random.value < 0.3f)
        {
            _flowMgr.RemoveFlow(_allFlow[Random.Range(0, _allFlow.Count)]);
        }
    }
}
