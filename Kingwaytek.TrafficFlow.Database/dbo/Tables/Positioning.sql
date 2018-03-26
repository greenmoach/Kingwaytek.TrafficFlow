CREATE TABLE [dbo].[Positioning] (
    [Id]                INT             NOT NULL,
    [InvestigationType] INT             NOT NULL,
    [City]              NVARCHAR (50)   NOT NULL,
    [Town]              NVARCHAR (50)   NOT NULL,
    [Road1]             NVARCHAR (150)  NOT NULL,
    [Road2]             NVARCHAR (150)  NOT NULL,
    [Latitude]          DECIMAL (10, 8) NOT NULL,
    [Longitude]         DECIMAL (11, 8) NOT NULL,
    [Positioning]       NVARCHAR (MAX)  NOT NULL,
    CONSTRAINT [PK_Positioning] PRIMARY KEY CLUSTERED ([Id] ASC, [InvestigationType] ASC)
);

