```flow
start=>start: 流程开始|OnStart
loading=>operation: 进入loading|OnLoading
countDown=>operation: 进入321倒计时|OnCountDown
gameStart=>operation: 游戏开始|OnGameStart
loadNextPic=>operation: 加载下一个猜画|OnLoadNextPic
waitInput=>inputoutput: 等待输入|WaitForInput
inputRight=>condition: 输入为画画正确|CheckRightInput
inputWrong=>condition: 输入为画画错误|CheckWrongInput
inputRepaint=>condition: 输入为重画|CheckClearInput
inputSkip=>condition: 输入为跳过|CheckSkipInput
inputNotFind=>end: 输入为异常|InputNotFindException
recordInput=>operation: 记录玩家成功|OnRecordRightInput
playRightEffect=>operation: 播放画画成功|OnPlayRightEffect
playClearEffect=>operation: 擦掉当前画作|OnClearDraw
playSkipEffect=>operation: 播放跳过表现|OnPlaySkipEffect

start->loading->countDown->gameStart->loadNextPic->waitInput()->inputRight
inputRight(no)->inputRepaint(no,bottom)->inputSkip(no,bottom)->inputWrong(no)->inputNotFind
inputRight(yes)->recordInput(left)->playRightEffect->waitInput
inputRepaint(yes)->playClearEffect->waitInput
inputSkip(yes)->playSkipEffect->waitInput
inputWrong(yes,right)->waitInput

```