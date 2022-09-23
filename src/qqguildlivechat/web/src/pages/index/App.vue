<template>
  <div id="panel" class="panel panel-default">
    <div class="panel-heading">
      <h2 class="panel-title" style="font-size: 30px; display: inline-block; margin-right: 10px">Bilibili Live Chat</h2>
    </div>
    <div class="panel-body">
      <!-- 频道名称 -->
      <input-group header="频道名称">
        <input
          class="form-control"
          type="text"
          placeholder="必填"
          v-model="form.guildName"
        />
      </input-group>
      <!-- 直播间名称 -->
      <input-group header="直播间名称">
        <input
          class="form-control"
          type="text"
          placeholder="必填"
          v-model="form.room"
        />
        <span class="input-group-btn">
          <button class="btn btn-primary" type="button" :disabled="!form.room || !form.guildName" @click="goLive">Go!</button>
        </span>
      </input-group>
      <!-- 弹幕排列 -->
      <input-group header="弹幕排列">
        <select class="form-control" v-model="form.display">
          <option v-for="{ value, text } in options.display" :key="value" :value="value">{{ text }}</option>
        </select>
      </input-group>
      <!-- 弹幕停留 -->
      <input-group header="弹幕停留" footer="毫秒">
        <input
          class="form-control"
          type="number"
          min="0"
          step="1"
          placeholder="选填，弹幕过这么久后会被隐藏，仅弹幕排列为“自底部”时有效"
          v-model.number="form.stay"
        />
      </input-group>
      <!-- 频率限制 -->
      <input-group header="频率限制" footer="条/秒">
        <input
          type="number"
          min="1"
          step="1"
          class="form-control"
          placeholder="选填，限制弹幕频率（不包括礼物），若超出频率则会随机丢弃弹幕"
          v-model.number="form.limit"
        />
      </input-group>
      <!-- 礼物合并 -->
      <input-group header="礼物合并" footer="毫秒">
        <input
          class="form-control"
          type="number"
          min="0"
          step="1"
          placeholder="选填，合并统计的等待时间，不知道填多少可填 5000"
          v-model.number="form.giftComb"
        />
      </input-group>
      <!-- 礼物置顶 -->
      <input-group header="礼物置顶" footer="条">
        <input
          class="form-control"
          type="number"
          min="0"
          step="1"
          placeholder="选填，可将礼物置顶，与弹幕分开展示，此项相当于设置礼物区域的高度"
          v-model.number="form.giftPin"
        />
      </input-group>
      <!-- 弹幕延迟 -->
      <input-group header="弹幕延迟" footer="秒">
        <input
          class="form-control"
          type="number"
          min="0"
          step="1"
          placeholder="选填，收到弹幕后延迟这么久才会显示"
          v-model.number="form.delay"
        />
      </input-group>
      <!-- 屏蔽用户 -->
      <!-- <input-group header="屏蔽用户" footer="">
        <input
          class="form-control"
          type="text"
          placeholder="选填，将不显示指定UID用户的弹幕和礼物，用竖杠|分隔"
          v-model="form.blockUID"
        />
      </input-group> -->
    </div>
  </div>
</template>

<script>
import { unref, reactive, watchEffect, computed, readonly } from 'vue';
import InputGroup from '@/components/InputGroup';
import { sget, sset } from '@/utils/storage';
import { defaultProps, intProps, selectOptions } from '@/utils/props';
import { stringify as qss } from 'querystring';
import { fromPairs, pick } from 'lodash';

export default {
  components: { InputGroup },
  setup() {
    const form = reactive({
      ...defaultProps,
      ...sget('setting', {}),
    });
    intProps.forEach(key => {
      watchEffect(() => {
        if (typeof form[key] === 'number') form[key] = Math.max(Math.floor(form[key]), 0);
      });
    });

    const simpleForm = computed(() =>
      pick(
        fromPairs(
          Object.entries(form)
            .filter(([k, v]) => {
              const val = unref(v);
              return val && val !== defaultProps[k];
            })
            .map(([k, v]) => [k, unref(v)])
        ),
        Object.keys(defaultProps)
      )
    );
    watchEffect(() => {
      sset('setting', simpleForm.value);
    });

    return {
      form,
      goLive: () => (window.location.href = `live.html#${qss(simpleForm.value)}`),
      options: readonly(selectOptions),
    };
  },
};
</script>

<style>
body,
html {
  width: 100%;
  height: 100%;
}
body {
  display: flex;
  align-items: center;
  justify-content: center;
}
#app {
  margin: 16px;
}
@media screen and (min-width: 800px) {
  #app {
    width: 70%;
    min-width: 768px;
    max-width: 1024px;
  }
}
@media screen and (max-width: 799px) {
  #app {
    width: 100%;
  }
}
#panel {
  margin: 0;
}
.form-control {
  box-shadow: none !important;
}
input[type='checkbox'] {
  vertical-align: middle;
}
label {
  margin-bottom: 0;
  font-weight: 400;
}
.input-group:not(:last-child) {
  margin-bottom: 10px;
}
a {
  text-decoration: none !important;
}

.bl-0 {
  border-left: 0;
}
</style>
