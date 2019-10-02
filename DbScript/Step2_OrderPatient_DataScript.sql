
ALTER TABLE OrderPatient ALTER COLUMN SSN VARCHAR(50)

truncate table OrderPatient

Insert Into OrderPatient (
 OrderId
,PatientTypeId
,Name
,SSN
,DateOfBirth
,DateOfDeath
,Address1
,Address2
,City
,StateId
,ZipCode
,ClaimMatterNo
,CreatedBy
,CreatedDate
,UpdatedBy
,UpdatedDate

)
Select
 OrderNo
,PatType
,Patient
,SSN
,BirthDate
,DeathDate
,PtntAddr1
,PtntAddr2
,PtntCity
,PtntState
,PtntZip
,CliMatNo
,EntBy
,EntDate
,ChngBy
,ChngDate

FROM [orders]

