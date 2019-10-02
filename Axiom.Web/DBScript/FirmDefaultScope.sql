ALTER TABLE FirmDefaultScope ADD CreatedBy INT;
ALTER TABLE FirmDefaultScope ADD CreatedDate Datetime;
ALTER TABLE FirmDefaultScope ADD UpdatedBy INT;
ALTER TABLE FirmDefaultScope ADD UpdatedDate Datetime;
UPDATE FirmDefaultScope set CreatedBy=-1 where ISNUMERIC(CreatedBy)=0
UPDATE FirmDefaultScope set UpdatedBy=-1 where ISNUMERIC(UpdatedBy)=0