CREATE VIEW [dbo].[vReadAccess]

	AS 

select ROW_NUMBER() over (order by orderid) id, access.orderid, access.UserId accessuserid, access.IsAway
from
(
select distinct orderid, userid, users.IsAway
from ordertracking
	inner join Users on users.Id = ordertracking.userid
) access