using System.Threading.Tasks;
using UnityEngine;

public class FlowScript_log : I_FlowScript_log {
        [FlowNeedInject] string context;
     string noInject = "noInject";

   // 群聊开始
   async Task I_FlowScript_log.Start() {
       Debug.Log($"enter Start {context}");
        await Task.Delay(1000);
        Debug.Log($"Exit Start {context} - {noInject}");
   }

   // 群友发言
   async Task I_FlowScript_log.Say() {
               await Task.Delay(2000);
        Debug.Log($"enter say  {context}");
   }

   // 赖子发屎图
   bool I_FlowScript_log.IsShit() {
               Debug.Log($"isShit  {context}");
        return Random.value > 0.5f;
   }

   // 群聊结束
   void I_FlowScript_log.End() {
        Debug.Log($"End  {context}");
   }

}
