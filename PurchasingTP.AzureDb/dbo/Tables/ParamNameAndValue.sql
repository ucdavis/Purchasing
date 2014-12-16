CREATE TABLE [dbo].[ParamNameAndValue] (
    [ParamName]   VARCHAR (255)  NOT NULL,
    [ParamValue]  VARCHAR (255)  NOT NULL,
    [Description] VARCHAR (1024) NULL,
    [IsActive]    BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TableName_1] PRIMARY KEY CLUSTERED ([ParamName] ASC)
);

