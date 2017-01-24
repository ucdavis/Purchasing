

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
-- Modifications:
--	20170124 by kjt: Added "isadmin" as additional return table column.
-- =============================================
CREATE FUNCTION [dbo].[udf_GetReadAndEditAccessOrderIdsForLogin] 
(
	-- Add the parameters for the function here
	@LoginId varchar(50)
)
RETURNS 
@ReadAndEditAccessOrderIds TABLE 
( 
	orderid int,
	isadmin bit
)
AS
BEGIN
	INSERT INTO @ReadAndEditAccessOrderIds
	select access.orderid, access.isadmin
from
	(
		select orderid, isadmin
		from udf_GetEditAccessOrdersForLogin(@LoginId)

		union

		select orderid, isadmin
		from udf_GetReadAccessOrdersForLogin(@LoginId)

	) access
	
	RETURN 
END