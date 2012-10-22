CREATE VIEW dbo.vCommentHistory
AS

SELECT DISTINCT NEWID() AS id, dbo.OrderComments.OrderId, dbo.Orders.RequestNumber, dbo.Users.FirstName + ' ' + dbo.Users.LastName AS CreatedBy, 
		dbo.OrderComments.Text AS comment, dbo.OrderComments.UserId AS createdbyuserid, dbo.OrderComments.DateCreated, access.UserId AS access
FROM dbo.OrderComments 
	INNER JOIN dbo.Orders ON dbo.Orders.Id = dbo.OrderComments.OrderId 
	INNER JOIN dbo.Users ON dbo.Users.Id = dbo.OrderComments.UserId 
	INNER JOIN (
		SELECT DISTINCT OrderId, UserId
		FROM (
			select orderid, accessuserid userid
			from vaccess
			where isadmin = 0
		) AS blank
	) AS access ON access.OrderId = dbo.OrderComments.OrderId
GO