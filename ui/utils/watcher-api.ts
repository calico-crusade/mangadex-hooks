import { ApiResult, ResourceType, UpsertResult, Watcher } from "./models";

class WatcherApi {

    get(hook: number, page: number = 1, size: number = 20) {
        return new PaginationHelper<Watcher>(`webhook/${hook}/watchers`, { size }, page);
    }

    post(hook: number, id: string, type: ResourceType) {
        return api.post<UpsertResult>(`webhook/${hook}/watchers`, { id, type });
    }

    delete(hook: number, id: number) {
        return api.delete<ApiResult>(`webhook/${hook}/watchers/${id}`);
    }
}

export const watcherApi = new WatcherApi();