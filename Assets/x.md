```flow
script=>FlowScript_log

st=>start: 群聊开始|Start
op=>operation: 群友发言|Say
con=>condition: 赖子发屎图|IsShit
en=>end: 群聊结束|End

st->op->con(yes,bottom)->en
con(no,left)->op
```