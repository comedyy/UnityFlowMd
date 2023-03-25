using System.Threading.Tasks;
public class CommonDrawFlow : I_CommonDrawFlow {
   // 流程开始
   void I_CommonDrawFlow.OnStart() {
   }

   // 进入loading
   void I_CommonDrawFlow.OnLoading() {
   }

   // 进入321倒计时
   void I_CommonDrawFlow.OnCountDown() {
   }

   // 游戏开始
   void I_CommonDrawFlow.OnGameStart() {
   }

   // 加载下一个猜画
   void I_CommonDrawFlow.OnLoadNextPic() {
   }

   // 输入为画画正确
   bool I_CommonDrawFlow.CheckRightInput() {
       return false;
   }

   // 输入为画画错误
   bool I_CommonDrawFlow.CheckWrongInput() {
       return false;
   }

   // 输入为重画
   bool I_CommonDrawFlow.CheckClearInput() {
       return false;
   }

   // 输入为跳过
   bool I_CommonDrawFlow.CheckSkipInput() {
       return false;
   }

   // 输入为异常
   void I_CommonDrawFlow.InputNotFindException() {
   }

   // 记录玩家成功
   void I_CommonDrawFlow.OnRecordRightInput() {
   }

   // 播放画画成功
   void I_CommonDrawFlow.OnPlayRightEffect() {
   }

   // 擦掉当前画作
   void I_CommonDrawFlow.OnClearDraw() {
   }

   // 播放跳过表现
   void I_CommonDrawFlow.OnPlaySkipEffect() {
   }

    public void CleanUp()
    {
    }
}
