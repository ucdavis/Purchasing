CREATE TABLE [dbo].[vBuildings] (
    [Id]              VARCHAR (10) NOT NULL,
    [CampusCode]      VARCHAR (2)  NOT NULL,
    [BuildingCode]    VARCHAR (4)  NOT NULL,
    [CampusName]      VARCHAR (40) NULL,
    [CampusShortName] VARCHAR (12) NULL,
    [CampusTypeCode]  VARCHAR (1)  NULL,
    [BuildingName]    VARCHAR (80) NULL,
    [LastUpdateDate]  DATETIME     NULL,
    [IsActive]        BIT          NULL,
    CONSTRAINT [PK_vBuildings] PRIMARY KEY CLUSTERED ([CampusCode] ASC, [BuildingCode] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [vBuildings_Id_UDX]
    ON [dbo].[vBuildings]([Id] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'vBuildings: Identifies university buildings for use in CAMS. Data imported from DaFIS DS: FINANCE.campus_building.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Id: A concatenated Id containing the CampusCode<dash>BuildingCode fields.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'CampusCode*: Campus code (PK with BuildingCode).', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'CampusCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'BuildingCode*: Building code (PK with CampusCode).', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'BuildingCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'CampusName: Campus name.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'CampusName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'CampusShortName: Campus short name.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'CampusShortName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'CampusTypeCode: 	Campus type code.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'CampusTypeCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'BuildingName: The Name of the Building.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'BuildingName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'LastUpdateDate: The Decision Support date-time-stamp of the last update of this record. (system provided)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'LastUpdateDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'IsActive: Indicates whether this is a currently active record.  1 for yes; 0 for no.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'vBuildings', @level2type = N'COLUMN', @level2name = N'IsActive';

