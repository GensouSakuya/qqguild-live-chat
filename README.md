本项目魔改自 [bilibili-live-chat](https://github.com/Tsuk1ko/bilibili-live-chat) 项目，加了个对接go-cqhttp qq机器人框架的后端，实现在obs中像弹幕一样展示qq频道直播间的聊天信息

理论上可以改成基于其它任何支持websocket通信和实时获取对话信息的qq机器人框架，但目前只有go-cqhttp支持频道信息，如果未来有其它可用框架需要对接可以开issue


效果图

![49712A3OKZB@_72~PPSWOEL.png](https://s2.loli.net/2022/09/23/iE9eHsmx3LOz4kQ.png)

构建顺序
1. build前端项目src\qqguildlivechat\web
2. 将前端项目dist内的文件拷贝至src\qqguildlivechat\GensouSakuya.QQGuildLiveChat.App\web下
3. 构建GensouSakuya.QQGuildLiveChat.App项目
