本项目魔改自 bilibili-live-chat 项目，通过对接go-cqhttp qq机器人框架，实现将qq频道直播间信息能够以弹幕的方式在OBS里展示出来

构建顺序
1. build前端项目src\qqguildlivechat\web
2. 将前端项目dist内的文件拷贝至src\qqguildlivechat\GensouSakuya.QQGuildLiveChat.App\web下
3. 构建GensouSakuya.QQGuildLiveChat.App项目