CREATE TABLE [dbo].[Faqs]
(
	[Id]			int				NOT NULL	IDENTITY(1,1)	PRIMARY KEY, 
	[Category]		varchar(50)		NOT NULL,
	[Question]		varchar(MAX)	NOT NULL,
	[Answer]		varchar(MAX)	NOT NULL,
	[OrgId]			varchar(10)		NULL,
	[Like]			int				NOT NULL	DEFAULT 0
)
