

-- =============================================
-- Author:		Ken Taylor
-- Create date: January 20, 2017
-- Description:	Given a Kerberos/UCD Login ID, and a number of rows to return, return a list of the most recent n comments.
--	Notes: 
--		Replaces vCommenthistory.--
-- Usage:
/*
	select * from udf_GetCommentHistoryForLogin('bazemore', 5)
*/
-- Modifications:
-- =============================================
CREATE FUNCTION [dbo].[udf_GetCommentHistoryForLogin]
(
	@LoginId varchar(50),
	@NumRows int = 5 
)
RETURNS 
	@CommentHistory TABLE 
	(
	   [Id] UNIQUEIDENTIFIER
      ,[OrderId] int
      ,[RequestNumber] varchar(20)
      ,[CreatedBy] varchar(101)
      ,[Comment] varchar(MAX)
      ,[CreatedByUserId] varchar(10)
      ,[DateCreated] datetime2(7)
      ,[Access] varchar(50)
	)
AS
BEGIN
	INSERT INTO @CommentHistory
	SELECT TOP (@NumRows)  
	(SELECT MyNewID FROM dbo.Get_NEWID) AS id, 
	dbo.OrderComments.OrderId, 
	dbo.Orders.RequestNumber, 
	dbo.Users.FirstName + ' ' + dbo.Users.LastName AS CreatedBy,  
 	dbo.OrderComments.Text AS comment, 
	dbo.OrderComments.UserId AS createdbyuserid, 
	dbo.OrderComments.DateCreated, 
	@LoginId AS access 
 FROM dbo.udf_GetReadAndEditAccessOrderIdsForLogin(@LoginId) access -- Returns orderIds only
    INNER JOIN dbo.OrderComments ON access.OrderId = dbo.OrderComments.OrderId
	INNER JOIN dbo.Orders ON dbo.Orders.Id = dbo.OrderComments.OrderId  
 	INNER JOIN dbo.Users ON dbo.Users.Id = dbo.OrderComments.UserId  
ORDER BY dbo.OrderComments.DateCreated DESC
	
	RETURN 
END