SET IDENTITY_INSERT OrderDetail ON

truncate table OrderDetail

Insert Into OrderDetail (
 OrderId
,OrderNo
,AttorneyFor
,OrderingAttorney
,OrderingFirmID
,Represents
,CurrentStepId
,OrderBy
,AuthBy
,OrderDate
,IsDeleted
,IsFromClient
,CompanyNo
,SubmitStatus
,OrdDate
,CancelDate
,IsArchive
,CreatedBy
,CreatedDate
,UpdatedBy
,UpdatedDate
,CreatedByUserAccessId
,ModifiedByByUserAccessId
,AcctRep

)
Select
OrderNo
,OrderNo
,NULL
,OrdAtty
,NULL
,Represent
,1
,OrdBy
,AuthBy
,OrdDate
,IsArchive
,IsFromClient
,NULL
,1
,OrdDate
,CancelDate
,IsArchive
,EntBy
,EntDate
,ChngBy
,ChngDate
,NULL
,NULL
,NULL

FROM [orders] 

UPDATE od  SET od.OrderingFirmID  =a.FirmID
			FROM OrderDetail od INNER JOIN dbo.Attorney a ON a.AttyID = od.OrderingAttorney

UPDATE od  SET od.AttorneyFor  = a.RepType
			FROM dbo.OrderDetail od INNER JOIN dbo.Orders a ON a.OrderNo=od.OrderNo

UPDATE od  SET od.companyno  = a.CompNo
			FROM dbo.OrderDetail od INNER JOIN dbo.Orders a ON a.OrderNo=od.OrderNo
			
SET IDENTITY_INSERT OrderDetail OFF

