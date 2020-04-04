-------Server_1_Count ----------ServerCountColumnName should be exists in both dbs
--- spGetRowCount 'Table1'
CREATE PROCEDURE [dbo].[spGetRowCount]
	@TableName VARCHAR(500)
AS
BEGIN
	DECLARE @sql nvarchar(MAX)

	SELECT
		@sql = COALESCE(@sql + ' UNION ALL ', '') +
			'SELECT
				''' + s.name + ''' AS ''Schema'',
				''' + t.name + ''' AS ''Table'',
				COUNT(*) AS Server_2_Count 
				FROM ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
		FROM sys.schemas s
		INNER JOIN sys.tables t ON t.schema_id = s.schema_id
		WHERE t.name = @TableName
		ORDER BY
			s.name,
			t.name

	EXEC(@sql)
END