-- Add indexes to the HelloMessages table for better performance
CREATE INDEX IX_HelloMessages_Name ON HelloMessages(Name);
CREATE INDEX IX_HelloMessages_CreatedAt ON HelloMessages(CreatedAt);