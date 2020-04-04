-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--- spGetRowCount 'Table1'
CREATE PROCEDURE spGetRowCount
	@TableName VARCHAR(500)
AS
BEGIN
	DECLARE @sql nvarchar(MAX)

	SELECT
		@sql = COALESCE(@sql + ' UNION ALL ', '') +
			'SELECT
				''' + s.name + ''' AS ''Schema'',
				''' + t.name + ''' AS ''Table'',
				COUNT(*) AS Count
				FROM ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
		FROM sys.schemas s
		INNER JOIN sys.tables t ON t.schema_id = s.schema_id
		WHERE t.name = @TableName
		ORDER BY
			s.name,
			t.name

	EXEC(@sql)
END
GO
