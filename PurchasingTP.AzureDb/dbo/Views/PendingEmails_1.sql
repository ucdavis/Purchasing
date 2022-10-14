CREATE VIEW dbo.PendingEmails
AS
SELECT        TOP (100) PERCENT dbo.EmailQueueV2.*, dbo.Users.Email AS Expr1, dbo.Users.FirstName, dbo.Users.LastName
FROM            dbo.EmailQueueV2 INNER JOIN
                         dbo.Users ON dbo.EmailQueueV2.UserId = dbo.Users.Id
WHERE        (dbo.EmailQueueV2.Pending = 1) AND (dbo.EmailQueueV2.NotificationType = 'PerEvent')
ORDER BY dbo.EmailQueueV2.DateTimeSent
GO


