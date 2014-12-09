CREATE TABLE [dbo].[WorkgroupVendors_copy] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [WorkgroupId]           INT           NOT NULL,
    [VendorId]              CHAR (10)     NULL,
    [VendorAddressTypeCode] VARCHAR (4)   NULL,
    [Name]                  VARCHAR (40)  NOT NULL,
    [Line1]                 VARCHAR (40)  NOT NULL,
    [Line2]                 VARCHAR (40)  NULL,
    [Line3]                 VARCHAR (40)  NULL,
    [City]                  VARCHAR (40)  NOT NULL,
    [State]                 CHAR (2)      NOT NULL,
    [Zip]                   VARCHAR (11)  NOT NULL,
    [CountryCode]           VARCHAR (2)   NULL,
    [IsActive]              BIT           CONSTRAINT [DF_WorkgroupVendors_copy_IsActive] DEFAULT ((1)) NOT NULL,
    [Phone]                 VARCHAR (15)  NULL,
    [Fax]                   VARCHAR (15)  NULL,
    [Email]                 VARCHAR (50)  NULL,
    [Url]                   VARCHAR (128) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkgroupVendors_copy_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);

