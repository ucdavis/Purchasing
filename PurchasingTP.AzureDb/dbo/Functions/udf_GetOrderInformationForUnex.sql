-- =============================================
-- Author:		Scott Kirkland
-- Create date: 3/21/2017
-- Description:	Downloads all order information for UNEX workgroups (under 3-UNEX org) for the last 2 years
-- =============================================
CREATE FUNCTION [dbo].[udf_GetOrderInformationForUnex] 
(	
)

RETURNS TABLE 
AS
RETURN 
(
	select * from vorderhistory orders
	where workgroupid in (
		select distinct workgroupid from vorganizationdescendants od
		inner join workgroupsxorganizations wo on od.orgid = wo.organizationid
		inner join workgroups w on wo.workgroupid = w.id
		where od.rollupparentid = '3-UNEX' and od.isactive = 1 and w.isActive = 1
	)
	and datecreated > (getdate() - 730) and requesttype = 'Other'
)