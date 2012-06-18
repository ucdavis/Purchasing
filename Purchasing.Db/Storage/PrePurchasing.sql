ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [PrePurchasing], FILENAME = 'E:\DB\PrePurchasing.mdf', SIZE = 243712 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

