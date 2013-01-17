CREATE PROCEDURE [dbo].[usp_SyncWorkgroupAccounts]
AS

insert into JobLogs (name, comments) values ('sync workgroup accounts', 'start')

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
			, (select userid from WorkgroupPermissions where workgroupid = w.id and roleid = 'AP' and IsDefaultForAccount = 1) AP
			, (select userid from WorkgroupPermissions where workgroupid = w.id and roleid = 'AM' and IsDefaultForAccount = 1) AM
			, (select userid from WorkgroupPermissions where workgroupid = w.id and roleid = 'PR' and IsDefaultForAccount = 1) PR
		from workgroups w
			inner join WorkgroupsXOrganizations wo on w.id = wo.WorkgroupId
			inner join vAccounts va on wo.OrganizationId = va.OrganizationId
		where w.id = @id
		  and va.isactive = 1
		) as src
	on t.workgroupid = src.workgroupid and t.accountid = src.accountid
	when not matched then 
		insert (accountid, workgroupid, ApproverUserId, AccountManagerUserId, PurchaserUserId) values (src.accountid, src.workgroupid, src.AP, src.AM, src.PR);

	-- remove the inactive ones
	delete from WorkgroupAccounts
	where id in (select wa.id from WorkgroupAccounts wa inner join vAccounts va on wa.AccountId = va.id where va.IsActive = 0 and wa.WorkgroupId = @id)

	fetch next from @cursor into @id

end

close @cursor
deallocate @cursor

insert into JobLogs (name, comments) values ('sync workgroup accounts', 'complete')

RETURN 0