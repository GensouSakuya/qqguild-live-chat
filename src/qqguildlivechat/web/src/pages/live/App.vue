<template>
  <danmaku-item v-if="errMsg" type="info" :message="errMsg" />
  <live v-else-if="ready" v-bind="props" />
</template>

<script>
import { reactive, onBeforeUnmount, ref } from 'vue';
import { parseProps } from '@/utils/props';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

import Live from '@/components/Live';
import DanmakuItem from '@/components/DanmakuItem';

export default {
  components: { Live, DanmakuItem },
  setup() {
    const onHashChange = () => window.location.reload();
    window.addEventListener('hashchange', onHashChange);
    onBeforeUnmount(() => window.removeEventListener('hashchange', onHashChange));

    const props = reactive(parseProps(window.location.hash));

    const ready = ref(false);
    const errMsg = ref('');

    const host = window.location.host;

    const connection = new HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl(`http://${host}/chat?room=${props.room}&guildName=${props.guildName}`)
      .configureLogging(LogLevel.Information)
      .build()

    props.connection = connection;
    props.face = true;
    ready.value = true;

    return { props, ready, errMsg };
  },
};
</script>

<style>
html,
body,
#app {
  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
  background-color: transparent;
}
</style>
