<template>
    <ClientOnly>
        <!-- <MonacoEditor class="editor" v-model="code" @load="setEditor" lang="javascript" :options="{ theme: 'vs-dark' }" /> -->
    </ClientOnly>
</template>

<script setup lang="ts">
import * as Monaco from 'monaco-editor'

type Editor = Monaco.editor.IStandaloneCodeEditor;

interface Props {
    modelValue: string;
    typings?: string;
}

interface Emits {
    (e: 'update:modelValue', value: string): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();
let editor: Editor | undefined = undefined;
let library: Monaco.IDisposable | undefined = undefined;
let model: Monaco.editor.ITextModel | undefined = undefined;
const typingUri = 'ts:runner/runner.d.ts';

const code = computed({
    get() { return props.modelValue; },
    set(value: string) {
        emit('update:modelValue', value);
    }
});

const resolveTypings = () => {
    // console.log('Typings hit');
    // library?.dispose();
    // model?.dispose();

    // if (!editor || !props.typings) return;
    // console.log('Typings');
    // library = Monaco.languages.typescript.javascriptDefaults.addExtraLib(props.typings, typingUri);
    // model = Monaco.editor.createModel(props.typings, 'typescript', Monaco.Uri.parse(typingUri));
    // console.log('Typings Set');
};

const setEditor = (e: Editor) => {
    editor = e;
    resolveTypings();
};

watch(() => props.typings, () => resolveTypings());
</script>