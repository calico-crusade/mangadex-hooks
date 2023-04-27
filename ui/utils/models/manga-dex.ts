export type MdType = 'manga' | 'chapter' | 'author' | 'artist' | 'cover_art' | 'scanlation_group' | 'user' | 'tag' | 'member' | 'leader' | 'custom_list';
export type Localization = { [key: string]: string };
export type AllTypes = MdTag | MdManga | MdChapter | MdUser | MdGroup | MdCoverArt | MdCustomList;

export interface MdResult<T, TRels> {
    id: string;
    type: MdType;
    attributes: T;
    relationships: MdResult<TRels, AllTypes>[];
}

export interface MdTag extends MdResult<{
    name: Localization;
    description: Localization;
    group: string;
    version: number;
}, AllTypes> { type: 'tag'; }

interface MangaAttrs {
    title: Localization;
    altTitles: Localization[];
    description: Localization;
    isLocked: boolean;
    links: Localization;
    originalLanguage: string;
    lastVolume: string;
    lastChapter: string;
    publicationDemographic?: 'shounen' | 'shoujo' | 'josei' | 'seinen' | 'none';
    status?: 'ongoing' | 'completed' | 'hiatus' | 'cancelled';
    year?: number;
    contentRating?: 'safe' | 'suggestive' | 'erotica' | 'pornographic';
    tags: MdTag[];
    state?: string;
    chapterNumbersResetOnNewVolume: boolean;
    createdAt: Date;
    updatedAt: Date;
    version: number;
    availableTranslatedLanguages: string[];
    latestUploadedChapter: string;
}

export interface MdManga extends MdResult<MangaAttrs, MdManga | MdCoverArt | MdUser | MdTag> { type: 'manga'; }

export interface MdRelated extends MdResult<MangaAttrs, AllTypes> {
    type: 'manga';
    related: 'monochrome' |  'main_story' |  'adapted_from' |  'based_on' |  'prequel' |  'side_story' |  'doujinshi' |  'same_franchise' |  'shared_universe' |  'sequel' |  'spin_off' |  'alternate_story' |  'alternate_version' |  'preserialization' |  'colored' |  'serialization';
}

export interface MdChapter extends MdResult<{
    volume: string;
    chapter: string;
    title: string;
    translatedLanguage: string;
    externalUrl: string;
    publishAt: Date;
    readableAt: Date;
    createdAt: Date;
    updatedAt: Date;
    pages: number;
    version: number;
    uploader?: string;
}, MdManga | MdChapter | MdUser> { type: 'chapter'; }

export interface MdUser extends MdResult<{
    username: string;
    roles: string[];
    version: number;
}, AllTypes> { type: 'user' | 'leader' | 'member'; }

export interface MdGroup extends MdResult<{
    name: string;
    altNames: Localization[];
    locked: boolean;
    website?: string;
    ircServer?: string;
    ircChannel?: string;
    discord?: string;
    contactEmail?: string;
    description?: string;
    twitter?: string;
    mangaUpdates?: string;
    focusedLanguages: string[];
    official: boolean;
    verified: boolean;
    inactive: boolean;
    createdAt: Date;
    updatedAt: Date;
    version: number;
}, MdUser> { type: 'scanlation_group'; }

export interface MdCoverArt extends MdResult<{
    description: string;
    volume: string;
    fileName: string;
    locale: string;
    createdAt: Date;
    updatedAt: Date;
    version: number;
}, MdManga | MdUser> { type: 'cover_art'; }

export interface MdCustomList extends MdResult<{
    name: string;
    visibility: 'Public' | 'Private';
    version: number;
}, MdRelated> { type: 'custom_list'; }