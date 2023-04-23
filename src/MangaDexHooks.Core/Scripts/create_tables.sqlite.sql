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