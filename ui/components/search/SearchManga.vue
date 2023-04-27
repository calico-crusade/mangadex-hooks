<template>
<div class="search fill-parent flex row scroll-y" @scroll="onScroll">
    <div class="scroll-header">
        <div class="control no-top">
            <div class="group">
                <input class="fill" v-model="search" placeholder="Search for a manga" @keyup.prevent="doSearch(search)" />
                <button @click="doSearchAct(search)">
                    <Icon>search</Icon>
                </button>
            </div>
        </div>
    </div>
    <div class="manga" v-for="m in manga">
        <a class="image" @click="$emit('manga', m)">
            <img :src="getCover(m)" />
        </a>
        <div class="content">
            <a class="title" @click="$emit('manga', m)">{{ preferEn(m.attributes.title) }}</a>
            <div class="tags in-line">
                <span style="display: none"></span>
                <span v-for="tag in m.attributes.tags">
                    {{ preferEn(tag.attributes.name) }}
                </span>
            </div>
            <div class="description">
                <Markdown :content="preferEn(m.attributes.description)" />
            </div>
        </div>
    </div>
    <Loading v-if="pending" />
</div>
</template>

<script setup lang="ts">
import { Localization, MdCoverArt, MdManga } from '~/utils/models';

interface Emits { (e: 'manga', manga: MdManga): void; }
defineEmits<Emits>();
const search = ref('');
const params = { search: '', size: 20 };
const paginator = new PaginationHelper<MdManga>(`md/manga`, params, 1, () => !!params.search);
const { data: manga, pending } = paginator;

const doSearchAct = async (input: string) => {
    if (!input || input.length < 3) return;
    params.search = search.value;
    paginator.refresh();
}

const getCover = (manga: MdManga) =>  mdApi.getCover(manga);
const preferEn = (dic: Localization) =>  dic['en'] ?? dic[Object.keys(dic)[0]] ?? '';
const doSearch = debounce(doSearchAct, 500);
const onScroll = (el: UIEvent) => paginator?.onScroll(<any>el.target);
</script>

<style lang="scss" scoped>
.manga {
    margin-top: 5px;
    display: flex;
    flex-flow: row;
    background-color: var(--bg-color-accent);
    border-radius: 5px;
    max-height: 300px;

    .image {
        margin: 10px;
        max-width: 150px;

        img {
            max-width: 100%;
            border-radius: 5px;
        }
    }

    .content {
        flex: 1;
        margin: 10px;
        overflow: hidden;
        .title { font-size: 20px; }
    }
}
</style>