CREATE PROCEDURE [dbo].[Common_GetPagingView]
	@PageIndex				int,	-- Note : First page index is 1
	@PageSize				int,
	@TablesName				nvarchar(400),
	@SelectedFields			nvarchar(4000)= '*',
	@SortFields				nvarchar(200),
	@Filter					nvarchar(600) = null,
	@RecordCount			int
as
	set nocount on

	declare @PageIndexLowerBound int, @PageIndexUperBound int
	set @PageIndexLowerBound = (@PageIndex - 1) * @PageSize + 1
	set @PageIndexUperBound = @PageIndexLowerBound + @PageSize - 1

	declare @TopRecords int
	set @TopRecords = @PageIndex * @PageSize
	
	if @SelectedFields = ''
		set @SelectedFields = '*'

	if @Filter is null
		set @Filter = ''
	else if @Filter <> ''
		set @Filter = ' where ' + @Filter
		
	declare @Sql nvarchar(2000)

	set @Sql = N'
	;with PageView_RecordsLimit as (
		select top ' + cast(@TopRecords as nvarchar(50)) + N' ' + 
		@SelectedFields + ' from ' + @TablesName + @Filter + N' order by ' + @SortFields + N'
	),
	PageView_Records as (
		select *, row_number() over (' + N' order by ' + @SortFields + N') as PageView_RowNum
		 from PageView_RecordsLimit
	)
	select * from PageView_Records where PageView_RowNum between ' + 
	cast(@PageIndexLowerBound as nvarchar(50)) + N' and ' + cast(@PageIndexUperBound as nvarchar(50)) + N'
	order by PageView_RowNum asc'
--RAISERROR (@Sql,16,1)
	exec sp_executesql @Sql
	
	if @RecordCount = 0 begin
		set @Sql = N'select count(*) as RecordCount from ' + @TablesName + @Filter
		exec sp_executesql @Sql
	end else
		select @RecordCount
		
GO
