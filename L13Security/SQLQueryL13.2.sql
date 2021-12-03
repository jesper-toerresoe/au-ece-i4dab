CREATE DATABASE [L13Security]
GO
USE [L13Security]
GO
CREATE TABLE [dbo].[UserInfo] (
    [id]         INT                                                              IDENTITY (1, 1) NOT NULL,
    [FirstName]  NVARCHAR (15)                                                     NULL,
    [LastName]   NVARCHAR (15) MASKED WITH (FUNCTION = 'default()')                NULL,
    [CreditCard] NVARCHAR (25) MASKED WITH (FUNCTION = 'partial(4, "XXXXXXX", 0)') NULL,
    [Email]      NVARCHAR (25) MASKED WITH (FUNCTION = 'email()')                  NULL,
    [DocNumber]  BIGINT MASKED WITH (FUNCTION = 'random(111111, 999999)')         NULL
)
GO
-- https://datoptim.com/how-to-mask-data-in-sql-server/

SELECT * FROM UserInfo;
