TRUNCATE TABLE  dbo.UserMaster


SET IDENTITY_INSERT dbo.UserMaster ON;  
INSERT INTO dbo.UserMaster
(
    UserAccessId,
    UserId,
    UserName,
    Password,
    Email,
    IsApproved,
    IsLockedOut,
    LastLoginDate,
    FailedPasswordAttemptCount,
    IsRequestedForUnlock,
    CreatedBy,
    CreatedDate
)
VALUES
(   -1,
    NEWID(),      -- UserId - uniqueidentifier
    'system@system.com',       -- UserName - nvarchar(256)
    'system@system.com',       -- Password - nvarchar(128)
    'system@system.com',       -- Email - nvarchar(256)
    0,      -- IsApproved - bit
    0,      -- IsLockedOut - bit
    GETDATE(), -- LastLoginDate - datetime
    0,         -- FailedPasswordAttemptCount - int
    0,         -- IsRequestedForUnlock - int
    -1,      -- CreatedBy - uniqueidentifier
    GETDATE() -- CreatedDate - datetime    
    )

SET IDENTITY_INSERT dbo.UserMaster OFF; 

------------------Insert aspnet_Membership

INSERT INTO  dbo.UserMaster
(
    UserId,
    UserName,
    Password,
    Email,
    IsApproved,
    IsLockedOut,
    LastLoginDate,
    FailedPasswordAttemptCount,
    IsRequestedForUnlock,
    CreatedBy,
    CreatedDate   
)
SELECT 
 AM.UserId
,AU.UserName
,'Y2VtQDEyMw==-zKiiouzBU/g=' --cem@123
,AM.Email
,AM.IsApproved
,AM.IsLockedOut
,AM.LastLoginDate
,AM.FailedPasswordAttemptCount
,0
,-1
,GETDATE()
FROM aspnet_Membership AM INNER JOIN aspnet_Users AU ON AM.UserId=AU.UserId



-------------------RolesMaster-------------
TRUNCATE Table RolesMaster

INSERT INTO RolesMaster
			(RoleId,
			RoleName,
			Description,
			IsActive,
			CreatedBy
			,CreatedDate)
			SELECT
			RoleId,
			RoleName,
			'',
			1,
			-1,
            GETDATE()
			FROM dbo.aspnet_Roles
			

truncate table UserRoles
			
INSERT INTO UserRoles
			(UserId
			,RoleId
			,CreatedBy
			,CreatedDate)
SELECT
		 UserId
		 ,RoleId
		 ,-1
		 ,GETDATE() FROM aspnet_UsersInRoles

----Update UserInformation to UserMaster

UPDATE UM  
	Set
    UM.FirstName=UI.FirstName
   ,UM.LastName=UI.LastName
   ,UM.AttorneyEmployeeTypeId=UI.AttorneyEmployeeTypeId
   FROM UserMaster UM INNER JOIN UserInformation UI ON UM.UserId=UI.UserId