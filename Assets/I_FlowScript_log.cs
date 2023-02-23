using System.Threading.Tasks;
public interface I_FlowScript_log {
   // 群聊开始
   Task Start();

   //等待我输入
   void WaitMyEnter(object param);

   // 群友发言
   Task Say();

   // 赖子发屎图
   bool IsShit();

   // 群聊结束
   void End();

}
