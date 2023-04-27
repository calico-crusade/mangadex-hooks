import { DbObject } from "./db.base";

export enum ResourceType {
    Manga = 1,
	User = 2,
	Group = 3,
	CustomList = 4,
}

export interface Watcher extends DbObject {
    itemId: string;
    type: ResourceType;
    webhookId: number;
    resourceName: string;
    resourceImage: string;
    cacheItems: string[];
    lastCacheCheck: Date;
}