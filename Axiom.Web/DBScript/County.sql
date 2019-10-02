--Created By

Select * from County c Inner JOIN  UserMaster um ON ISNULL(c.CreatedBy, -1) = CAST(um.UserId AS varchar(100))

ALTER TABLE County Alter COLUMN CreatedBy varchar(100)
UPDATE c SET     c.CreatedBy = um.UserAccessId FROM  County c  INNER JOIN UserMaster AS um
ON  ISNULL(c.CreatedBy, -1) = CAST(um.UserId AS VARCHAR(100))

Select * from County c Inner JOIN  UserMaster um ON ISNULL(c.CreatedBy, -1) = CAST(um.UserAccessId AS varchar(100))

--ALTER TABLE County Alter COLUMN CreatedBy INT-- throw error, so follow below stmt
UPDATE c SET     c.CreatedBy = -1 FROM  County c  where ISNUMERIC(CreatedBy) = 0


ALTER TABLE County Alter COLUMN CreatedBy INT

--Update By

Select * from County c Inner JOIN  UserMaster um ON c.UpdatedBy = um.UserId

ALTER TABLE County Alter COLUMN UpdatedBy varchar(100)

UPDATE c SET c.CreatedBy = um.UserAccessId FROM  County c
INNER JOIN UserMaster AS um
ON  ISNULL(c.UpdatedBy, -1) = CAST(um.UserId AS VARCHAR(100))

Select * from County c Inner JOIN  UserMaster um ON ISNULL(c.UpdatedBy, -1) = CAST(um.UserAccessId AS varchar(100))

--ALTER TABLE County Alter COLUMN UpdatedBy INT-- throw error, so follow below stmt
UPDATE c SET     c.UpdatedBy = -1 FROM  County c  where ISNUMERIC(UpdatedBy) = 0

ALTER TABLE County Alter COLUMN UpdatedBy INT


