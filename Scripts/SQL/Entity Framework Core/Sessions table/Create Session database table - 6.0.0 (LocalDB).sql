CREATE TABLE [Sessions] (
    [Id] uniqueidentifier NOT NULL,
    [ApplicationKey] nvarchar(max) NOT NULL,
    [EnvironmentUrl] nvarchar(max) NOT NULL,
    [InstanceId] nvarchar(max) NULL,
    [QueueId] nvarchar(max) NULL,
    [SessionToken] nvarchar(max) NOT NULL,
    [SolutionId] nvarchar(max) NULL,
    [SubscriptionId] nvarchar(max) NULL,
    [UserToken] nvarchar(max) NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id])
);
GO



