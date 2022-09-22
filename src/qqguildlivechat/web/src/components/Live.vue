<template>
  <div id="live">
    <danmaku-list
      ref="giftPinList"
      v-bind="props"
      :gift-show-face="giftShowFace"
      :is-gift-list="true"
      v-if="props.giftPin"
    />
    <danmaku-list ref="danmakuList" v-bind="props" />
  </div>
</template>

<script>
import { onBeforeUnmount, ref, onMounted, computed } from 'vue';
import { propsType } from '@/utils/props';

import DanmakuList from '@/components/DanmakuList';

export default {
  components: { DanmakuList },
  props: propsType,
  setup(props) {
    const giftPinList = ref(null);
    const danmakuList = ref(null);

    const addInfoDanmaku = message => {
      danmakuList.value.addDanmaku({
        type: 'info',
        message,
        stay: props.stay || 5000,
      });
    };
    const addDanmaku = danmaku => {
      if (props.limit) danmakuList.value.addSpeedLimitDanmaku(danmaku);
      else danmakuList.value.addDanmaku(danmaku);
    };

    onMounted(() => {
      props.connection.on('SendChatMessage', ({ message, uid, name, isOwner, avatarUrl /*, isVip, isSvip*/}) => {
        const danmaku = {
          type: 'message',
          showFace: true,
          face: avatarUrl,
          uid,
          uname: name,
          message,
          isAnchor: uid === props.anchor,
          isOwner: !!isOwner,
        };
        if (props.delay > 0) setTimeout(() => addDanmaku(danmaku), props.delay * 1000);
        else addDanmaku(danmaku);
      })
      props.connection.onreconnecting(error => {
        addInfoDanmaku(`连接错误，正在重新连接`);
      });

      console.log('正在连接直播弹幕服务');
      props.connection.start();
      addInfoDanmaku(`已连接直播间 ${props.room}`);

      // 礼物
      // const giftList = props.giftPin ? giftPinList : danmakuList;
      // live.on('SEND_GIFT', ({ data: { uid, uname, action, giftName, num, face } }) => {
      //   if (isBlockedUID(uid)) {
      //     console.log(`屏蔽了来自[${uname}]的礼物：${giftName}*${num}`);
      //     return;
      //   }
      //   setFace(uid, face);
      //   if (props.giftComb) {
      //     const key = `${uid}-${giftName}`;
      //     const existComb = giftCombMap.get(key);
      //     if (existComb) {
      //       giftCombMap.set(key, {
      //         ...existComb,
      //         num: existComb.num + num,
      //       });
      //     } else {
      //       giftCombMap.set(key, {
      //         type: 'gift',
      //         showFace: props.face !== 'false',
      //         uid,
      //         uname,
      //         action,
      //         giftName,
      //         num,
      //       });
      //       setTimeout(() => {
      //         giftList.value.addDanmaku(giftCombMap.get(key));
      //         giftCombMap.delete(key);
      //       }, props.giftComb);
      //     }
      //   } else {
      //     giftList.value.addDanmaku({
      //       type: 'gift',
      //       showFace: props.face !== 'false',
      //       uid,
      //       uname,
      //       action,
      //       giftName,
      //       num,
      //     });
      //   }
      // });

      // SC
      // live.on('SUPER_CHAT_MESSAGE', fullData => {
      //   console.log('SUPER_CHAT_MESSAGE', fullData);
      //   const {
      //     data: {
      //       uid,
      //       user_info: { uname },
      //       message,
      //     },
      //   } = fullData;
      //   giftList.value.addDanmaku({
      //     type: 'sc',
      //     showFace: props.face !== 'false',
      //     uid,
      //     uname,
      //     message,
      //   });
      // });
      // window.giftList = giftList;

      // // 舰长
      // live.on('USER_TOAST_MSG', fullData => {
      //   const {
      //     data: { uid, username: uname, role_name: giftName, num },
      //   } = fullData;
      //   giftList.value.addDanmaku({
      //     type: 'gift',
      //     showFace: props.face !== 'false',
      //     uid,
      //     uname,
      //     giftName,
      //     num,
      //   });
      // });
    });

    return { props, giftPinList, danmakuList };
  },
};
</script>

<style lang="scss">
#live {
  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
  background-color: transparent;
  display: flex;
  flex-direction: column;
}
</style>
