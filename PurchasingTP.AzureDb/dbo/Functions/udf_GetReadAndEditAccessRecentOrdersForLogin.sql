-- =============================================
-- Author:		Scott Kirkland
-- Create date: January 12, 2021
-- Description:	Given a Kerberos/LoginId, Return a list of orders that the user has read access and edit access to and have been modified recently
--	Notes:
--		Replaces vAccess.
--		Determines access on any order (regardless of status).  Uses udf_GetEditAccessRecentOrdersForLogin(@LoginId) and
--			udf_GetReadAccessRecentOrdersForLogin(@LoginId) user defined functions.
-- Usage:
/*
	select * from udf_GetReadAndEditAccessRecentOrdersForLogin('bazemore', '1/1/2020')
*/
-- =============================================
CREATE FUNCTION udf_GetReadAndEditAccessRecentOrdersForLogin
(
	-- Add the parameters for the function here
	@LoginId varchar(50),
	@Cutoff datetime
)
RETURNS
@ReadAndEditAccessOrders TABLE
(
	id int,
	orderid int,
	accessuserid varchar(10),
	readaccess bit,
	editaccess bit,
	isadmin bit,
	accesslevel char(2)
)
AS
BEGIN
	INSERT INTO @ReadAndEditAccessOrders
	select ROW_NUMBER() over (order by orderid) id, access.orderid, access.accessuserid, cast(access.readaccess as bit) readaccess, cast(access.editaccess as bit) editaccess, access.isadmin, accesslevel
from
	(
		select orderid, accessuserid, 1 readaccess, 1 editaccess, isadmin, accesslevel
		from udf_GetEditAccessRecentOrdersForLogin(@LoginId, @Cutoff)

		union

		select orderid, accessuserid, 1 readaccess, 0 editaccess, isadmin, accesslevel
		from udf_GetReadAccessRecentOrdersForLogin(@LoginId, @Cutoff)
	) access

	RETURN
END