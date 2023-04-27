<template>
<div class="search fill-parent flex row scroll-y">
    <div class="scroll-header">
        <div class="control no-top">
            <div class="group">
                <input class="fill" v-model="id" placeholder="Custom List Id" />
                <button @click="doSearch()">
                    <Icon>search</Icon>
                </button>
            </div>
        </div>
    </div>
    <a class="custom-list" v-if="!pending && list" @click="$emit('list', list)">
        <div class="image">
            <img :src="cover" />
        </div>
        <div class="content">
            <div class="title">{{ list.attributes.name }}</div>
            <div class="id">ID: {{ list.id }}</div>
            <div class="count">Title Count: {{ list.relationships.length }}</div>
        </div>
    </a>
    <Loading v-if="pending" />
</div>
</template>

<script setup lang="ts">
import { MdCustomList } from '~/utils/models';
interface Emits { (e: 'list', list: MdCustomList): void; }
defineEmits<Emits>();
const id = ref('');
const pending = ref(false);
const list = ref<MdCustomList | undefined>(undefined);
const cover = ref('~/assets/broken.png');

const doSearch = async () => {
    pending.value = true;
    const { result } = await mdApi.list(id.value);
    if(!result.value) {
        pending.value = false;
        list.value = undefined;
        return;
    }

    const { result: manga } = await mdApi.mangaById(result.value.relationships[0].id);
    cover.value = mdApi.getCover(manga.value);
    list.value = result.value;
    pending.value = false;
};
</script>

<style lang="scss" scoped>
.custom-list {
    margin-top: 5px;
    display: flex;
    flex-flow: row;
    background-color: var(--bg-color-accent);
    border-radius: 5px;
    max-height: 300px;
    padding: var(--margin);

    .image {
        max-width: 100px;
        margin-right: var(--margin);
        img {
            max-width: 100%;
            max-height: 100%;
            border-radius: 5px;
        }
    }

    .content {
        flex: 1;
        margin: auto 5px;

        .title { font-weight: bold; }
        .id { color: var(--color-muted); }
    }
}
</style>