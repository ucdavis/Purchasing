CREATE PROCEDURE [dbo].[usp_SyncWorkgroupAccounts]
AS

declare @cursor cursor, @id int

set @cursor = cursor for
	select workgroups.id from workgroups where syncaccounts = 1

open @cursor

fetch next from @cursor into @id

while(@@FETCH_STATUS = 0)
begin

	-- insert the new ones
	merge workgroupaccounts as t
	using (
		select w.id workgroupid, va.id accountid
		from workgroups w
			inner join WorkgroupsXOrganizations wo on w.id = wo.WorkgroupId
			inner join vAccounts va on wo.OrganizationId = va.OrganizationId
		where w.id = @id
		  and va.isactive = 1
		) as src
	on t.workgroupid = src.workgroupid and t.accountid = src.accountid
	when not matched then 
		insert (accountid, workgroupid) values (src.accountid, src.workgroupid);

	-- remove the inactive ones
	delete from WorkgroupAccounts
	where id in (select wa.id from WorkgroupAccounts wa inner join vAccounts va on wa.AccountId = va.id where va.IsActive = 0 and wa.WorkgroupId = @id)

	fetch next from @cursor into @id

end

close @cursor
deallocate @cursor

RETURN 0