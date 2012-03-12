USE [msdb]
GO

/****** Object:  Job [Update PrePurchasing Tables]    Script Date: 03/09/2012 17:48:34 ******/
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'Update PrePurchasing Tables')
EXEC msdb.dbo.sp_delete_job @job_id=N'3528c2b1-bdbb-4ad8-af6d-010d4d1ea234', @delete_unused_schedule=1
GO

USE [msdb]
GO

/****** Object:  Job [Update PrePurchasing Tables]    Script Date: 03/09/2012 17:48:34 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 03/09/2012 17:48:34 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Update PrePurchasing Tables', 
        @enabled=1, 
        @notify_level_eventlog=0, 
        @notify_level_email=0, 
        @notify_level_netsend=0, 
        @notify_level_page=0, 
        @delete_level=0, 
        @description=N'Calls the usp_LoadAllPrePurchasingTables sproc to update the PrePurchasing Organizations, Accounts, SubAccounts, Commodities, Vendors and VendorAdresses tables.', 
        @category_name=N'[Uncategorized (Local)]', 
        @owner_login_name=N'AESDEAN\taylor', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EXEC usp_LoadAllPrePurchasingTables]    Script Date: 03/09/2012 17:48:34 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EXEC usp_LoadAllPrePurchasingTables', 
        @step_id=1, 
        @cmdexec_success_code=0, 
        @on_success_action=3, 
        @on_success_step_id=0, 
        @on_fail_action=2, 
        @on_fail_step_id=0, 
        @retry_attempts=0, 
        @retry_interval=0, 
        @os_run_priority=0, @subsystem=N'TSQL', 
        @command=N'USE [PrePurchasing]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[usp_LoadAllPrePurchasingTables]
        @LinkedServerName = ''FIS_DS'', --(default value)
        @IsDebug = 0 --(default value)

SELECT	''Return Value'' = @return_value

GO', 
        @database_name=N'PrePurchasing', 
        @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EXEC Usp_processorgdescendants]    Script Date: 03/09/2012 17:48:34 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EXEC Usp_processorgdescendants', 
        @step_id=2, 
        @cmdexec_success_code=0, 
        @on_success_action=3, 
        @on_success_step_id=0, 
        @on_fail_action=2, 
        @on_fail_step_id=0, 
        @retry_attempts=0, 
        @retry_interval=0, 
        @os_run_priority=0, @subsystem=N'TSQL', 
        @command=N'USE [PrePurchasing]
GO

DECLARE @return_value int

EXEC	@return_value = [dbo].[Usp_processorgdescendants]
        
SELECT	''Return Value'' = @return_value

GO', 
        @database_name=N'PrePurchasing', 
        @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EXEC Usp_syncworkgroupaccounts]    Script Date: 03/09/2012 17:48:34 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EXEC Usp_syncworkgroupaccounts', 
        @step_id=3, 
        @cmdexec_success_code=0, 
        @on_success_action=1, 
        @on_success_step_id=0, 
        @on_fail_action=2, 
        @on_fail_step_id=0, 
        @retry_attempts=0, 
        @retry_interval=0, 
        @os_run_priority=0, @subsystem=N'TSQL', 
        @command=N'USE [PrePurchasing]
GO

DECLARE @return_value int

EXEC	@return_value = [dbo].[Usp_syncworkgroupaccounts]
        
SELECT	''Return Value'' = @return_value

GO', 
        @database_name=N'PrePurchasing', 
        @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO


