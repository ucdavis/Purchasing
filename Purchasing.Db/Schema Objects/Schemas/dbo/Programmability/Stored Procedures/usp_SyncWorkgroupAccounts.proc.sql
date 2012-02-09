CREATE PROCEDURE [dbo].[usp_SyncWorkgroupAccounts]
AS

	declare @cursor cursor, @id int

	set @cursor = cursor for
		select workgroups.id, workgroups.primaryorganizationid from workgroups where syncaccounts = 1
	
	open @cursor

	fetch next from @cursor into @id

	while(@@FETCH_STATUS = 0)
	begin

		-- delete accounts that are no longer part of the org

		-- insert any new accounts that are part of the org

		fetch next from @cursor into @id

	end

	close @cursor
	deallocate @cursor

RETURN 0