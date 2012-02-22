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
			SELECT OrderId, UserId
			FROM dbo.OrderTracking
			UNION
			SELECT dbo.Approvals.OrderId, dbo.Approvals.UserId
			FROM dbo.Approvals 
				INNER JOIN dbo.OrderStatusCodes AS osc ON dbo.Approvals.OrderStatusCodeId = osc.Id 
				INNER JOIN dbo.Orders AS Orders_2 ON dbo.Approvals.OrderId = Orders_2.Id 
				INNER JOIN dbo.OrderStatusCodes AS osc2 ON Orders_2.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
			WHERE dbo.Approvals.UserId IS NOT NULL
			UNION
			SELECT Approvals_1.OrderId, Approvals_1.SecondaryUserId
			FROM dbo.Approvals AS Approvals_1 
				INNER JOIN dbo.OrderStatusCodes AS osc ON Approvals_1.OrderStatusCodeId = osc.Id 
				INNER JOIN dbo.Orders AS Orders_1 ON Approvals_1.OrderId = Orders_1.Id 
				INNER JOIN dbo.OrderStatusCodes AS osc2 ON Orders_1.OrderStatusCodeId = osc2.Id AND osc.[Level] = osc2.[Level]
			WHERE (Approvals_1.SecondaryUserId IS NOT NULL)
		) AS blank
	) AS access ON access.OrderId = dbo.OrderComments.OrderId