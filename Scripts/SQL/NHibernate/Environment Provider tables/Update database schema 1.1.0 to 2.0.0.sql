-- Note that the commented SQL statements are not to be executed. They are
-- provided for reference only as it is not possible to simply change a
-- uniqueidentifier column to bigint. Refer to the link below for the an
-- approach to do so.

--alter table ENVIRONMENT drop constraint FK???;
--alter table ENVIRONMENT alter column ZONE_ID bigint;
--alter table ENVIRONMENT add constraint FK_ENVIRONMENT_ZONE foreign key (ZONE_ID) references [ZONE] (ZONE_ID);

alter table ENVIRONMENT_REGISTER add ZONE_ID bigint;
--alter table ENVIRONMENT_REGISTER add constraint FK_ENVIRONMENT_REGISTER_ZONE foreign key (ZONE_ID) references [ZONE] (ZONE_ID);

-- To change the primary key from a GUID to an integer for the ZONE table,
-- refer to the following article.
--
-- http://stackoverflow.com/questions/2730305/approach-for-altering-primary-key-from-guid-to-bigint-in-sql-server-related-tabl
--
--alter table [ZONE] drop constraint PK???;
--alter table [ZONE] alter column ZONE_ID bigint identity not null;
--alter table [ZONE] add constraint PK_ZONE primary key (ZONE_ID);

alter table [ZONE] add SIF_ID nvarchar(64);

--alter table ZONE_PROPERTIES drop constraint PK???;
--alter table ZONE_PROPERTIES alter column ZONE_ID bigint not null;
--alter table ZONE_PROPERTIES add constraint PK_ZONE_PROPERTIES primary key (ZONE_ID, NAME);
