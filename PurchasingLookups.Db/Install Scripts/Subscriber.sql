-- Adding the transactional pull subscription

declare @pub varchar(max), @login varchar(max), @password varchar(max), @snapshotfolder varchar(max)

-- publishing server's name
set @pub = N''	
-- login that is running the service
set @login = N''			
-- password for above login
set @password = N''
-- snapshot folder, share from the publisher to get the data from
set @snapshotfolder = N''

/****** Begin: Script to be run at Subscriber ******/
use [PrePurchasingLookups]
exec sp_addpullsubscription @publisher = @pub, @publication = N'PrePurchasing Lookups', @publisher_db = N'PrePurchasingLookups', 
	@independent_agent = N'True', @subscription_type = N'pull', @description = N'', @update_mode = N'read only', @immediate_sync = 1

exec sp_addpullsubscription_agent @publisher = N'CLAMPS', @publisher_db = N'PrePurchasingLookups', @publication = N'PrePurchasing Lookups', 
	@distributor = @pub, @distributor_security_mode = 1, @distributor_login = N'', @distributor_password = N'', @enabled_for_syncmgr = N'False', 
	@frequency_type = 4, @frequency_interval = 1, @frequency_relative_interval = 1, @frequency_recurrence_factor = 0, @frequency_subday = 1, 
	@frequency_subday_interval = 0, @active_start_time_of_day = 53000, @active_end_time_of_day = 235959, @active_start_date = 0, @active_end_date = 0, 
	@alt_snapshot_folder = @snapshotfolder, @working_directory = N'', @use_ftp = N'False', 
	@job_login = @login, @job_password = @password, @publication_type = 0
GO
/****** End: Script to be run at Subscriber ******/

/****** Begin: Script to be run at Publisher ******/
/*use [PrePurchasingLookups]
-- Parameter @sync_type is scripted as 'automatic', please adjust when appropriate.
exec sp_addsubscription @publication = N'PrePurchasing Lookups', @subscriber = N'JOEYMOUSEPADS', @destination_db = N'PrePurchasingLookups', @sync_type = N'Automatic', @subscription_type = N'pull', @update_mode = N'read only'
*/
/****** End: Script to be run at Publisher ******/

