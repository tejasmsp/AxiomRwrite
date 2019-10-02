ALTER TABLE CustomMessage ADD CreatedBy INT;
ALTER TABLE CustomMessage ADD CreatedDate Datetime;
ALTER TABLE CustomMessage ADD UpdatedBy INT;
ALTER TABLE CustomMessage ADD UpdatedDate Datetime;

UPDATE CustomMessage set CreatedBy=-1 where ISNUMERIC(CreatedBy)=0
UPDATE CustomMessage set UpdatedBy=-1 where ISNUMERIC(UpdatedBy)=0