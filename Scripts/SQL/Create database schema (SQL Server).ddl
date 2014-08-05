
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC71F9BACE16D310A]') AND parent_object_id = OBJECT_ID('ENVIRONMENT'))
alter table ENVIRONMENT  drop constraint FKC71F9BACE16D310A


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC71F9BACD1CA0358]') AND parent_object_id = OBJECT_ID('ENVIRONMENT'))
alter table ENVIRONMENT  drop constraint FKC71F9BACD1CA0358


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB343704D722350EE]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_INFRASTRUCTURE_SERVICES'))
alter table ENVIRONMENT_INFRASTRUCTURE_SERVICES  drop constraint FKB343704D722350EE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB343704DE1C1FDE4]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_INFRASTRUCTURE_SERVICES'))
alter table ENVIRONMENT_INFRASTRUCTURE_SERVICES  drop constraint FKB343704DE1C1FDE4


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD922ED44A5E6A335]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_PROVISIONED_ZONES'))
alter table ENVIRONMENT_PROVISIONED_ZONES  drop constraint FKD922ED44A5E6A335


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD922ED44E1C1FDE4]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_PROVISIONED_ZONES'))
alter table ENVIRONMENT_PROVISIONED_ZONES  drop constraint FKD922ED44E1C1FDE4


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC68E304FAA0F47F3]') AND parent_object_id = OBJECT_ID('APPLICATION_INFO'))
alter table APPLICATION_INFO  drop constraint FKC68E304FAA0F47F3


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC68E304FE329EE49]') AND parent_object_id = OBJECT_ID('APPLICATION_INFO'))
alter table APPLICATION_INFO  drop constraint FKC68E304FE329EE49


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKDD117004722350EE]') AND parent_object_id = OBJECT_ID('ZONE_PROPERTIES'))
alter table ZONE_PROPERTIES  drop constraint FKDD117004722350EE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKDD117004D1CA0358]') AND parent_object_id = OBJECT_ID('ZONE_PROPERTIES'))
alter table ZONE_PROPERTIES  drop constraint FKDD117004D1CA0358


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKDF16D6B621874850]') AND parent_object_id = OBJECT_ID('SERVICE_RIGHTS'))
alter table SERVICE_RIGHTS  drop constraint FKDF16D6B621874850


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKDF16D6B642A065CE]') AND parent_object_id = OBJECT_ID('SERVICE_RIGHTS'))
alter table SERVICE_RIGHTS  drop constraint FKDF16D6B642A065CE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA5FFA4EE42A065CE]') AND parent_object_id = OBJECT_ID('PROVISIONED_ZONE_SERVICES'))
alter table PROVISIONED_ZONE_SERVICES  drop constraint FKA5FFA4EE42A065CE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA5FFA4EEA5E6A335]') AND parent_object_id = OBJECT_ID('PROVISIONED_ZONE_SERVICES'))
alter table PROVISIONED_ZONE_SERVICES  drop constraint FKA5FFA4EEA5E6A335


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK84C906C83C59E2E3]') AND parent_object_id = OBJECT_ID('APPLICATION_ENVIRONMENT_REGISTERS'))
alter table APPLICATION_ENVIRONMENT_REGISTERS  drop constraint FK84C906C83C59E2E3


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK84C906C87E6CCFAE]') AND parent_object_id = OBJECT_ID('APPLICATION_ENVIRONMENT_REGISTERS'))
alter table APPLICATION_ENVIRONMENT_REGISTERS  drop constraint FK84C906C87E6CCFAE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK86CD31CA722350EE]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES'))
alter table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES  drop constraint FK86CD31CA722350EE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK86CD31CA3C59E2E3]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES'))
alter table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES  drop constraint FK86CD31CA3C59E2E3


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK62F82A66A5E6A335]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_REGISTER_PROVISIONED_ZONES'))
alter table ENVIRONMENT_REGISTER_PROVISIONED_ZONES  drop constraint FK62F82A66A5E6A335


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK62F82A663C59E2E3]') AND parent_object_id = OBJECT_ID('ENVIRONMENT_REGISTER_PROVISIONED_ZONES'))
alter table ENVIRONMENT_REGISTER_PROVISIONED_ZONES  drop constraint FK62F82A663C59E2E3


    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT

    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT_INFRASTRUCTURE_SERVICES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT_INFRASTRUCTURE_SERVICES

    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT_PROVISIONED_ZONES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT_PROVISIONED_ZONES

    if exists (select * from dbo.sysobjects where id = object_id(N'APPLICATION_INFO') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table APPLICATION_INFO

    if exists (select * from dbo.sysobjects where id = object_id(N'PRODUCT_IDENTITY') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PRODUCT_IDENTITY

    if exists (select * from dbo.sysobjects where id = object_id(N'PROPERTY') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PROPERTY

    if exists (select * from dbo.sysobjects where id = object_id(N'[ZONE]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [ZONE]

    if exists (select * from dbo.sysobjects where id = object_id(N'ZONE_PROPERTIES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ZONE_PROPERTIES

    if exists (select * from dbo.sysobjects where id = object_id(N'[RIGHT]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [RIGHT]

    if exists (select * from dbo.sysobjects where id = object_id(N'SERVICE') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table SERVICE

    if exists (select * from dbo.sysobjects where id = object_id(N'SERVICE_RIGHTS') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table SERVICE_RIGHTS

    if exists (select * from dbo.sysobjects where id = object_id(N'PROVISIONED_ZONE') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PROVISIONED_ZONE

    if exists (select * from dbo.sysobjects where id = object_id(N'PROVISIONED_ZONE_SERVICES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PROVISIONED_ZONE_SERVICES

    if exists (select * from dbo.sysobjects where id = object_id(N'APPLICATION_REGISTER') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table APPLICATION_REGISTER

    if exists (select * from dbo.sysobjects where id = object_id(N'APPLICATION_ENVIRONMENT_REGISTERS') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table APPLICATION_ENVIRONMENT_REGISTERS

    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT_REGISTER') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT_REGISTER

    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES

    if exists (select * from dbo.sysobjects where id = object_id(N'ENVIRONMENT_REGISTER_PROVISIONED_ZONES') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table ENVIRONMENT_REGISTER_PROVISIONED_ZONES

    create table ENVIRONMENT (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       AUTHENTICATION_METHOD NVARCHAR(16) null,
       CONSUMER_NAME NVARCHAR(256) null,
       INSTANCE_ID NVARCHAR(256) null,
       SESSION_TOKEN NVARCHAR(64) null,
       SOLUTION_ID NVARCHAR(256) null,
       TYPE INT null,
       USER_TOKEN NVARCHAR(256) null,
       APPLICATION_INFO_ID BIGINT null unique,
       ZONE_ID UNIQUEIDENTIFIER null,
       primary key (ENVIRONMENT_ID)
    )

    create table ENVIRONMENT_INFRASTRUCTURE_SERVICES (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       PROPERTY_ID BIGINT not null,
       NAME NVARCHAR(255) not null,
       primary key (ENVIRONMENT_ID, NAME)
    )

    create table ENVIRONMENT_PROVISIONED_ZONES (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       PROVISIONED_ZONE_ID BIGINT not null,
       SIF_ID NVARCHAR(255) not null,
       primary key (ENVIRONMENT_ID, SIF_ID)
    )

    create table APPLICATION_INFO (
        APPLICATION_INFO_ID BIGINT IDENTITY NOT NULL,
       APPLICATION_KEY NVARCHAR(256) null,
       SUPPORTED_INFRASTRUCTURE_VERSION NVARCHAR(16) null,
       SUPPORTED_DATA_MODEL NVARCHAR(256) null,
       SUPPORTED_DATA_MODEL_VERSION NVARCHAR(256) null,
       TRANSPORT NVARCHAR(8) null,
       APPLICATION_PRODUCT_ID BIGINT null unique,
       ADAPTER_PRODUCT_ID BIGINT null unique,
       primary key (APPLICATION_INFO_ID)
    )

    create table PRODUCT_IDENTITY (
        PRODUCT_IDENTITY_ID BIGINT IDENTITY NOT NULL,
       VENDOR_NAME NVARCHAR(256) null,
       PRODUCT_NAME NVARCHAR(256) null,
       PRODUCT_VERSION NVARCHAR(80) null,
       ICON_URI NVARCHAR(256) null,
       primary key (PRODUCT_IDENTITY_ID)
    )

    create table PROPERTY (
        PROPERTY_ID BIGINT IDENTITY NOT NULL,
       NAME NVARCHAR(80) null,
       [VALUE] NVARCHAR(256) null,
       primary key (PROPERTY_ID)
    )

    create table [ZONE] (
        ZONE_ID UNIQUEIDENTIFIER not null,
       DESCRIPTION NVARCHAR(256) null,
       primary key (ZONE_ID)
    )

    create table ZONE_PROPERTIES (
        ZONE_ID UNIQUEIDENTIFIER not null,
       PROPERTY_ID BIGINT not null,
       NAME NVARCHAR(255) not null,
       primary key (ZONE_ID, NAME)
    )

    create table [RIGHT] (
        RIGHT_ID BIGINT IDENTITY NOT NULL,
       TYPE INT null,
       [VALUE] INT null,
       primary key (RIGHT_ID)
    )

    create table SERVICE (
        SERVICE_ID BIGINT IDENTITY NOT NULL,
       CONTEXTID NVARCHAR(128) null,
       NAME NVARCHAR(128) null,
       TYPE INT null,
       primary key (SERVICE_ID)
    )

    create table SERVICE_RIGHTS (
        SERVICE_ID BIGINT not null,
       RIGHT_ID BIGINT not null,
       TYPE INT not null,
       primary key (SERVICE_ID, TYPE)
    )

    create table PROVISIONED_ZONE (
        PROVISIONED_ZONE_ID BIGINT IDENTITY NOT NULL,
       SIF_ID NVARCHAR(32) null,
       primary key (PROVISIONED_ZONE_ID)
    )

    create table PROVISIONED_ZONE_SERVICES (
        PROVISIONED_ZONE_ID BIGINT not null,
       SERVICE_ID BIGINT not null,
       primary key (PROVISIONED_ZONE_ID, SERVICE_ID)
    )

    create table APPLICATION_REGISTER (
        APPLICATION_REGISTER_ID BIGINT IDENTITY NOT NULL,
       APPLICATION_KEY NVARCHAR(256) null,
       SHARED_SECRET NVARCHAR(256) null,
       primary key (APPLICATION_REGISTER_ID)
    )

    create table APPLICATION_ENVIRONMENT_REGISTERS (
        APPLICATION_REGISTER_ID BIGINT not null,
       ENVIRONMENT_REGISTER_ID BIGINT not null,
       primary key (APPLICATION_REGISTER_ID, ENVIRONMENT_REGISTER_ID)
    )

    create table ENVIRONMENT_REGISTER (
        ENVIRONMENT_REGISTER_ID BIGINT IDENTITY NOT NULL,
       APPLICATION_KEY NVARCHAR(256) null,
       INSTANCE_ID NVARCHAR(256) null,
       SOLUTION_ID NVARCHAR(256) null,
       USER_TOKEN NVARCHAR(256) null,
       primary key (ENVIRONMENT_REGISTER_ID)
    )

    create table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES (
        ENVIRONMENT_REGISTER_ID BIGINT not null,
       PROPERTY_ID BIGINT not null,
       NAME NVARCHAR(255) not null,
       primary key (ENVIRONMENT_REGISTER_ID, NAME)
    )

    create table ENVIRONMENT_REGISTER_PROVISIONED_ZONES (
        ENVIRONMENT_REGISTER_ID BIGINT not null,
       PROVISIONED_ZONE_ID BIGINT not null,
       SIF_ID NVARCHAR(255) not null,
       primary key (ENVIRONMENT_REGISTER_ID, SIF_ID)
    )

    alter table ENVIRONMENT 
        add constraint FKC71F9BACE16D310A 
        foreign key (APPLICATION_INFO_ID) 
        references APPLICATION_INFO

    alter table ENVIRONMENT 
        add constraint FKC71F9BACD1CA0358 
        foreign key (ZONE_ID) 
        references [ZONE]

    alter table ENVIRONMENT_INFRASTRUCTURE_SERVICES 
        add constraint FKB343704D722350EE 
        foreign key (PROPERTY_ID) 
        references PROPERTY

    alter table ENVIRONMENT_INFRASTRUCTURE_SERVICES 
        add constraint FKB343704DE1C1FDE4 
        foreign key (ENVIRONMENT_ID) 
        references ENVIRONMENT

    alter table ENVIRONMENT_PROVISIONED_ZONES 
        add constraint FKD922ED44A5E6A335 
        foreign key (PROVISIONED_ZONE_ID) 
        references PROVISIONED_ZONE

    alter table ENVIRONMENT_PROVISIONED_ZONES 
        add constraint FKD922ED44E1C1FDE4 
        foreign key (ENVIRONMENT_ID) 
        references ENVIRONMENT

    alter table APPLICATION_INFO 
        add constraint FKC68E304FAA0F47F3 
        foreign key (APPLICATION_PRODUCT_ID) 
        references PRODUCT_IDENTITY

    alter table APPLICATION_INFO 
        add constraint FKC68E304FE329EE49 
        foreign key (ADAPTER_PRODUCT_ID) 
        references PRODUCT_IDENTITY

    alter table ZONE_PROPERTIES 
        add constraint FKDD117004722350EE 
        foreign key (PROPERTY_ID) 
        references PROPERTY

    alter table ZONE_PROPERTIES 
        add constraint FKDD117004D1CA0358 
        foreign key (ZONE_ID) 
        references [ZONE]

    alter table SERVICE_RIGHTS 
        add constraint FKDF16D6B621874850 
        foreign key (RIGHT_ID) 
        references [RIGHT]

    alter table SERVICE_RIGHTS 
        add constraint FKDF16D6B642A065CE 
        foreign key (SERVICE_ID) 
        references SERVICE

    alter table PROVISIONED_ZONE_SERVICES 
        add constraint FKA5FFA4EE42A065CE 
        foreign key (SERVICE_ID) 
        references SERVICE

    alter table PROVISIONED_ZONE_SERVICES 
        add constraint FKA5FFA4EEA5E6A335 
        foreign key (PROVISIONED_ZONE_ID) 
        references PROVISIONED_ZONE

    alter table APPLICATION_ENVIRONMENT_REGISTERS 
        add constraint FK84C906C83C59E2E3 
        foreign key (ENVIRONMENT_REGISTER_ID) 
        references ENVIRONMENT_REGISTER

    alter table APPLICATION_ENVIRONMENT_REGISTERS 
        add constraint FK84C906C87E6CCFAE 
        foreign key (APPLICATION_REGISTER_ID) 
        references APPLICATION_REGISTER

    alter table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES 
        add constraint FK86CD31CA722350EE 
        foreign key (PROPERTY_ID) 
        references PROPERTY

    alter table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES 
        add constraint FK86CD31CA3C59E2E3 
        foreign key (ENVIRONMENT_REGISTER_ID) 
        references ENVIRONMENT_REGISTER

    alter table ENVIRONMENT_REGISTER_PROVISIONED_ZONES 
        add constraint FK62F82A66A5E6A335 
        foreign key (PROVISIONED_ZONE_ID) 
        references PROVISIONED_ZONE

    alter table ENVIRONMENT_REGISTER_PROVISIONED_ZONES 
        add constraint FK62F82A663C59E2E3 
        foreign key (ENVIRONMENT_REGISTER_ID) 
        references ENVIRONMENT_REGISTER
