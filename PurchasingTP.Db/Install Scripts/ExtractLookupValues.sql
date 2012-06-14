/*

	Description:
				
				Given a list of table, this script generates insert statements necessary to repopulate a blank database for the validation tables.

	Author:		Alan Lai
	Date:		6/14/2012

*/

-- specifiy the tables we want to extract
declare @tables table ( name varchar(max) )
insert into @tables (name) values ('OrderStatusCodes'), ('Roles'), ('ShippingTypes'), ('States') 

-- delete the temporary table it exists
if OBJECT_ID('tempdb..#tmpcopy') is not null begin drop table #tmpcopy end
-- create the temporary table
create table #tmpcopy (value varchar(max))

-- declare the variables we need
declare @tc cursor, @name varchar(max), @tsql varchar(max), @cols varchar(max), @cols2 varchar(max), @values varchar(max), @insert varchar(max)
declare @stmts table(stmt varchar(max))

-- start the cursor
set @tc = cursor for select name from @tables
open @tc
fetch next from @tc into @name

while(@@FETCH_STATUS = 0)
begin

	set @cols = null
	set @cols2 = null
	set @insert = null

	select @cols = COALESCE(@cols + ',', '') + COLUMN_NAME
	from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @name order by ORDINAL_POSITION

	select @cols2 = COALESCE(@cols2 + ' + '','' ', '') + 
		case when data_type = 'varchar' or data_type = 'char' then ' + '''''''' + ' + column_name + ' + '''''''' + '  
		else ' + cast(' + COLUMN_NAME + ' as varchar(20))' end
	from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @name order by ORDINAL_POSITION

	set @cols2 = '''(''' + @cols2 + ' + '')'''

	set @tsql = 'insert into #tmpcopy (value) select ' + @cols2 + ' from ' + @name

	exec (@tsql)

	select @insert = COALESCE(@insert + ',','') + value from #tmpcopy 

	if exists(select column_name, table_name from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = 'dbo' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and table_name = @name)
	begin
		insert into @stmts (stmt) values ('SET IDENTITY_INSERT ' + @name + ' ON'), ('insert into '+@name+' (' + @cols + ') values ' + @insert), ('SET IDENTITY_INSERT ' + @name + ' OFF')
	end
	else
	begin
		insert into @stmts (stmt) values ('insert into '+@name+' (' + @cols + ') values ' + @insert)
	end
	
	delete from #tmpcopy

	fetch next from @tc into @name

end

select * from @stmts

close @tc
deallocate @tc

drop table #tmpcopy