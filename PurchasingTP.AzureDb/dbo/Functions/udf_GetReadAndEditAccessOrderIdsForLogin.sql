

-- =============================================
-- Author:		Ken Taylor
-- Create date: January 20, 2017
-- Description:	Given a Kerberos/LoginId, Return a list of order Ids that the user has read access and edit access to.
--	Notes:
--		Replaces vAccess, but only returns order IDs.
--		Determines access on any order (regardless of status).  Uses udf_GetEditAccessOrdersForLogin(@LoginId) and 
--			udf_GetReadAccessOrdersForLogin(@LoginId) user defined functions.
-- Usage:
/*
	select * from udf_GetReadAndEditAccessOrdersForLogin('bazemore') 
*/
-- =============================================
CREATE FUNCTION [dbo].[udf_GetReadAndEditAccessOrderIdsForLogin] 
(
	-- Add the parameters for the function here
	@LoginId varchar(50) 
)
RETURNS 
@ReadAndEditAccessOrders TABLE 
( 
	orderid int
)
AS
BEGIN
	INSERT INTO @ReadAndEditAccessOrders
	select access.orderid
from
	(
		select orderid
		from udf_GetEditAccessOrdersForLogin(@LoginId)
		WHERE IsAdmin = 0

		union

		select orderid
		from udf_GetReadAccessOrdersForLogin(@LoginId)
		WHERE IsAdmin = 0
	) access
	
	RETURN 
END