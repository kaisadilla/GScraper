namespace Kaisa.GScraper.SQLite {
    public static class TableDefinitions {
        public const string SQL_MOVIES =
                   @"create table if not exists movies (
                        internalName text primary key,
                        displayName text,
                        date text,
                        imgPath text,
                        favorite int,
                        rating real
                    );";
        public const string SQL_SERIES =
                   @"create table if not exists series (
                        internalName text primary key,
                        displayName text,
                        imgPath text,
                        favorite int,
                        rating real
                    );";
        public const string SQL_EPISODES =
                   @"create table if not exists episodes (
                        fullId text primary key,
                        series text,
                        seasonIndex int,
                        episodeIndex int,
                        name text,
                        date text,
                        imgPath text,
                        rating real,
                        foreign key(series) references series(internalName) on delete cascade
                    );";
        public const string SQL_LINKS =
                   @"create table if not exists links (
                        url text primary key,
                        episode text,
                        linkType int,
                        hostName text,
                        quality text,
                        language text,
                        favorite int,
                        badlink int,
                        foreign key(episode) references episodes(fullId) on delete cascade
                    );";
    }
}
