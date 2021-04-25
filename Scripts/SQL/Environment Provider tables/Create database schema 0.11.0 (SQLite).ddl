
    PRAGMA foreign_keys = OFF

    drop table if exists ENVIRONMENT

    drop table if exists ENVIRONMENT_INFRASTRUCTURE_SERVICES

    drop table if exists ENVIRONMENT_PROVISIONED_ZONES

    drop table if exists APPLICATION_INFO

    drop table if exists PRODUCT_IDENTITY

    drop table if exists PROPERTY

    drop table if exists ZONE

    drop table if exists ZONE_PROPERTIES

    drop table if exists RIGHT

    drop table if exists SERVICE

    drop table if exists SERVICE_RIGHTS

    drop table if exists PROVISIONED_ZONE

    drop table if exists PROVISIONED_ZONE_SERVICES

    drop table if exists APPLICATION_REGISTER

    drop table if exists APPLICATION_ENVIRONMENT_REGISTERS

    drop table if exists ENVIRONMENT_REGISTER

    drop table if exists ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES

    drop table if exists ENVIRONMENT_REGISTER_PROVISIONED_ZONES

    PRAGMA foreign_keys = ON

    create table ENVIRONMENT (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       AUTHENTICATION_METHOD TEXT,
       CONSUMER_NAME TEXT,
       INSTANCE_ID TEXT,
       SESSION_TOKEN TEXT,
       SOLUTION_ID TEXT,
       TYPE INT,
       USER_TOKEN TEXT,
       APPLICATION_INFO_ID BIGINT unique,
       ZONE_ID UNIQUEIDENTIFIER,
       primary key (ENVIRONMENT_ID),
       constraint FKC71F9BACE16D310A foreign key (APPLICATION_INFO_ID) references APPLICATION_INFO,
       constraint FKC71F9BACD1CA0358 foreign key (ZONE_ID) references ZONE
    )

    create table ENVIRONMENT_INFRASTRUCTURE_SERVICES (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       PROPERTY_ID BIGINT not null,
       NAME TEXT not null,
       primary key (ENVIRONMENT_ID, NAME),
       constraint FKB343704D722350EE foreign key (PROPERTY_ID) references PROPERTY,
       constraint FKB343704DE1C1FDE4 foreign key (ENVIRONMENT_ID) references ENVIRONMENT
    )

    create table ENVIRONMENT_PROVISIONED_ZONES (
        ENVIRONMENT_ID UNIQUEIDENTIFIER not null,
       PROVISIONED_ZONE_ID BIGINT not null,
       SIF_ID TEXT not null,
       primary key (ENVIRONMENT_ID, SIF_ID),
       constraint FKD922ED44A5E6A335 foreign key (PROVISIONED_ZONE_ID) references PROVISIONED_ZONE,
       constraint FKD922ED44E1C1FDE4 foreign key (ENVIRONMENT_ID) references ENVIRONMENT
    )

    create table APPLICATION_INFO (
        APPLICATION_INFO_ID  integer primary key autoincrement,
       APPLICATION_KEY TEXT,
       SUPPORTED_INFRASTRUCTURE_VERSION TEXT,
       SUPPORTED_DATA_MODEL TEXT,
       SUPPORTED_DATA_MODEL_VERSION TEXT,
       TRANSPORT TEXT,
       APPLICATION_PRODUCT_ID BIGINT unique,
       ADAPTER_PRODUCT_ID BIGINT unique,
       constraint FKC68E304FAA0F47F3 foreign key (APPLICATION_PRODUCT_ID) references PRODUCT_IDENTITY,
       constraint FKC68E304FE329EE49 foreign key (ADAPTER_PRODUCT_ID) references PRODUCT_IDENTITY
    )

    create table PRODUCT_IDENTITY (
        PRODUCT_IDENTITY_ID  integer primary key autoincrement,
       VENDOR_NAME TEXT,
       PRODUCT_NAME TEXT,
       PRODUCT_VERSION TEXT,
       ICON_URI TEXT
    )

    create table PROPERTY (
        PROPERTY_ID  integer primary key autoincrement,
       NAME TEXT,
       VALUE TEXT
    )

    create table ZONE (
        ZONE_ID UNIQUEIDENTIFIER not null,
       DESCRIPTION TEXT,
       primary key (ZONE_ID)
    )

    create table ZONE_PROPERTIES (
        ZONE_ID UNIQUEIDENTIFIER not null,
       PROPERTY_ID BIGINT not null,
       NAME TEXT not null,
       primary key (ZONE_ID, NAME),
       constraint FKDD117004722350EE foreign key (PROPERTY_ID) references PROPERTY,
       constraint FKDD117004D1CA0358 foreign key (ZONE_ID) references ZONE
    )

    create table RIGHT (
        RIGHT_ID  integer primary key autoincrement,
       TYPE INT,
       VALUE INT
    )

    create table SERVICE (
        SERVICE_ID  integer primary key autoincrement,
       CONTEXTID TEXT,
       NAME TEXT,
       TYPE INT
    )

    create table SERVICE_RIGHTS (
        SERVICE_ID BIGINT not null,
       RIGHT_ID BIGINT not null,
       TYPE INT not null,
       primary key (SERVICE_ID, TYPE),
       constraint FKDF16D6B621874850 foreign key (RIGHT_ID) references RIGHT,
       constraint FKDF16D6B642A065CE foreign key (SERVICE_ID) references SERVICE
    )

    create table PROVISIONED_ZONE (
        PROVISIONED_ZONE_ID  integer primary key autoincrement,
       SIF_ID TEXT
    )

    create table PROVISIONED_ZONE_SERVICES (
        PROVISIONED_ZONE_ID BIGINT not null,
       SERVICE_ID BIGINT not null,
       primary key (PROVISIONED_ZONE_ID, SERVICE_ID),
       constraint FKA5FFA4EE42A065CE foreign key (SERVICE_ID) references SERVICE,
       constraint FKA5FFA4EEA5E6A335 foreign key (PROVISIONED_ZONE_ID) references PROVISIONED_ZONE
    )

    create table APPLICATION_REGISTER (
        APPLICATION_REGISTER_ID  integer primary key autoincrement,
       APPLICATION_KEY TEXT,
       SHARED_SECRET TEXT
    )

    create table APPLICATION_ENVIRONMENT_REGISTERS (
        APPLICATION_REGISTER_ID BIGINT not null,
       ENVIRONMENT_REGISTER_ID BIGINT not null,
       primary key (APPLICATION_REGISTER_ID, ENVIRONMENT_REGISTER_ID),
       constraint FK84C906C83C59E2E3 foreign key (ENVIRONMENT_REGISTER_ID) references ENVIRONMENT_REGISTER,
       constraint FK84C906C87E6CCFAE foreign key (APPLICATION_REGISTER_ID) references APPLICATION_REGISTER
    )

    create table ENVIRONMENT_REGISTER (
        ENVIRONMENT_REGISTER_ID  integer primary key autoincrement,
       APPLICATION_KEY TEXT,
       INSTANCE_ID TEXT,
       SOLUTION_ID TEXT,
       USER_TOKEN TEXT
    )

    create table ENVIRONMENT_REGISTER_INFRASTRUCTURE_SERVICES (
        ENVIRONMENT_REGISTER_ID BIGINT not null,
       PROPERTY_ID BIGINT not null,
       NAME TEXT not null,
       primary key (ENVIRONMENT_REGISTER_ID, NAME),
       constraint FK86CD31CA722350EE foreign key (PROPERTY_ID) references PROPERTY,
       constraint FK86CD31CA3C59E2E3 foreign key (ENVIRONMENT_REGISTER_ID) references ENVIRONMENT_REGISTER
    )

    create table ENVIRONMENT_REGISTER_PROVISIONED_ZONES (
        ENVIRONMENT_REGISTER_ID BIGINT not null,
       PROVISIONED_ZONE_ID BIGINT not null,
       SIF_ID TEXT not null,
       primary key (ENVIRONMENT_REGISTER_ID, SIF_ID),
       constraint FK62F82A66A5E6A335 foreign key (PROVISIONED_ZONE_ID) references PROVISIONED_ZONE,
       constraint FK62F82A663C59E2E3 foreign key (ENVIRONMENT_REGISTER_ID) references ENVIRONMENT_REGISTER
    )
