CREATE PROCEDURE [dbo].[usp_SyncWorkgroupAccounts]
AS

	declare @cursor cursor, @id int, @orgid varchar(6)

	set @cursor = cursor for
		select workgroups.id, workgroups.primaryorganizationid from workgroups where syncaccounts = 1
	
	open @cursor

	fetch next from @cursor into @id, @orgid

	while(@@FETCH_STATUS = 0)
	begin

		-- delete accounts that are no longer part of the org
		delete from WorkgroupAccounts
		where workgroupid = @id and AccountId not in ( select id from vAccounts where OrganizationId = @orgid and IsActive = 1 )

		-- insert any new accounts that are part of the org
		insert into WorkgroupAccounts (WorkgroupId, AccountId)
		select @id, vaccounts.Id
		from vAccounts 
		where OrganizationId = @orgid
		  and id not in ( select accountid from WorkgroupAccounts where workgroupid = @id )
		  and IsActive = 1

		fetch next from @cursor into @id, @orgid

	end

	close @cursor
	deallocate @cursor

RETURN 0