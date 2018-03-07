CREATE TABLE [dbo].[Investigation] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [InvestigationType]    INT             NOT NULL,
    [PositioningId]        INT             NOT NULL,
    [PositioningCity]      NVARCHAR (50)   NOT NULL,
    [PositioningTown]      NVARCHAR (50)   NOT NULL,
    [PositioningRoad1]     NVARCHAR (150)  NOT NULL,
    [PositioningRoad2]     NVARCHAR (150)  NOT NULL,
    [PositioningLatitude]  DECIMAL (10, 8) NOT NULL,
    [PositioningLongitude] DECIMAL (11, 8) NOT NULL,
    [Positioning]          NVARCHAR (MAX)  NOT NULL,
    [IntersectionId]       NVARCHAR (50)   NULL,
    [Weather]              NVARCHAR (50)   NULL,
    [InvestigaionTime]     DATETIME        NOT NULL,
    [TrafficControlNote]   NVARCHAR (500)  NULL,
    [FileName]             NVARCHAR (250)  NOT NULL,
    [CreateTime]           DATETIME        NOT NULL,
    [LastEditTime]         DATETIME        NULL,
    [Deleted]              BIT             CONSTRAINT [DF_Investigaion_Deleted] DEFAULT ((0)) NOT NULL,
    [Enabled]              BIT             CONSTRAINT [DF_Investigaion_Enabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Investigaion_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'路口指向設定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Investigation', @level2type = N'COLUMN', @level2name = N'Positioning';

