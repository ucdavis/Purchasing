CREATE PROCEDURE [dbo].[usp_ProcessOrgDescendants]

AS

	/*

		Recursively try to find all descendants of a parent org
		http://blogs.msdn.com/b/securitytools/archive/2009/06/23/traversing-a-recursive-table-self-reference-using-t-sql.aspx

	*/

	-- reset the table seed
	--truncate table vorganizationdescendants
	--DBCC CHECKIDENT(vorganizationdescendants, reseed, 1)

	insert into JobLogs (name, comments) values ('process org descendants', 'start')

	declare @cursor cursor, @top varchar(10)
	declare @descendants table (orgid varchar(10), name varchar(max), immediateparent varchar(10), isactive bit)

	declare @tmp table (id int primary key identity, orgid varchar(10), name varchar(50), isactive bit, immediateparentid varchar(10), rollupparentid varchar(10))

	set @cursor = cursor for
		select distinct parentid from vorganizations where parentid is not null

	open @cursor

	fetch next from @cursor into @top

	while(@@FETCH_STATUS = 0)
	begin

		-- clear the table
		delete from @descendants

		-- get the immediate descendants
		insert into @descendants
		select child.id, child.name, child.parentid, child.IsActive 
		from vorganizations parent
			inner join vorganizations child on child.parentid = parent.id
		where parent.id = @top

		-- get all the descendants recursively
		while @@ROWCOUNT > 0
		BEGIN

			insert into @descendants (orgid, name, immediateparent, isactive)
			select child.id, child.name, child.parentid, child.IsActive
			from @descendants parent
				inner join vorganizations child on child.parentid = parent.orgid
							and not exists ( select parent2.orgid from @descendants parent2 where parent2.orgid  = child.id )	

		END

		-- insert the top
		insert into @tmp (orgid, name, immediateparentid, rollupparentid, IsActive)
		select id, name, null, @top, IsActive
		from vOrganizations
		where id = @top

		-- insert into descendants
		--insert into vorganizationdescendants (orgid, name, immediateparentid, rollupparentid, IsActive)
		--select orgid, name, immediateparent, @top, isactive from @descendants

		insert into @tmp (orgid, name, immediateparentid, rollupparentid, IsActive)
		select orgid, name, immediateparent, @top, isactive from @descendants

		-- insert itself

		fetch next from @cursor into @top

	end

	close @cursor
	deallocate @cursor

	insert into JobLogs (name, comments) values ('process org descendants', 'determined org descendants')

	-- insert all the orgs that don't have anything report to it
	--insert into vOrganizationDescendants(OrgId, Name, ImmediateParentId, RollupParentId, IsActive)
	--select id, Name, ParentId, id, 1 from vorganizations where id not in (
	--	select distinct parentid from vorganizations where parentid is not null
	--)
	insert into @tmp(OrgId, Name, ImmediateParentId, RollupParentId, IsActive)
	select id, Name, ParentId, id, 1 from vorganizations where id not in (
		select distinct parentid from vorganizations where parentid is not null
	)

	insert into JobLogs (name, comments) values ('process org descendants', 'determined top level orgs')

	-- compare the tmp results to existing results, cannot be more than 10% below existing data
	declare @existingCount int, @newCount int, @message nvarchar(500)

	-- 90% of the existing count
	set @existingCount = (select count(id) from (select top 90 percent id from vorganizationdescendants) orgdescendants)
	-- our new table's worth of data
	set @newCount = (select count(id) from @tmp)

	
	if (@newCount < @existingCount)
	begin

		set @message = 'Only processed ' + convert(varchar(10),@newCount) + ' org descendant records, 90% of original count was ' + convert(varchar(10),@existingCount)

		-- we have a problem, too many records are missing
		insert into ELMAH_Error (Application, Host, Type, Source, Message, [User], StatusCode, AllXml, TimeUtc)
		values ('Org Descendants', '', '', '', @message, ' ', -1, '', getutcdate())

		insert into JobLogs (name, comments) values ('process org descendants', 'failed, check elmah_error table')

	end
	else
	begin

		truncate table vorganizationdescendants
		
		insert into vOrganizationDescendants(OrgId, Name, ImmediateParentId, RollupParentId, IsActive)
		select OrgId, Name, ImmediateParentId, RollupParentId, IsActive from @tmp

		insert into JobLogs (name, comments) values ('process org descendants', 'updated ' + cast(@newCount as varchar(5)) + ' records')

	end
	
RETURN 0