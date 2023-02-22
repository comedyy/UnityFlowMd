public static class CommonDrawFlow{
   // 流程开始
   public static void OnStart() {
   }

   // 进入loading
   public static void OnLoading() {
   }

   // 进入321倒计时
   public static void OnCountDown() {
   }

   // 游戏开始
   public static void OnGameStart() {
   }

   // 加载下一个猜画
   public static void OnLoadNextPic() {
   }

   // 等待输入
   public static void WaitForInput() {
   }

   // 输入为画画正确
   public static bool CheckRightInput() {
       return false;
   }

   // 输入为画画错误
   public static bool CheckWrongInput() {
       return false;
   }

   // 输入为重画
   public static bool CheckClearInput() {
       return false;
   }

   // 输入为跳过
   public static bool CheckSkipInput() {
       return false;
   }

   // 输入为异常
   public static void InputNotFindException() {
   }

   // 记录玩家成功
   public static void OnRecordRightInput() {
   }

   // 播放画画成功
   public static void OnPlayRightEffect() {
   }

   // 擦掉当前画作
   public static void OnClearDraw() {
   }

   // 播放跳过表现
   public static void OnPlaySkipEffect() {
   }

}
