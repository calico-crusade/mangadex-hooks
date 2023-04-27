<template>
    <div class="tab" :class="{ 'active': isActive, dark }">
        <div class="tab-panel" :class="[className, scrollable ? 'scroll' : '']" v-if="keepAlive || isActive">
            <slot />
        </div>
    </div>
</template>

<script lang="ts" setup>
import { toRef, reactive } from 'vue'
import { Params } from '~/utils/api-helper';
import { useTab } from '~/utils/tabs';

interface TabData {
  title: string;
  icon?: string;
  scrollable?: boolean;
  keepAlive?: boolean;
  dark?: boolean;
  className?: string;
}

const props = defineProps<TabData>();

const refProps: Params = {};

for(const key in props) {
    refProps[key] = toRef(props, key as keyof TabData);
}

const tabData = reactive(<TabData>refProps);
const { isActive } = useTab(tabData);
</script>

<style lang="scss">
.scroll {
    overflow-y: auto;
}
</style>