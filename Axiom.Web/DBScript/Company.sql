
ALTER TABLE Company ADD CreatedBy INT;
ALTER TABLE Company ADD CreatedDate Datetime;
ALTER TABLE Company ADD UpdatedBy INT;
ALTER TABLE Company ADD UpdatedDate Datetime;

UPDATE Company set CreatedBy=-1 where ISNUMERIC(CreatedBy)=0
UPDATE Company set UpdatedBy=-1 where ISNUMERIC(UpdatedBy)=0