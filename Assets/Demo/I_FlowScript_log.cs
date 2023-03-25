using Cysharp.Threading.Tasks;
public interface I_FlowScript_log : ICleanUp{
   // 群聊开始
   UniTask Start();

   // 群友发言
   UniTask Say();

   // 赖子发屎图
   bool IsShit();

   // 群聊结束
   void End();

}
public class WaitMyEnterConst {
   public static string _DEFAULT="DEFAULT";
}
