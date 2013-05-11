
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
ALTER FUNCTION dbo.JoinIdToTable
(	
	@JoinId VARCHAR(1000)
)
RETURNS @IdList TABLE (
	Id INT,
	RowNumber INT
)
AS
BEGIN

	DECLARE @Pos INT;
	DECLARE @StrId varchar(20);
	SET @Pos=1;
	DECLARE @RowNumber INT;
	SET @RowNumber=1;

	WHILE(@Pos<>0)
	BEGIN

		SET @Pos=CHARINDEX(',',@JoinId);
		IF @Pos=0
			SET @StrId=@JoinId;
		ELSE
			SET @StrId=SUBSTRING(@JoinId,1,@Pos-1);
		
		SET @JoinId=STUFF(@JoinId,1,@Pos,'');
		IF(ISNUMERIC(@StrId)=1)
		BEGIN
			INSERT INTO @IdList VALUES(CAST(@StrId AS INT),@RowNumber)
			SET @RowNumber=@RowNumber+1;
		END
		
	END

	RETURN

END

GO
