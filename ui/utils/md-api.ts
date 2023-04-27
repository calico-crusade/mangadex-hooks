import { MdCustomList, MdGroup, MdManga, MdUser, MdCoverArt } from "./models";

class MdApi {

    manga(search: string, page = 1, size = 20) {
        return api.getMany<MdManga>(`md/manga`, { search, page, size });
    }

    mangaById(id: string) {
        return api.getOne<MdManga>(`md/manga/${id}`);
    }

    group(search: string, page = 1, size = 20) {
        return api.getMany<MdGroup>(`md/group`, { search, page, size });
    }

    mangaPage(search: Ref<string>, page: number = 1, size: number = 20) {
        return new PaginationHelper<MdManga>(`md/manga`, { search, size }, page);
    }

    groupPage(search: string, page: number = 1, size: number = 20) {
        return new PaginationHelper<MdGroup>(`md/group`, { search, size }, page);
    }

    user(id: string) { return api.getOne<MdUser>(`md/user/${id}`); }
    
    list(id: string) { return api.getOne<MdCustomList>(`md/list/${id}`); }

    getCover(manga?: MdManga) {
        const DEFAULT = '~/assets/broken.png';
        if (!manga) return DEFAULT;
        const file = (<MdCoverArt | undefined>manga.relationships.find(t => t.type === 'cover_art'))?.attributes?.fileName;
        const id = manga.id;
        return file ? api.proxy(`https://mangadex.org/covers/${id}/${file}`, 'manga-covers') : DEFAULT;
    }
}

export const mdApi = new MdApi();