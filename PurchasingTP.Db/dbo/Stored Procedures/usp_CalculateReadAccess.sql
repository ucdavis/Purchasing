--CREATE PROCEDURE [dbo].[usp_CalculateReadAccess]
--AS
	
--	merge vReadAccess as t
--	using vReadAccessView as s
--	on t.orderid = s.orderid and t.accessuserid = s.accessuserid and t.accesslevel = s.accesslevel and t.isadmin = s.[isadmin]
--	when not matched
--		then insert (orderid, accessuserid, isadmin, accesslevel) values (s.orderid, s.accessuserid, s.isadmin, s.accesslevel)
--	when not matched by source
--		then delete;

--RETURN 0
