
CREATE TABLE IF NOT EXISTS "sources" (
	"name"						TEXT NOT NULL PRIMARY KEY,
	"product_type"				TEXT,
	"market_hyphenated"			INTEGER,
	"is_active"					INTEGER NOT NULL DEFAULT 0,
	UNIQUE("name")
);

CREATE TABLE IF NOT EXISTS "source_credentials" (
	"id"		INTEGER NOT NULL PRIMARY KEY,
	"source"	TEXT NOT NULL,
	"name"		TEXT NOT NULL,	
	"value"		TEXT,
	UNIQUE("source", "name"),
	FOREIGN KEY("source") REFERENCES "sources"("name")
		ON DELETE RESTRICT ON UPDATE RESTRICT
);

CREATE TABLE IF NOT EXISTS "product_ids" (
	"id" INTEGER PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS "accounts" (
	"id"			INTEGER NOT NULL PRIMARY KEY,
	"source"		TEXT NOT NULL, 
	"asset"			TEXT NOT NULL,
	"external_id"	TEXT NOT NULL,
	"is_active"		INTEGER NOT NULL DEFAULT 0,
	UNIQUE("source", "asset")
);
CREATE TRIGGER IF NOT EXISTS "set_account_id_after_insert" AFTER INSERT ON "accounts"
BEGIN
	INSERT INTO product_ids values (null);

	UPDATE accounts
	SET id = (SELECT last_insert_rowid())
	WHERE
		source = NEW.source
		AND asset = NEW.asset;
END;

CREATE TABLE IF NOT EXISTS "markets" (
	"id"			INTEGER NOT NULL PRIMARY KEY,
	"source"		TEXT NOT NULL, 
	"base"			TEXT NOT NULL,
	"quote"			TEXT NOT NULL,
	"is_active"		INTEGER NOT NULL DEFAULT 0,
	UNIQUE("source", "base", "quote")
);
CREATE TRIGGER IF NOT EXISTS "set_market_id_after_insert" AFTER INSERT ON "markets"
BEGIN
	INSERT INTO product_ids values (null);

	UPDATE markets
	SET id = (SELECT last_insert_rowid())
	WHERE
		source = NEW.source
		AND base = NEW.base
		AND quote = NEW.quote;
END;

CREATE TABLE IF NOT EXISTS "coins" (
	"id"		TEXT NOT NULL PRIMARY KEY,
	"name"		TEXT NULL,
	"symbol"	TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS "coin_lookup" (
	"asset"		TEXT NOT NULL PRIMARY KEY,
	"coin_id"	TEXT NOT NULL,
	FOREIGN KEY("coin_id") REFERENCES "coins"("id")
		ON DELETE RESTRICT ON UPDATE RESTRICT
);

CREATE TABLE IF NOT EXISTS "transaction_types" (
	"name"			TEXT NOT NULL PRIMARY KEY,
	"is_incoming"	INTEGER NOT NULL DEFAULT 0,
	"is_outgoing"	INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS "batches" (
	"id"	INTEGER NOT NULL PRIMARY KEY,
	"type"	TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS "transactions" (
	"id"					INTEGER NOT NULL PRIMARY KEY,
	"asset"	                TEXT NOT NULL,
	"quantity"	            TEXT NOT NULL,
	"unit_value"	        TEXT,
	"unit_value_timestamp"	INTEGER,
	"type"					TEXT NOT NULL,
	"timestamp"	            INTEGER NOT NULL,
	"source"				TEXT,
	"external_id"	        TEXT,
	"batch_id"				INTEGER,
	FOREIGN KEY("type") REFERENCES "transaction_types"("name")
		ON DELETE RESTRICT ON UPDATE RESTRICT,
	FOREIGN KEY("batch_id") REFERENCES "batches"("id")
		ON DELETE RESTRICT ON UPDATE RESTRICT
);

CREATE TABLE IF NOT EXISTS "app_settings" (
	"key"	TEXT NOT NULL PRIMARY KEY,
	"value"	TEXT
);
