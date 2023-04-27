CREATE TABLE IF NOT EXISTS user_profiles (
	id INTEGER PRIMARY KEY,

	username TEXT NOT NULL,
    avatar TEXT NOT NULL,
    platform_id TEXT NOT NULL,
    admin INTEGER NOT NULL DEFAULT 0,
    email TEXT NOT NULL,
    provider TEXT NULL,
    provider_id TEXT NULL,

    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TEXT,

    UNIQUE(platform_id)
);

CREATE TABLE IF NOT EXISTS webhooks (
    id INTEGER PRIMARY KEY,

    name TEXT NOT NULL,
    owner_id INTEGER NOT NULL REFERENCES user_profiles(id),
    url TEXT NOT NULL,
    type INTEGER NOT NULL,
    discord_data TEXT NULL,
    discord_script TEXT NULL,

    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TEXT,

    UNIQUE(owner_id, name)
);

CREATE TABLE IF NOT EXISTS webhook_results (
    id INTEGER PRIMARY KEY,

    webhook_id INTEGER NOT NULL REFERENCES webhooks(id),
    results TEXT NULL,
    code INTEGER NOT NULL DEFAULT 200,

    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TEXT
);

CREATE TABLE IF NOT EXISTS mangadex_cache (
    id INTEGER PRIMARY KEY,
    
    resource_id TEXT NOT NULL,
    resource_type INTEGER NOT NULL,
    results TEXT NOT NULL,
    last_check TEXT NULL DEFAULT CURRENT_TIMESTAMP,

    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TEXT

    UNIQUE(resource_id, resource_type)
);

CREATE TABLE IF NOT EXISTS watchers (
    id INTEGER PRIMARY KEY,
    
    item_id TEXT NOT NULL,
    watch_type INTEGER NOT NULL,
    webhook_id INTEGER NOT NULL REFERENCES webhooks(id),
    resource_image TEXT NOT NULL,
    resource_name TEXT NOT NULL,
    cache_items TEXT NOT NULL DEFAULT '[]',
    last_cache_check TEXT NULL DEFAULT CURRENT_TIMESTAMP,

    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TEXT,

    UNIQUE(item_id, watch_type, webhook_id)
);