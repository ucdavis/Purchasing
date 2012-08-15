CREATE VIEW [dbo].[vAccess]
	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.accessuserid, access.readaccess, access.editaccess, access.isadmin, access.IsAway, accesslevel
from
	(
	select orderid, accessuserid, 1 readaccess, 1 editaccess, [admin] isadmin, isaway, accesslevel
	from vEditAccess
	union
	select orderid, accessuserid, 1 readaccess, 0 editaccess, [admin] isadmin, isaway, accesslevel
	from vReadAccess

) access