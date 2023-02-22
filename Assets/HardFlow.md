```flow
st=>start: 页面加载|OnStart
e=>end: End|OnEnd
op1=>operation: get_hotel_ids|OnA
sub1=>operation: get_proxy|OnB
op3=>operation: save_comment|OnC
op4=>operation: set_sentiment|OnD
op5=>operation: set_record|OnE
 
cond1=>condition: ids_remain空?|XX1
cond2=>condition: proxy_list空?|XX2
cond3=>condition: ids_got空?|XX3
cond4=>condition: 爬取成功??|XX4
cond5=>condition: ids_remain空?|XX5
 
io1=>operation: ids-remain|XX6
io2=>operation: proxy_list|XX7
io3=>operation: ids-got|XX8
 
st->op1(right)->io1->cond1
cond1(yes)->sub1->io2->cond2
cond2(no)->op3
cond2(yes)->sub1
cond1(no)->op3->cond4
cond4(yes)->io3->cond3
cond4(no)->io1
cond3(no)->op4
cond3(yes, right)->cond5
cond5(yes)->op5
cond5(no)->cond3
op5->e
op4->e
```