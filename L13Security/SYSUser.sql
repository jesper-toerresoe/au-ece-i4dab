CREATE TABLE [dbo].[SYSUser]
(
	[UserId] BIGINT NOT NULL PRIMARY KEY, 
    [LoginName] NVARCHAR(50) NULL, 
    [PasswordEncryptedText] NVARCHAR(50) NULL
)
