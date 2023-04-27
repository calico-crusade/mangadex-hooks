<template>
    <Popup v-model="open" :title="title ?? 'Loading...'" stop-close>
        <Loading inline />
    </Popup>
</template>

<script setup lang="ts">
interface Props {
    modelValue: boolean;
    title?: string;
}

interface Emits {
    (e: 'update:modelValue', value: boolean): void;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();

const open = computed({
    get() { return props.modelValue; },
    set(value: boolean) { emits('update:modelValue', value); }
});
</script>