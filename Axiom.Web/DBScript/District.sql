--Created By
Select * from District
Select * from District  obj Inner JOIN  UserMaster um ON obj.CreatedBy = um.UserId

ALTER TABLE District Alter COLUMN CreatedBy varchar(100)
UPDATE obj SET  obj.CreatedBy = um.UserAccessId FROM  District obj  INNER JOIN UserMaster AS um
ON  ISNULL(obj.CreatedBy, -1) = CAST(um.UserId AS VARCHAR(100))

Select * from District obj Inner JOIN  UserMaster um ON ISNULL(obj.CreatedBy, -1) = CAST(um.UserAccessId AS varchar(100))

--ALTER TABLE County Alter COLUMN CreatedBy INT-- throw error, so follow below stmt
UPDATE obj SET obj.CreatedBy = -1 FROM  District obj  where ISNUMERIC(CreatedBy) = 0


ALTER TABLE District Alter COLUMN CreatedBy INT




--UpdatedBy

Select * from District
Select * from District  obj Inner JOIN  UserMaster um ON obj.UpdatedBy = um.UserId

ALTER TABLE District Alter COLUMN UpdatedBy varchar(100)
UPDATE obj SET  obj.UpdatedBy = um.UserAccessId FROM  District obj  INNER JOIN UserMaster AS um
ON  ISNULL(obj.UpdatedBy, -1) = CAST(um.UserId AS VARCHAR(100))

Select * from District obj Inner JOIN  UserMaster um ON ISNULL(obj.UpdatedBy, -1) = CAST(um.UserAccessId AS varchar(100))

--ALTER TABLE tablename Alter COLUMN UpdatedBy INT-- throw error, so follow below stmt
UPDATE obj SET obj.UpdatedBy = -1 FROM  District obj  where ISNUMERIC(UpdatedBy) = 0


ALTER TABLE District Alter COLUMN UpdatedBy INT

