CREATE VIEW [dbo].[vAccess]
	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.accessuserid, access.readaccess, access.editaccess, access.isadmin, accesslevel
from
	(
	select orderid, accessuserid, readaccess, editaccess, isadmin, accesslevel
	from vEditAccess
	union
	select orderid, accessuserid, readaccess, editaccess, isadmin, accesslevel
	from vReadAccess
) access