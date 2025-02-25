create table users (
    user_id uuid primary key default uuid_generate_v4(),
    created timestamp with time zone not null default current_timestamp,
    removed timestamp with time zone
);
