CREATE TABLE [dbo].[Investigation] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [InvestigationType]  INT            NOT NULL,
    [PositioningId]      INT            NOT NULL,
    [IntersectionId]     NVARCHAR (50)  NULL,
    [Weather]            NVARCHAR (50)  NULL,
    [InvestigaionTime]   DATETIME       NOT NULL,
    [TrafficControlNote] NVARCHAR (500) NULL,
    [FileName]           NVARCHAR (250) NOT NULL,
    [CreateTime]         DATETIME       NOT NULL,
    [LastEditTime]       DATETIME       NULL,
    [Deleted]            BIT            CONSTRAINT [DF_Investigaion_Deleted] DEFAULT ((0)) NOT NULL,
    [Enabled]            BIT            CONSTRAINT [DF_Investigaion_Enabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Investigaion_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Investigation_Positioning] FOREIGN KEY ([PositioningId], [InvestigationType]) REFERENCES [dbo].[Positioning] ([Id], [InvestigationType])
);




GO


