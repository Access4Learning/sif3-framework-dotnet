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


INSERT INTO Sessions
(Id, ApplicationKey, EnvironmentUrl, SessionToken, SolutionId)
VALUES
('a38d4676-65b4-4ba6-b2e2-19e131417447', 'Sif3DemoConsumer', 'http://localhost:62921/api/environments/bca76787-48ae-4532-b851-fd7099a4098b', 'U2lmM0RlbW9Db25zdW1lcjo6OlNpZjNGcmFtZXdvcms=', 'Sif3Framework')

