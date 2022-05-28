CREATE TABLE "Sessions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Sessions" PRIMARY KEY,
    "ApplicationKey" TEXT NOT NULL,
    "EnvironmentUrl" TEXT NOT NULL,
    "InstanceId" TEXT NULL,
    "QueueId" TEXT NULL,
    "SessionToken" TEXT NOT NULL,
    "SolutionId" TEXT NULL,
    "SubscriptionId" TEXT NULL,
    "UserToken" TEXT NULL
);



