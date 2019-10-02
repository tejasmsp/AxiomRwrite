

truncate table OrderBilling

Insert Into OrderBilling (
 OrderId
,BillToOrderingFirm
,BillingFirmId
,BillingAttorneyId
,BillingDateOfLoss
,BillingClaimNo
,BillingInsured
,CreatedBy
,CreatedDate
,UpdatedBy
,UpdatedDate
)
Select
OrderNo
,NULL
,BillFirm
,BillAtty
,LossDate
,ClaimNo
,Insured
,EntBy
,EntDate
,ChngBy
,ChngDate


FROM [orders]

