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