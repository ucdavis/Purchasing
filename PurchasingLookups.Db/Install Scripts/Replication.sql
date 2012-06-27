-- Enabling the replication database
use master
exec sp_replicationdboption @dbname = N'PrePurchasingLookups', @optname = N'publish', @value = N'true'
GO

-- Adding the snapshot publication
use [PrePurchasingLookups]
exec sp_addpublication @publication = N'PrePurchasing Lookups', @description = N'Snapshot publication of database ''PrePurchasingLookups'' from Publisher ''CLAMPS''.', @sync_method = N'native', @retention = 0, @allow_push = N'true', @allow_pull = N'true', @allow_anonymous = N'true', @enabled_for_internet = N'false', @snapshot_in_defaultfolder = N'true', @compress_snapshot = N'false', @ftp_port = 21, @ftp_login = N'anonymous', @allow_subscription_copy = N'false', @add_to_active_directory = N'false', @repl_freq = N'snapshot', @status = N'active', @independent_agent = N'true', @immediate_sync = N'true', @allow_sync_tran = N'false', @autogen_sync_procs = N'false', @allow_queued_tran = N'false', @allow_dts = N'false', @replicate_ddl = 1
GO


exec sp_addpublication_snapshot @publication = N'PrePurchasing Lookups', @frequency_type = 4, @frequency_interval = 1, @frequency_relative_interval = 1, @frequency_recurrence_factor = 0, @frequency_subday = 1, @frequency_subday_interval = 1, @active_start_time_of_day = 50000, @active_end_time_of_day = 235959, @active_start_date = 0, @active_end_date = 0, @job_login = N'aesdean\prodreplication', @job_password = null, @publisher_security_mode = 1
exec sp_grant_publication_access @publication = N'PrePurchasing Lookups', @login = N'sa'
GO
exec sp_grant_publication_access @publication = N'PrePurchasing Lookups', @login = N'NT AUTHORITY\SYSTEM'
GO

/*
	fill in the login for the job that will be running the job
exec sp_grant_publication_access @publication = N'PrePurchasing Lookups', @login = N''
GO
*/

-- Adding the snapshot articles
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'ParamNameAndValue', @source_owner = N'dbo', @source_object = N'ParamNameAndValue', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'ParamNameAndValue', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'udf_GetParameterValue', @source_owner = N'dbo', @source_object = N'udf_GetParameterValue', @type = N'func schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'udf_GetParameterValue', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'UnitOfMeasures', @source_owner = N'dbo', @source_object = N'UnitOfMeasures', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'UnitOfMeasures', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'usp_Post_DownloadAccountsTable_Processing', @source_owner = N'dbo', @source_object = N'usp_Post_DownloadAccountsTable_Processing', @type = N'proc schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'usp_Post_DownloadAccountsTable_Processing', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'usp_Post_DownloadBuildingsTable_Processing', @source_owner = N'dbo', @source_object = N'usp_Post_DownloadBuildingsTable_Processing', @type = N'proc schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'usp_Post_DownloadBuildingsTable_Processing', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'usp_Post_DownloadCommoditiesTable_Processing', @source_owner = N'dbo', @source_object = N'usp_Post_DownloadCommoditiesTable_Processing', @type = N'proc schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'usp_Post_DownloadCommoditiesTable_Processing', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'usp_Post_DownloadVendorsTable_Processing', @source_owner = N'dbo', @source_object = N'usp_Post_DownloadVendorsTable_Processing', @type = N'proc schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'usp_Post_DownloadVendorsTable_Processing', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'usp_RunPostProcessingScripts', @source_owner = N'dbo', @source_object = N'usp_RunPostProcessingScripts', @type = N'proc schema only', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x0000000008000001, @destination_table = N'usp_RunPostProcessingScripts', @destination_owner = N'dbo', @status = 16
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vAccounts', @source_owner = N'dbo', @source_object = N'vAccounts', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vAccounts', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vBuildings', @source_owner = N'dbo', @source_object = N'vBuildings', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vBuildings', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vCommodities', @source_owner = N'dbo', @source_object = N'vCommodities', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vCommodities', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vCommodityGroups', @source_owner = N'dbo', @source_object = N'vCommodityGroups', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vCommodityGroups', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vOrganizationDescendants', @source_owner = N'dbo', @source_object = N'vOrganizationDescendants', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'manual', @destination_table = N'vOrganizationDescendants', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vOrganizations', @source_owner = N'dbo', @source_object = N'vOrganizations', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vOrganizations', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vSubAccounts', @source_owner = N'dbo', @source_object = N'vSubAccounts', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vSubAccounts', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vVendorAddresses', @source_owner = N'dbo', @source_object = N'vVendorAddresses', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vVendorAddresses', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO
use [PrePurchasingLookups]
exec sp_addarticle @publication = N'PrePurchasing Lookups', @article = N'vVendors', @source_owner = N'dbo', @source_object = N'vVendors', @type = N'logbased', @description = N'', @creation_script = N'', @pre_creation_cmd = N'drop', @schema_option = 0x000000000903509D, @identityrangemanagementoption = N'none', @destination_table = N'vVendors', @destination_owner = N'dbo', @status = 24, @vertical_partition = N'false', @ins_cmd = N'SQL', @del_cmd = N'SQL', @upd_cmd = N'SQL'
GO