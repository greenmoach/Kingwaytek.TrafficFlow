CREATE TABLE [dbo].[InvestigationData] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [InvestigationId]    INT           NOT NULL,
    [Intersection]       NVARCHAR (10) NOT NULL,
    [Direction]          NVARCHAR (10) NOT NULL,
    [Amount]             INT           NOT NULL,
    [TargetType]         INT           NOT NULL,
    [HourlyInterval]     NVARCHAR (50) NOT NULL,
    [FirstQuarterCount]  INT           NULL,
    [SecondQuarterCount] INT           NULL,
    [ThirdQuarterCount]  INT           NULL,
    [FourthQuarterCount] INT           NULL,
    CONSTRAINT [PK_Investigaion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InvestigationData_Investigation] FOREIGN KEY ([InvestigationId]) REFERENCES [dbo].[Investigation] ([Id])
);

