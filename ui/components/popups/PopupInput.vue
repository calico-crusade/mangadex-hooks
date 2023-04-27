<template>
<Popup v-model="open" :title="title ?? 'Please enter a value:'" @closed="() => cancel()">
    <div class="control">
        <label>{{ label ?? title ?? 'Please enter a value:' }}</label>
        <input type="text" :placeholder="placeholder ?? title ?? ''" v-model="model" />
    </div>
    <footer class="pad-top flex">
        <button class="icon-btn pad-left" @click="() => ok()">
            <Icon>done</Icon>
            <p>Confirm</p>
        </button>
    </footer>
</Popup>
</template>

<script setup lang="ts">
interface Props {
    modelValue?: string;
    prompt?: number;
    title: string;
    label?: string;
    placeholder?: string;
    default?: string;
}

interface Emits {
    (e: 'update:modelValue', value?: string): void;
    (e: 'canceled'): void;
    (e: 'confirmed', value?: string): void;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();

const open = ref(false);
const model = computed({
    get() { return props.modelValue || props.default || ''; },
    set(value?: string) { emits('update:modelValue', value); }
});

const cancel = () => { emits('canceled'); open.value = false; }
const ok = () => { emits('confirmed', model.value); open.value = false; }

watch(() => props.prompt, () => open.value = true);
</script>