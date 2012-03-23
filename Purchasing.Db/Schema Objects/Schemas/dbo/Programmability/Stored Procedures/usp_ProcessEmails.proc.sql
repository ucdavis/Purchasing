CREATE PROCEDURE [dbo].[usp_ProcessEmails]
	
	@perevent bit = 0,
	@daily bit = 0,
	@weekly bit = 0

AS

	-- declare variables
	declare @pcursor cursor, @userid varchar(10), @text varchar(max), @body varchar(max), @email varchar(50)
	declare @ntype varchar(50)

	-- determine which type/interval we are sending out
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

	-- find the distinct people that have messages pending		
	set @pcursor = cursor for
		select distinct userid from emailqueue where pending = 1 and lower(notificationtype) = @ntype
	
	open @pcursor

	fetch next from @pcursor into @userid

	while(@@FETCH_STATUS = 0)
	begin

		set @text = null
	
		-- get the individual notifications into a li
		select @text = coalesce(@text + '</li><li>', '<li>') + [text]
		from emailqueue where pending = 1 and lower(notificationtype) = @ntype and userid = @userid
	
		set @email = null
		select @email = email from Users where id = @userid

		-- build up the body of the email
		set @body = null
		set @body = '<p>***This is an automatically generated email.  Please do not respond to this email.***</p>'
		set @body = '<p>Here is your summary for the PrePurchasing system.</p><ul>'
		set @body = @body + @text
		set @body = @body + '</li></ul> <p>-The PrePurchasing System</p>'

		-- execute the send
		exec msdb.dbo.sp_send_dbmail
			@profile_name = 'Opp No Reply',
			@recipients = @email,
			@subject = 'PrePurchasing Notifications',
			@body = @body,
			@body_format = 'HTML'

		-- mark the messages as sent
		update EmailQueue set Pending = 0, DateTimeSent = GETDATE() where Pending = 1 and LOWER(NotificationType) = @ntype and UserId = @userid

		fetch next from @pcursor into @userid

	end

	close @pcursor
	deallocate @pcursor

RETURN 0