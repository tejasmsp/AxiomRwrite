ALTER TABLE AttorneyDefaultScope ADD CreatedBy INT;
ALTER TABLE AttorneyDefaultScope ADD CreatedDate Datetime;
ALTER TABLE AttorneyDefaultScope ADD UpdatedBy INT;
ALTER TABLE AttorneyDefaultScope ADD UpdatedDate Datetime;

UPDATE AttorneyDefaultScope set CreatedBy=-1 where ISNUMERIC(CreatedBy)=0
UPDATE AttorneyDefaultScope set UpdatedBy=-1 where ISNUMERIC(UpdatedBy)=0
