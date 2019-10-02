

truncate table OrderCase

Insert Into OrderCase (
 OrderId
,CaseTypeId
,Caption1
,VsText1
,Caption2
,VsText2
,Caption3
,VsText3
,CauseNo
,TrialDate
,[State]
,Rush
,IsStateOrFedral
,County
,Court
,District
,Division
,CreatedBy
,CreatedDate
,UpdatedBy
,UpdatedDate

)
Select
OrderNo
,NULL
,FormStyle1
,FormStyle5
,FormStyle2
,FormStyle6
,FormStyle3
,FormStyle7
,CauseNo
,TrialDate
,State
,Rush
,Federal
,County
,Judicial
,District
,Division
,EntBy
,EntDate
,ChngBy
,ChngDate

FROM [orders]

