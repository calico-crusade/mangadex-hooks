<template>
    <div class="popup-container flex" :class="{ 'open': open }">
        <div class="popup-fade" @click="() => close()"></div>
        <div class="popup center flex row overflow" v-if="open" :style="maxSize ? 'max-width:' + maxSize : ''">
            <header class="flex center-items">
                <h3 class="title fill">{{ title || 'Popup' }}</h3>
                <button v-if="!stopClose" @click="() => close()">
                    <Icon>close</Icon>
                </button>
            </header>
            <main class="popup-content fill flex row scroll-y">
                <slot />
            </main>
        </div>
    </div>
</template>

<script setup lang="ts">
interface Props {
    modelValue: boolean;
    title?: string;
    stopClose?: boolean;
    maxSize?: string;
}

interface Emits {
    (e: 'closed'): void;
    (e: 'opened'): void;
    (e: 'update:modelValue', val: boolean): void;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();


const open = computed({
    get() { return props.modelValue; },
    set(value: boolean) { 
        emits('update:modelValue', value);

        if (value) emits('opened'); else emits('closed');
    }
});

const close = () => {
    if (props.stopClose) return;
    open.value = false;
}
</script>

<style lang="scss" scoped>
$zindex: 90;
.popup-container {
    position: fixed;
    top: 0;
    left: 100%;
    width: max(100vw, 100%);
    height: max(100vh, 100%);
    transition: opacity 250ms;
    z-index: $zindex;
    opacity: 0;

    .popup-fade {
        position: absolute;
        top: 0;
        left: 0;
        width: max(100vw, 100%);
        height: max(100vh, 100%);
        background-color: var(--bg-color);
        z-index: #{$zindex + 1};
        transition: all 250ms;
    }

    .popup {
        padding: var(--margin);
        border-radius: var(--margin);
        background-image: var(--bg-image);
        z-index: #{$zindex + 2};
        min-width: min(500px, 90vw);
        max-width: 95vw;
        min-height: min(200px, 80vh);
        max-height: 80vh;

        header {
            margin-top: 0;
            h3 { margin-top: 0; }
            button { margin: 0; margin-top: 5px; }
        }

        .popup-content {
            max-width: 100%;
            overflow: hidden;
        }
    }

    &.open {
        opacity: 1;
        left: 0;
    }
}
</style>