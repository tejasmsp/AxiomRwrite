
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
,CASE WHEN BillFirm IS NULL THEN 0 ELSE  1 end
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

UPDATE orderbilling SET billtoorderingfirm=0 WHERE billingfirmid   ='        '      