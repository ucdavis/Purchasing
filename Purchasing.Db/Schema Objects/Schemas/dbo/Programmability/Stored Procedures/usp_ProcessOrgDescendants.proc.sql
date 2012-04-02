CREATE PROCEDURE [dbo].[usp_ProcessOrgDescendants]

AS

	/*

		Recursively try to find all descendants of a parent org
		http://blogs.msdn.com/b/securitytools/archive/2009/06/23/traversing-a-recursive-table-self-reference-using-t-sql.aspx

	*/

	-- reset the table seed
	truncate table vorganizationdescendants
	DBCC CHECKIDENT(vorganizationdescendants, reseed, 1)

	declare @cursor cursor, @top varchar(10)
	declare @descendants table (orgid varchar(10), name varchar(max), immediateparent varchar(10), isactive bit)

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
		insert into vorganizationdescendants (orgid, name, immediateparentid, rollupparentid, IsActive)
		select id, name, null, @top, IsActive
		from vOrganizations
		where id = @top

		-- insert into descendants
		insert into vorganizationdescendants (orgid, name, immediateparentid, rollupparentid, IsActive)
		select orgid, name, immediateparent, @top, isactive from @descendants

		-- insert itself

		fetch next from @cursor into @top

	end

	close @cursor
	deallocate @cursor

	-- insert all the orgs that don't have anything report to it
	insert into vOrganizationDescendants(OrgId, Name, ImmediateParentId, RollupParentId, IsActive)
	select id, Name, ParentId, id, 1 from vorganizations where id not in (
		select distinct parentid from vorganizations where parentid is not null
	)


RETURN 0