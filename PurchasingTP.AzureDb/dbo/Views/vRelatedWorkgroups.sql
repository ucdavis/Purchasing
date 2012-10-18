CREATE VIEW dbo.vRelatedWorkgroups
AS
SELECT 
       row_number() over (order by AdminWorkgroupId) id 
      ,WorkgroupId
      ,Name
      ,PrimaryOrganizationId
      ,AdminWorkgroupId
      ,AdminOrgId
FROM (
      SELECT DISTINCT 
              w.id WorkgroupId
            , w.Name
            , w.PrimaryOrganizationId
            , w.Administrative
            , od.RollupParentId
            , p.ParentWorkgroupId AdminWorkgroupId
            , p.ParentOrgId AdminOrgId
            , w.IsActive
      FROM dbo.WorkgroupsXOrganizations wxo
            INNER JOIN dbo.Workgroups w ON wxo.WorkgroupId = w.Id
            INNER JOIN dbo.vOrganizationDescendants od ON wxo.OrganizationId = od.OrgId
            INNER JOIN (
                /* This gets all the parent orgs */
                  SELECT DISTINCT wxo.WorkgroupId ParentWorkgroupId, wxo.OrganizationId ParentOrgId, w.IsActive
                  FROM dbo.WorkgroupsXOrganizations wxo
                  INNER JOIN dbo.Workgroups w ON wxo.WorkgroupId = w.Id
                  INNER JOIN dbo.vOrganizationDescendants od ON wxo.OrganizationId = od.RollupParentId 
                  WHERE 
                        w.Administrative = 1 AND 
                        w.IsActive = 1
                  ) p ON od.RollupParentId = p.ParentOrgId
            ) t2
WHERE t2.Administrative = 0 AND t2.IsActive = 1
