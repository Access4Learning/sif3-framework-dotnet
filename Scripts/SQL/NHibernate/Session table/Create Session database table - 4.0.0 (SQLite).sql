CREATE TABLE Sessions
(
    Id TEXT NOT NULL CONSTRAINT PK_Session PRIMARY KEY,
    ApplicationKey TEXT NOT NULL,
    EnvironmentUrl TEXT NOT NULL,
    SessionToken TEXT NOT NULL,
    SolutionId TEXT NULL,
    InstanceId TEXT NULL,
    UserToken TEXT NULL,
    SubscriptionId TEXT NULL,
    QueueId TEXT NULL
)
