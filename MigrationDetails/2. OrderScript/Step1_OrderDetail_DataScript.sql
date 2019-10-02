SET IDENTITY_INSERT OrderDetail ON

truncate table OrderDetail

Insert Into OrderDetail (
OrderId
,OrderNo
,AttorneyFor
,OrderingAttorney
,Represents
,CurrentStepId
,OrderBy
,AuthBy
,OrderDate
,IsDeleted
,IsFromClient
,CreatedBy
,CreatedDate
,UpdatedBy
,UpdatedDate
,AcctRep
,Processor
,ProgID
,StartDate
,WaiverDate
,DueDate
,CompanyNo
)
SELECT
OrderNo
,OrderNo
,RepType
,OrdAtty
,Represent
,1
,OrdBy
,AuthBy
,OrdDate
,IsArchive
,IsFromClient
,EntBy
,EntDate
,ChngBy
,ChngDate
,AcctRep
,Processor
,progID
,StartDate
,WaiverDate
,DueDate
,CompNo
FROM [orders]

SET IDENTITY_INSERT OrderDetail OFF