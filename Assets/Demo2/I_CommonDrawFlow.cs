using Cysharp.Threading.Tasks;
public interface I_CommonDrawFlow : ICleanUp{
   // 流程开始
   void OnStart();

   // 进入loading
   void OnLoading();

   // 进入321倒计时
   void OnCountDown();

   // 游戏开始
   void OnGameStart();

   // 加载下一个猜画
   void OnLoadNextPic();

   // 输入为画画正确
   bool CheckRightInput();

   // 输入为画画错误
   bool CheckWrongInput();

   // 输入为重画
   bool CheckClearInput();

   // 输入为跳过
   bool CheckSkipInput();

   // 输入为异常
   void InputNotFindException();

   // 记录玩家成功
   void OnRecordRightInput();

   // 播放画画成功
   void OnPlayRightEffect();

   // 擦掉当前画作
   void OnClearDraw();

   // 播放跳过表现
   void OnPlaySkipEffect();

}
public class WaitForInputConst {
   public static string _DEFAULT="DEFAULT";
}
