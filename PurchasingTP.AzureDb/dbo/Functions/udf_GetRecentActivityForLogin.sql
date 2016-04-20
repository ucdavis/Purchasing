

-- =============================================
-- Author:		Scott Kirkland
-- Create date: 4/20/2016
-- Description:	Given a Kerberos/LoginId, Return most recent activity
--	Notes:
--		Replaces vOrderTracking.
--
-- Usage:
/*
	select * from udf_GetRecentActivityForLogin('bazemore') 
*/
-- Modifications:
-- =============================================
CREATE FUNCTION [dbo].[udf_GetRecentActivityForLogin]
(
	-- Add the parameters for the function here
	@LoginId varchar(50) 
)
RETURNS 
@RecentActivity TABLE 
(
	  [trackingdate] datetime
      ,[orderid] int
      ,[requestnumber] varchar(20)
      ,[datecreated] datetime
      ,[createdby] varchar(101)
      ,[accessuserid] varchar(10)
      ,[summary] varchar(max)
)
AS
BEGIN
	INSERT INTO @RecentActivity
	select top 1 ordertracking.datecreated TrackingDate
		, ordertracking.orderid, orders.requestnumber
		, orders.datecreated, creator.firstname + ' ' + creator.lastname createdby
		, users.Id AccessUserId
		, orders.LineItemSummary as summary
	from users
			inner join ordertracking on ordertracking.userid = users.id
			inner join orders on ordertracking.orderid = orders.id
			inner join users creator on orders.createdby = creator.id		 
	where users.Id = @LoginId 
			AND
			ordertracking.datecreated in ( select max(tracking.datecreated)
									from ordertracking tracking
									where ordertracking.userid = tracking.userid)
	order by ordertracking.datecreated desc

	RETURN 
END