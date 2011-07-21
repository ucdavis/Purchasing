ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [PrePurchasing], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

