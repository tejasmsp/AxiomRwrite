
--Quick Note

ALTER TABLE PartStatuses ADD CreatedBy INT;
ALTER TABLE PartStatuses ADD CreatedDate Datetime;
ALTER TABLE PartStatuses ADD UpdatedBy INT;
ALTER TABLE PartStatuses ADD UpdatedDate Datetime;

UPDATE PartStatuses set CreatedBy=-1 where ISNUMERIC(CreatedBy)=0
UPDATE PartStatuses set UpdatedBy=-1 where ISNUMERIC(UpdatedBy)=0
