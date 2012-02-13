CREATE PROCEDURE [dbo].[usp_ProcessEmails]
	
	@perevent bit = 0,
	@daily bit = 0,
	@weekly bit = 0

AS

	declare @pcursor cursor, @userid varchar(10), @text varchar(max), @body varchar(max)

	declare @ntype varchar(50)

	if (@perevent = 1)
	begin
		set @ntype = 'perevent'
	end
	else if (@daily = 1)
	begin
		set @ntype = 'daily'
	end
	else if (@weekly = 1)
	begin
		set @ntype = 'weekly'
	end
	else
	begin
		return 0
	end
		
	set @pcursor = cursor for
		select distinct userid from emailqueue where pending = 1 and lower(notificationtype) = @ntype
	
	open @pcursor

	fetch next from @pcursor into @userid

	while(@@FETCH_STATUS = 0)
	begin

		set @text = null
	
		select @text = coalesce(@text + '</li><li>', '<li>') + [text]
		from emailqueue where pending = 1 and lower(notificationtype) = @ntype and userid = @userid
	
		set @body = null

		set @body = '<p>Here is your summary for the PrePurchasing system.</p><ul>'

		set @body = @body + @text + '</li>'

		set @body = @body + '</ul> <p>-The PrePurchasing System</p>'

		exec msdb.dbo.sp_send_dbmail
			@profile_name = 'automatedemail',
			@recipients = 'anlai@ucdavis.edu',
			--@from_address = 'no-reply@prepurchasing@ucdavis.edu',
			@subject = 'PrePurchasing Notifications',
			@body = @body,
			@body_format = 'HTML'

		fetch next from @pcursor into @userid

	end

	close @pcursor
	deallocate @pcursor

RETURN 0