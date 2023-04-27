CREATE TABLE IF NOT EXISTS user_profiles (
	id BIGSERIAL PRIMARY KEY,

    username TEXT NOT NULL,
    avatar TEXT NOT NULL,
    platform_id TEXT NOT NULL,
    admin BOOLEAN NOT NULL DEFAULT FALSE,
    email TEXT NOT NULL,
    provider TEXT NULL,
    provider_id TEXT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP,

    UNIQUE(platform_id)
);


CREATE TABLE IF NOT EXISTS webhooks (
    id BIGSERIAL PRIMARY KEY,
    
    name TEXT NOT NULL,
    owner_id BIGINT NOT NULL REFERENCES user_profiles(id),
    url TEXT NOT NULL,
    type INT NOT NULL,
    discord_data TEXT NULL,
    discord_script TEXT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP,

    UNIQUE(owner_id, name)
);

CREATE TABLE IF NOT EXISTS webhook_results (
    id BIGSERIAL PRIMARY KEY,
    
    webhook_id BIGINT NOT NULL REFERENCES webhooks(id),
    results TEXT NULL,
    code INT NOT NULL DEFAULT 200,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS mangadex_cache (
    id BIGSERIAL PRIMARY KEY,
    
    resource_id TEXT NOT NULL,
    resource_type INT NOT NULL,
    results TEXT NOT NULL,
    last_check TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP,

    UNIQUE(resource_id, resource_type)
);

CREATE TABLE IF NOT EXISTS watchers (
    id BIGSERIAL PRIMARY KEY,
    
    item_id TEXT NOT NULL,
    watch_type INT NOT NULL,
    webhook_id BIGINT NOT NULL REFERENCES webhooks(id),
    resource_image TEXT NOT NULL,
    resource_name TEXT NOT NULL,
    cache_items TEXT[] NOT NULL DEFAULT '{}',
    last_cache_check TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP,

    UNIQUE(item_id, watch_type, webhook_id)
);