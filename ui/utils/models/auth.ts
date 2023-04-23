export interface UserResult {
    roles: string[];
    nickname: string;
    avatar: string;
    platformId: string;
    provider: string;
    providerId: string;
    profileId: number;
}

export interface LoginResult {
    user: UserResult;
    token: string;
}