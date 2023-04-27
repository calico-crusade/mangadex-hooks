<template>
<Popup :title="title ?? 'Search for something'" v-model="open" @closed="() => open = false">
    <div class="tab-popup">
        <Tabs>
            <Tab title="Manga" icon="menu_book" keep-alive>
                <SearchManga @manga="foundManga" />
            </Tab>
            <Tab title="Scanlation Group" icon="group" keep-alive>
                <SearchGroup @group="foundGroup" />
            </Tab>
            <Tab title="Custom List" icon="receipt_long" keep-alive>
                <SearchList @list="foundList" />
            </Tab>
            <Tab title="User" icon="person" keep-alive>
                <SearchUser @user="foundUser" />
            </Tab>
        </Tabs>
    </div>
</Popup>
</template>

<script setup lang="ts">
import { MdCustomList, MdGroup, MdManga, MdUser } from '~/utils/models';

interface Emits {
    (e: 'manga', m: MdManga): void;
    (e: 'group', g: MdGroup): void;
    (e: 'list', l: MdCustomList): void;
    (e: 'user', u: MdUser): void;
    (e: 'closed'): void;
    (e: 'update:modelValue', open: boolean): void;
}

interface Props {
    title?: string;
    modelValue: boolean;
}

const props = defineProps<Props>();
const emits = defineEmits<Emits>();
const open = computed({
    get() { return props.modelValue; },
    set(value: boolean) { 
        emits('update:modelValue', value);
        if (!value) emits('closed');
    }
});

const foundManga = (m: MdManga) => { emits('manga', m); open.value = false; }
const foundGroup = (g: MdGroup) => { emits('group', g); open.value = false; }
const foundList = (l: MdCustomList) => { emits('list', l); open.value = false; }
const foundUser = (u: MdUser) => { emits('user', u); open.value = false; }
</script>

<style lang="scss" scoped>
.tab-popup {
    max-width: 90vw;
    min-width: 90vw;
    max-height: 75vh;
    min-height: 75vh;
    flex: 1;
    position: relative;
    display: flex;
    flex-flow: row;
    margin: 0;
    padding: 0;
}
</style>