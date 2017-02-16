


-- =============================================
-- Author:		Ken Taylor
-- Create date: January 20, 2017
-- Description:	Given a Kerberos/UCD Login ID, and a number of rows to return, return a list of the most recent n comments.
--	Notes: 
--		Replaces vCommenthistory.--
-- Usage:
/*
	select * from udf_GetCommentHistoryForLogin('rajahn', 5)
*/
-- Modifications:
--	20170216 by kjt: Needed to add grouping statement in order not to repeat same comment being returned than once because we added column "isAdmin" to
--		udf_GetReadAndEditAccessOrderIdsForLogin after this was initially created.
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
 FROM dbo.udf_GetReadAndEditAccessOrderIdsForLogin(@LoginId) access -- Returns orderIds AND isAdmin only
    INNER JOIN dbo.OrderComments ON access.OrderId = dbo.OrderComments.OrderId
	INNER JOIN dbo.Orders ON dbo.Orders.Id = dbo.OrderComments.OrderId  
 	INNER JOIN dbo.Users ON dbo.Users.Id = dbo.OrderComments.UserId 
GROUP BY access.OrderId, OrderComments.OrderId, Orders.RequestNumber, Users.FirstName, Users.LastName, OrderComments.Text, UserId, OrderComments.DateCreated
ORDER BY dbo.OrderComments.DateCreated DESC, access.OrderId, OrderComments.OrderId, Orders.RequestNumber, Users.FirstName, Users.LastName, OrderComments.Text, UserId
	
	RETURN 
END