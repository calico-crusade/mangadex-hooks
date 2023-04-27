export interface ApiResult {
    result: 'error' | 'ok';
    code: number;
}

export interface SuccessResult<T> extends ApiResult {
    data: T;
    result: 'ok';
}

export interface CollectionResult<T> extends SuccessResult<T[]> {
    pages: number;
    count: number;
}

export interface UpsertResult extends ApiResult {
    result: 'ok';
    id: number;
    resource: string;
}

export interface FailureResult extends ApiResult {
    result: 'error';
    errors: string[];
}

export interface MessageResult extends ApiResult {
    message?: string;
}