/*

	Description:

	Determines access on any order (regardless of status).  Uses vEditAccess and vReadAccess queries.

	This is a fairly slow query when used in practice.

*/

CREATE VIEW [dbo].[vAccess]
	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.accessuserid, cast(access.readaccess as bit) readaccess, cast(access.editaccess as bit) editaccess, access.isadmin, accesslevel
from
	(
	select orderid, accessuserid, 1 readaccess, 1 editaccess, isadmin, accesslevel
	from vEditAccess
	union
	select orderid, accessuserid, 1 readaccess, 0 editaccess, isadmin, accesslevel
	from vReadAccess
) access