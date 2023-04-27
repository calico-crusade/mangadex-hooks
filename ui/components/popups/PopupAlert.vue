<template>
<Popup v-model="open" :title="title ?? 'Please enter a value:'" @closed="() => cancel()">
    <div class="center">
        <h3 v-if="!isMarkdown">{{ content ?? title ?? 'Something important happened!' }}</h3>
        <Markdown v-else :content="content ?? title ?? 'Something important happened'" />
    </div>
    <footer class="pad-top flex">
        <button class="icon-btn pad-left" @click="() => ok()">
            <Icon>done</Icon>
            <p>Ok</p>
        </button>
    </footer>
</Popup>
</template>
    
<script setup lang="ts">
interface Props {
    title?: string;
    content?: string;
    isMarkdown?: boolean;
    resetSwitch?: any;
}

interface Emits {
    (e: 'canceled'): void;
    (e: 'confirmed'): void;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();

const open = ref(false);

const cancel = () => { emits('canceled'); open.value = false; }
const ok = () => { emits('confirmed'); open.value = false; }

watch(() => props.resetSwitch, () => open.value = true);
</script>

<style lang="scss" scoped>
.center {
    max-width: 100%;
    overflow: hidden;
}
</style>